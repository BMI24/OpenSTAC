using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodify.Calculator.Converters.ValueChecker
{
    internal class IntValueChecker : ValueCheckerBase<int>
    {
        public override string ProcessInput(string input)
        {
            if (int.TryParse(input, out var result))
            {
                ParsedObject = ParsedValue = result;
            }

            return ParsedValue.ToString();
        }
    }
}
