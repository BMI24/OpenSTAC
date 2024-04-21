using Nodify;
using Nodify.Calculator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Nodify.Calculator
{
    public class KnotNodeViewModel : OperationViewModel
    {
        private CalculatorViewModel _graph = default!;
        public CalculatorViewModel Graph
        {
            get => _graph;
            internal set => SetProperty(ref _graph, value);
        }
        private ConnectorViewModel _connector = default!;
        public ConnectorViewModel Connector
        {
            get => _connector;
            set
            {
                if (SetProperty(ref _connector, value))
                {
                    _connector.Operation = this;
                }
            }
        }
    }
}
