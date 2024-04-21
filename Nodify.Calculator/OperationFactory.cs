using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Nodify.Calculator
{
    public static class OperationFactory
    {
        public static OperationViewModel GetOperation(OperationInfoViewModel info)
        {
            var op = new OperationViewModel
            {
                Title = info.Title,
                Output = new() { Type = info.OutputType },
                PayloadType = info.PayloadType
            };

            var payload = op.PayloadConstructor();
            var payloadType = info.PayloadType;

            foreach (var input in info.Input)
            {
                string value = "";
                var valueChecker = input.ValueChecker?.Invoke();
                if (valueChecker != null)
                {
                    var property = payloadType.GetProperty(input.PropertyName)!;
                    var defaultValue = property.GetValue(payload)!;
                    valueChecker.ParsedObject = defaultValue;
                    value = valueChecker.ProcessInput("");
                }

                var connector = new ConnectorViewModel
                {
                    Title = input.Title,
                    Type = input.Type,
                    MultiInputAllowed = input.AllowMultipleInputs,
                    IsValueOnlyInput = input.IsValueOnlyInput,
                    ValueChecker = valueChecker,
                    Value = value,
                    PropertyName = input.PropertyName,
                };

                op.Input.Add(connector);
            }

            return op;
        }
    }
}
