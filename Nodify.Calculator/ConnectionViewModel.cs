using System.Windows;
using System.Windows.Input;

namespace Nodify.Calculator
{
    public class ConnectionViewModel : ObservableObject
    {
        private ConnectorViewModel _input = default!;
        public ConnectorViewModel Input
        {
            get => _input;
            set => SetProperty(ref _input, value);
        }

        private ConnectorViewModel _output = default!;
        public ICommand SplitCommand { get; }
        public ConnectorViewModel Output
        {
            get => _output;
            set => SetProperty(ref _output, value);
        }
        private CalculatorViewModel _graph = default!;
        public CalculatorViewModel Graph
        {
            get => _graph;
            internal set => SetProperty(ref _graph, value);
        }
        public void Split(Point point)
            => Graph.SplitConnection(this, point);
        public ConnectionViewModel()
        {
            SplitCommand = new DelegateCommand<Point>(Split);
        }
    }
}
