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
    internal class Color4ValueChecker : ValueCheckerBase<Color4>
    {
        public override string ProcessInput(string input)
        {
            try
            {
                var result = (Color)ColorConverter.ConvertFromString(input);
                ParsedObject = new Color4(result.R, result.G, result.B, result.A);
            }
            catch { }

            return "#" + (ParsedValue.A != 1 ? ((int)(ParsedValue.A * 255)).ToString("X2") : "")
                + ((int)(ParsedValue.R * 255)).ToString("X2") 
                + ((int)(ParsedValue.G * 255)).ToString("X2") 
                + ((int)(ParsedValue.B * 255)).ToString("X2");
        }
    }
}
