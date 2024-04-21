using Newtonsoft.Json;
using STAC.Components;
using System;
using System.Windows;

namespace Nodify.Calculator
{
    public class OperationViewModel : ObservableObject
    {
        public OperationViewModel()
        {
            Input.WhenAdded(x =>
            {
                x.Operation = this;
                x.IsInput = true;
            });
        }

        private Point _location;
        public Point Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }

        private Size _size;
        public Size Size
        {
            get => _size;
            set => SetProperty(ref _size, value);
        }

        private string? _title;
        public string? Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public bool IsReadOnly { get; set; }

        public NodifyObservableCollection<ConnectorViewModel> Input { get; } = new NodifyObservableCollection<ConnectorViewModel>();

        private ConnectorViewModel? _output;
        public ConnectorViewModel? Output
        {
            get => _output;
            set
            {
                if (SetProperty(ref _output, value) && _output != null)
                {
                    _output.Operation = this;
                }
            }
        }
        public Type PayloadType { get; set; }
        [JsonIgnore]
        public Func<IComponent> PayloadConstructor => () => (IComponent)Activator.CreateInstance(PayloadType)!;
    }
}
