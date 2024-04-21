using Nodify.Calculator.Converters.ValueChecker;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Nodify.Calculator
{
    public enum ConnectorShape
    {
        None,
        CameraRay,
        Color,
        Marcher,
        Normals,
        SDF
    }
    public record struct ConnectorType(ConnectorShape Shape, Color Color)
    {
        public bool Satisfies(ConnectorType other)
        {
            return (Shape == other.Shape)
                && (Color.R >= other.Color.R)
                && (Color.G >= other.Color.G)
                && (Color.B >= other.Color.B);
        }

        public bool IsSatisfiedBy(ConnectorType other)
        {
            return other.Satisfies(this);
        }
    }

    public class ConnectorViewModel : ObservableObject
    {
        private string? _title;
        public string? Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string? _value;
        public string? Value
        {
            get => _value;
            set
            {
                var val = value;
                if (ValueChecker != null && val != null)
                    val = ValueChecker.ProcessInput(val);

                SetProperty(ref _value, val);
                ValueObservers.ForEach(o => o.Value = value);
            }
        }

        private string? _propertyName;
        public string? PropertyName
        {
            get => _propertyName;
            set => SetProperty(ref _propertyName, value);
        }

        private bool _isValueOnlyInput;
        public bool IsValueOnlyInput
        {
            get => _isValueOnlyInput;
            set => SetProperty(ref _isValueOnlyInput, value);
        }

        private IValueChecker? _valueChecker;
        public IValueChecker? ValueChecker
        {
            get => _valueChecker;
            set => SetProperty(ref _valueChecker, value);
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }

        private bool _isValueSettable;

        public bool IsValueSettable
        {
            get => _isValueSettable;
            set => SetProperty(ref _isValueSettable, value);
        }
        public bool IsKnot => Operation is KnotNodeViewModel;
        private bool _isInput;
        public bool IsInput
        {
            get => _isInput;
            set => SetProperty(ref _isInput, value);
        }

        private Point _anchor;
        public Point Anchor
        {
            get => _anchor;
            set => SetProperty(ref _anchor, value);
        }

        private OperationViewModel _operation = default!;
        public OperationViewModel Operation
        {
            get => _operation;
            set => SetProperty(ref _operation, value);
        }

        private ConnectorType _type;
        public ConnectorType Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        private bool _multiInputAllowed;
        public bool MultiInputAllowed
        {
            get => _multiInputAllowed;
            set => SetProperty(ref _multiInputAllowed, value);
        }

        public List<ConnectorViewModel> IngoingConnections { get; } = new List<ConnectorViewModel>();
        public List<ConnectorViewModel> ValueObservers { get; } = new List<ConnectorViewModel>();
    }
}
