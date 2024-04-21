using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodify.Calculator.Converters.ValueChecker
{
    internal class FloatValueChecker : ValueCheckerBase<float>
    {
        public override string ProcessInput(string input)
        {
            if (float.TryParse(input, out float result))
            {
                ParsedObject = result;
            }

            return ParsedValue.ToString();
        }
    }
}
