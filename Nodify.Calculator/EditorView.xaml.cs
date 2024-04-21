using Newtonsoft.Json;
using OpenTK.Windowing.Desktop;
using STAC.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Linq;

namespace Nodify.Calculator
{
    public partial class EditorView : UserControl
    {
        public EditorView()
        {
            InitializeComponent();

            EventManager.RegisterClassHandler(typeof(NodifyEditor), MouseLeftButtonDownEvent, new MouseButtonEventHandler(CloseOperationsMenu));
            EventManager.RegisterClassHandler(typeof(ItemContainer), ItemContainer.DragStartedEvent, new RoutedEventHandler(CloseOperationsMenu));
            EventManager.RegisterClassHandler(typeof(NodifyEditor), MouseRightButtonUpEvent, new MouseButtonEventHandler(OpenOperationsMenu));
        }

        private void OpenOperationsMenu(object sender, MouseButtonEventArgs e)
        {
            if (!e.Handled && e.OriginalSource is NodifyEditor editor && !editor.IsPanning && editor.DataContext is CalculatorViewModel calculator)
            {
                e.Handled = true;
                calculator.OperationsMenu.OpenAt(editor.MouseLocation);
            }
        }

        private void CloseOperationsMenu(object sender, RoutedEventArgs e)
        {
            ItemContainer? itemContainer = sender as ItemContainer;
            NodifyEditor? editor = sender as NodifyEditor ?? itemContainer?.Editor;

            if (!e.Handled && editor?.DataContext is CalculatorViewModel calculator)
            {
                calculator.OperationsMenu.Close();
            }
        }

        [JsonIgnore]
        GameWindow Window;
        private void Node_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OperationViewModel? GetIngoingConnectedOperation(ConnectorViewModel? connector)
            {
                if (connector == null)
                    return null;

                while (connector != null && (connector.IsInput || connector.IsKnot))
                {
                    connector = connector.IngoingConnections.FirstOrDefault();
                }

                return connector?.Operation;
            }


            Node clickedNode = (Node)sender;
            OperationViewModel mainOperation = (OperationViewModel)clickedNode.DataContext;

            if (mainOperation.Title != nameof(MainComponent))
                return;
            

            HashSet<OperationViewModel> processedOps = new();
            Stack<OperationViewModel> processingStack = new();
            Dictionary<OperationViewModel, IComponent> opToComponent = new();

            IComponent getComponent(OperationViewModel operation)
            {
                if (!opToComponent.ContainsKey(operation))
                    opToComponent[operation] = operation.PayloadConstructor();
                return opToComponent[operation];
            }

            processingStack.Push(mainOperation);

            while (processingStack.Count > 0) 
            { 
                var operation = processingStack.Pop();
                if (processedOps.Contains(operation))
                    continue;

                processedOps.Add(operation);
                var component = getComponent(operation);

                var componentType = component.GetType();

                foreach (var input in operation.Input)
                {
                    var property = componentType.GetProperty(input.PropertyName!)!;

                    if (input.IsValueOnlyInput)
                    {
                        property.SetValue(component, input.ValueChecker!.ParsedObject);
                    }
                    else if (input.MultiInputAllowed)
                    {
                        var list = property.GetValue(component);
                        var clearMethod = property.PropertyType.GetMethod("Clear")!;
                        clearMethod.Invoke(list, null);
                        var addMethod = property.PropertyType.GetMethod("Add")!;
                        
                        foreach (var inputOperations in input.IngoingConnections
                            .Select(GetIngoingConnectedOperation)
                            .Where(o =>  o != null))
                        {
                            processingStack.Push(inputOperations!);
                            addMethod.Invoke(list, new object[] { getComponent(inputOperations!) });
                        }
                    }
                    else
                    {
                        var connectedOperation = GetIngoingConnectedOperation(input);
                        if (connectedOperation == null)
                            return;

                        processingStack.Push(connectedOperation!);
                        property.SetValue(component, getComponent(connectedOperation));
                    }
                }
            }

            MainComponent mainComponent = (MainComponent)getComponent(mainOperation);
            STAC.Program.RunParams oldWindowData = default;

            if (Window != null)
            {
                oldWindowData = new STAC.Program.RunParams(Window.Location, (Window as STAC.Window)?.Camera);
                Window?.Close();
            }
            Window = STAC.Program.Run(mainComponent, oldWindowData);
        }

    }
}
