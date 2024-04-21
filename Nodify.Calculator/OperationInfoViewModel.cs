using Nodify.Calculator.Converters.ValueChecker;
using STAC.Components;
using System;
using System.Collections.Generic;

namespace Nodify.Calculator
{
    public class OperationInfoViewModel
    {
        public record struct InputData(string Title, string PropertyName, ConnectorType Type, bool AllowMultipleInputs, bool IsValueOnlyInput = false, Func<IValueChecker>? ValueChecker = null);
        public string? Title { get; set; }
        public string? PropertyName { get; set; }
        public required List<InputData> Input { get; init; } = new List<InputData>();
        public required ConnectorType OutputType { get; set; }
        public required Type PayloadType { get; set; }
    }
}
