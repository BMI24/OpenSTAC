using Nodify.Calculator.Converters.ValueChecker;
using OpenTK.Mathematics;
using STAC;
using STAC.Components;
using STAC.Components.CameraRay;
using STAC.Components.Color;
using STAC.Components.March;
using STAC.Components.Normals;
using STAC.Components.SDF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using static Nodify.Calculator.OperationInfoViewModel;

namespace Nodify.Calculator
{
    public class OperationsMenuViewModel : ObservableObject
    {
        private bool _isVisible;
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                SetProperty(ref _isVisible, value);
                if (!value)
                {
                    Closed?.Invoke();
                }
            }
        }

        private Point _location;
        public Point Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }

        public event Action? Closed;

        public void OpenAt(Point targetLocation, ConnectorType? minType = null, ConnectorType? maxType = null)
        {
            Close();
            Location = targetLocation;
            AvailableOperations.Clear();
            if (minType?.Shape != ConnectorShape.None && maxType?.Shape != ConnectorShape.None)
                AvailableOperations.AddRange(AllOperations.Where(o =>
                    (minType == null || o.OutputType.Satisfies(minType.Value))
                    && (maxType == null || o.Input.Any(i => i.Type.IsSatisfiedBy(maxType.Value)))));
            IsVisible = true;
        }

        public void Close()
        {
            IsVisible = false;
        }

        public NodifyObservableCollection<OperationInfoViewModel> AllOperations { get; }
        public NodifyObservableCollection<OperationInfoViewModel> AvailableOperations { get; }
        public INodifyCommand CreateOperationCommand { get; }
        private readonly CalculatorViewModel _calculator;


        private Dictionary<Type, Func<IValueChecker>> SupportedStructs = new()
        {
            { typeof(int), () => new IntValueChecker() },
            { typeof(float), () =>  new FloatValueChecker() },
            { typeof(Vector3), () => new Vector3ValueChecker() },
            { typeof(Color4), () => new Color4ValueChecker() },
            { typeof(Point), () => new PointValueChecker() },
            { typeof(bool), () => new BoolValueChecker() },
        };

        ConnectorType GetConnectorType(Type t)
        {
            ConnectorShape shape = ConnectorShape.None;
            Color color = Color.FromRgb(0, 0, 0);
            if (!t.IsAssignableTo(typeof(IComponent)))
            {

            }
            else if (t.IsAssignableTo(typeof(ISDFComponent)))
            {
                shape = ConnectorShape.SDF;
                if (t.IsAssignableTo(typeof(IColorSDFComponent)))
                    color.R = 255;
            }
            else if (t.IsAssignableTo(typeof(INormalComponent)))
            {
                shape = ConnectorShape.Normals;
            }
            else if (t.IsAssignableTo(typeof(IMarchingComponent)))
            {
                shape = ConnectorShape.Marcher;
                if (t.IsAssignableTo(typeof(IFlexibleMarchingComponent)))
                    color.R = 255;
                if (t.IsAssignableTo(typeof(IColorMarchingComponent)))
                    color.G = 255;
            }
            else if (t.IsAssignableTo(typeof(IColorComponent)))
            {
                shape = ConnectorShape.Color;
            }
            else if (t.IsAssignableTo(typeof(ICameraRayComponent)))
            {
                shape = ConnectorShape.CameraRay;
            }
            else if (t.IsAssignableTo(typeof(MainComponent)))
            {
                shape = ConnectorShape.None;
            }

            return new(shape, color);
        }

        public static bool HasCustomAttribute<T>(PropertyInfo member) where T : Attribute
        {
            if (member.GetCustomAttribute<T>() != null)
                return true;

            var declaringType = member.DeclaringType!;
            foreach (var interfaceType in declaringType.GetInterfaces())
            {
                var interfaceMap = declaringType.GetInterfaceMap(interfaceType);
                var getAccessor = member.GetAccessors().FirstOrDefault(a => a.ReturnType != typeof(void));
                if (getAccessor == null)
                    continue;
                var accessorIndex = Array.IndexOf(interfaceMap.TargetMethods, getAccessor);
                if (accessorIndex == -1)
                    continue;

                var interfaceProperty = interfaceType.GetProperties()
                    .First(p =>
                    {
                        var getAccessor = p.GetAccessors().First(a => a.ReturnType != typeof(void));
                        var interfaceAccessorIndex = Array.IndexOf(interfaceMap.InterfaceMethods, getAccessor);
                        return interfaceAccessorIndex == accessorIndex;
                    });
                if (interfaceProperty.GetCustomAttribute<T>() != null)
                    return true;
            }

            return false;
        }

        public OperationsMenuViewModel(CalculatorViewModel calculator)
        {
            _calculator = calculator;

            List<OperationInfoViewModel> operations = new();

            foreach (Type t in typeof(MainComponent).Assembly
                .GetTypes()
                .Where(t => t.IsAssignableTo(typeof(IComponent)) && !t.IsInterface && !t.IsAbstract))
            {
                var title = t.Name;
                var outputShape = GetConnectorType(t);

                List<InputData> inputs = new();
                foreach (var p in t.GetProperties()
                    .Where(p => p.PropertyType != typeof(GenerationManager)
                        && !HasCustomAttribute<NotCustomizableAttribute>(p)))
                {
                    var inputTitle = p.Name;
                    if (inputTitle.EndsWith("Component"))
                        inputTitle = inputTitle[..^"Component".Length];
                    if (p.PropertyType.IsAssignableTo(typeof(IComponent)))
                    {
                        inputs.Add(new(inputTitle, p.Name, GetConnectorType(p.PropertyType), false));
                    }
                    else if (p.PropertyType.IsGenericType
                        && p.PropertyType.GetGenericTypeDefinition() == typeof(List<>)
                        && p.PropertyType.GenericTypeArguments.Length == 1
                        && typeof(IComponent).IsAssignableFrom(p.PropertyType.GenericTypeArguments[0]))
                    {
                        var genericType = p.PropertyType.GenericTypeArguments[0];
                        inputs.Add(new("List: " + inputTitle, p.Name, GetConnectorType(genericType), true));
                    }
                    else if (SupportedStructs.ContainsKey(p.PropertyType))
                    {
                        inputs.Add(new(inputTitle, p.Name, GetConnectorType(p.PropertyType), false, IsValueOnlyInput: true, SupportedStructs[p.PropertyType]));
                    }
                }


                operations.Add(new() { Title = title, Input = inputs, OutputType = outputShape, PayloadType = t });
            }

            AllOperations = new NodifyObservableCollection<OperationInfoViewModel>(operations);
            AvailableOperations = new();
            CreateOperationCommand = new DelegateCommand<OperationInfoViewModel>(CreateOperation);
        }

        private void CreateOperation(OperationInfoViewModel operationInfo)
        {

            OperationViewModel op = OperationFactory.GetOperation(operationInfo);
            op.Location = Location;

            bool shouldRightAlign = _calculator.PendingConnection.IsVisible && _calculator.PendingConnection.Source.IsInput;
            if (shouldRightAlign)
                RightAlign(op);

            _calculator.Operations.Add(op);

            var pending = _calculator.PendingConnection;
            if (pending.IsVisible)
            {
                var connector = pending.Source.IsInput ? op.Output : op.Input.FirstOrDefault();
                if (connector != null && _calculator.CanCreateConnection(pending.Source, connector))
                {
                    _calculator.CreateConnection(pending.Source, connector);
                }
            }
            Close();
        }

        private void RightAlign(OperationViewModel op)
        {
            void move()
            {
                op.Location -= new Vector(op.Size.Width, 0);
            }
            void Op_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                if (e.PropertyName != nameof(op.Size))
                    return;

                op.PropertyChanged -= Op_PropertyChanged;
                move();
            }
            if (op.Size.Width != 0)
            {
                move();
            }
            else
            {
                op.PropertyChanged += Op_PropertyChanged;
            }
        }
    }
}
