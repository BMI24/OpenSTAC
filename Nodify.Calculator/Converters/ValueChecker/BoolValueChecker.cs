using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Nodify.Calculator.Converters.ValueChecker
{
    internal class BoolValueChecker : ValueCheckerBase<bool>
    {
        public override string ProcessInput(string input)
        {

            if (bool.TryParse(input, out var result))
            {
                ParsedObject = result;
            }

            return ParsedValue.ToString();
        }
    }
}
