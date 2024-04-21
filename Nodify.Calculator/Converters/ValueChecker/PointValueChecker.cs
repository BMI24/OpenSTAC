using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Nodify.Calculator.Converters.ValueChecker
{
    internal class PointValueChecker : ValueCheckerBase<Point>
    {
        public override string ProcessInput(string input)
        {
            try
            {
                var result = Point.Parse(input);
                ParsedObject = ParsedValue = result;
            }
            catch { }

            return ParsedValue.ToString();
        }
    }
}
