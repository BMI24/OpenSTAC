using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Nodify.Calculator
{
    public class EditorViewModel : ObservableObject
    {
        public event Action<EditorViewModel, CalculatorViewModel>? OnOpenInnerCalculator;

        public EditorViewModel? Parent { get; set; }

        public EditorViewModel()
        {
            Calculator = new CalculatorViewModel();
        }

        public Guid Id { get; } = Guid.NewGuid();

        private CalculatorViewModel _calculator = default!;
        public CalculatorViewModel Calculator 
        {
            get => _calculator;
            set => SetProperty(ref _calculator, value);
        }

        private string? _name;
        public string? Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
    }
}
