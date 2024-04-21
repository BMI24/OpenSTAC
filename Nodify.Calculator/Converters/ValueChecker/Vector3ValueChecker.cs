using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Nodify.Calculator.Converters.ValueChecker
{
    internal class Vector3ValueChecker : ValueCheckerBase<Vector3>
    {
        public override string ProcessInput(string input)
        {
            try
            {
                input = input.Trim(new[] { '(', ')', '[', ']', '{', '}' });
                var split = input.Split(new[] { ',', ';' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                float val1 = float.Parse(split[0]);
                float val2 = float.Parse(split[1]);
                float val3 = float.Parse(split[2]);

                ParsedObject = ParsedValue = new Vector3(val1, val2, val3);
            }
            catch { }

            return ParsedValue.ToString();
        }
    }
}
