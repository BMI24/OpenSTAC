using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.Color
{
    public class ConstantColorComponent : ComponentBase, IColorComponent
    {
        public Color4 Color { get; set; }

        public string ResultFieldName { get; set; } = string.Empty;

        public override string Generate()
        {
            ArgumentException.ThrowIfNullOrEmpty(ResultFieldName);

            return $"""
                {ResultFieldName} = vec3({Color.R}, {Color.G}, {Color.B});
                """;
        }
    }
}
