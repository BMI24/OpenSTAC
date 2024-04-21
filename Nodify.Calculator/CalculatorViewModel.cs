using Newtonsoft.Json;
using System;
using System.Linq;
using System.Windows;

namespace Nodify.Calculator
{
    public class CalculatorViewModel : ObservableObject
    {
        public CalculatorViewModel Clone()
        {
            return Serializer.Clone(this);
        }
        public CalculatorViewModel()
        {
            Init();
        }

        private void Init()
        {
            CreateConnectionCommand = new DelegateCommand<ConnectorViewModel>(
                            _ => CreateConnection(PendingConnection.Source, PendingConnection.Target),
                            _ => CanCreateConnection(PendingConnection.Source, PendingConnection.Target));
            StartConnectionCommand = new DelegateCommand<ConnectorViewModel>(_ => PendingConnection.IsVisible = true, (c) => !(c.IsConnected && c.IsInput));
            DisconnectConnectorCommand = new DelegateCommand<ConnectorViewModel>(DisconnectConnector);
            DeleteSelectionCommand = new DelegateCommand(DeleteSelection);

            Connections.ClearListeners().WhenAdded(c =>
            {
                c.Input.IsConnected = true;
                c.Output.IsConnected = true;
                c.Input.IngoingConnections.Add(c.Output);
                c.Graph = this;
            })
            .WhenRemoved(c =>
            {
                var ic = Connections.Count(con => con.Input == c.Input || con.Output == c.Input);
                var oc = Connections.Count(con => con.Input == c.Output || con.Output == c.Output);

                if (ic == 0)
                {
                    c.Input.IsConnected = false;
                }

                if (oc == 0)
                {
                    c.Output.IsConnected = false;
                }

                c.Input.IngoingConnections.Remove(c.Output);
            });

            Operations.ClearListeners().WhenAdded(x =>
            {
                x.Input.WhenRemoved(RemoveConnection);

                if (x is CalculatorInputOperationViewModel ci)
                {
                    ci.Output.WhenRemoved(RemoveConnection);
                }

                void RemoveConnection(ConnectorViewModel i)
                {
                    var c = Connections.Where(con => con.Input == i || con.Output == i).ToArray();
                    c.ForEach(con => Connections.Remove(con));
                }
            })
            .WhenRemoved(x =>
            {
                if (x is KnotNodeViewModel knot)
                {
                    DisconnectConnector(knot.Connector);
                    return;
                }
                foreach (var input in x.Input)
                {
                    DisconnectConnector(input);
                }

                if (x.Output != null)
                {
                    DisconnectConnector(x.Output);
                }
            });

            OperationsMenu = new OperationsMenuViewModel(this);
        }

        private NodifyObservableCollection<OperationViewModel> _operations = new NodifyObservableCollection<OperationViewModel>();
        public NodifyObservableCollection<OperationViewModel> Operations
        {
            get => _operations;
            set => SetProperty(ref _operations, value);
        }

        private NodifyObservableCollection<OperationViewModel> _selectedOperations = new NodifyObservableCollection<OperationViewModel>();
        public NodifyObservableCollection<OperationViewModel> SelectedOperations
        {
            get => _selectedOperations;
            set => SetProperty(ref _selectedOperations, value);
        }

        public NodifyObservableCollection<ConnectionViewModel> Connections { get; } = new NodifyObservableCollection<ConnectionViewModel>();
        [JsonIgnore]
        public PendingConnectionViewModel PendingConnection { get; set; } = new PendingConnectionViewModel();
        [JsonIgnore]
        public OperationsMenuViewModel OperationsMenu { get; set; }
        [JsonIgnore]
        public INodifyCommand StartConnectionCommand { get; private set; }
        [JsonIgnore]
        public INodifyCommand CreateConnectionCommand { get; private set; }
        [JsonIgnore]
        public INodifyCommand DisconnectConnectorCommand { get; private set; }
        [JsonIgnore]
        public INodifyCommand DeleteSelectionCommand { get; private set; }

        private void DisconnectConnector(ConnectorViewModel connector)
        {
            DisconnectConenctorInputOutputs(connector);
        }

        private void DisconnectConenctorInputOutputs(ConnectorViewModel connector, bool disconnectInputs = true, bool disconnectOutputs = true)
        {
            var connections = Connections.Where(c => (disconnectInputs && c.Input == connector) || (disconnectOutputs && c.Output == connector)).ToList();
            connections.ForEach(c => Connections.Remove(c));
        }

        internal bool CanCreateConnection(ConnectorViewModel source, ConnectorViewModel? target)
        {
            return target == null 
                || (source != target 
                    && source.Operation != target.Operation 
                    && (source.IsInput != target.IsInput || target.Operation is KnotNodeViewModel)
                    && source.Type.Satisfies(target.Type)
                    && !Connections.Any(c => (c.Output == source || c.Output == target) && (c.Input == source || c.Input == target))
                    )
                || PendingConnection.IsVisible;
        }

        internal void CreateConnection(ConnectorViewModel source, ConnectorViewModel? target)
        {
            if (target == null)
            {
                PendingConnection.IsVisible = true;
                ConnectorType? minType = !source.IsKnot && source.IsInput ? source.Type : null;
                ConnectorType? maxType = source.IsKnot || !source.IsInput ? source.Type : null;
                OperationsMenu.OpenAt(PendingConnection.TargetLocation, minType, maxType);
                OperationsMenu.Closed += OnOperationsMenuClosed;
                return;
            }



            if (PendingConnection.IsVisible && target.IsInput)
            {
                target = target.Operation.Input.First(o => o.Type.IsSatisfiedBy(source.Type));
            }


            ConnectorViewModel input;
            ConnectorViewModel output;

            if (source.IsKnot && target.IsKnot)
            {
                input = source;
                output = target;
            }
            else if (source.IsKnot)
            {
                input = target.IsInput ? target : source;
                output = target.IsInput ? source : target;
            }
            else if (target.IsKnot)
            {
                input = source.IsInput ? source : target;
                output = source.IsInput ? target : source;
            }
            else
            {
                input = source.IsInput ? source : target;
                output = target.IsInput ? source : target;
            }

            PendingConnection.IsVisible = false;

            DisconnectConenctorInputOutputs(input, !input.MultiInputAllowed, !input.IsKnot);

            Connections.Add(new ConnectionViewModel
            {
                Input = input,
                Output = output
            });
        }

        private void OnOperationsMenuClosed()
        {
            PendingConnection.IsVisible = false;
            OperationsMenu.Closed -= OnOperationsMenuClosed;
        }

        private void DeleteSelection()
        {
            var selected = SelectedOperations.ToList();
            selected.ForEach(o => Operations.Remove(o));
        }

        public void SplitConnection(ConnectionViewModel connection, Point location)
        {
            var input = connection.Input;
            var output = connection.Output;

            var knot = new KnotNodeViewModel
            {
                Location = location,
                Connector = new ConnectorViewModel
                {
                    Type = input.Type
                }
            };

            connection.Graph.Connections.Remove(connection);
            connection.Graph.Operations.Add(knot);

            connection.Graph.CreateConnection(input, knot.Connector);
            connection.Graph.CreateConnection(knot.Connector, output);
        }
    }
}

