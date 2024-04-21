using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.Color
{
    public class UVColorComponent : ComponentBase, IColorComponent
    {
        public string ResultFieldName { get; set; } = string.Empty;

        public override string Generate()
        {
            ArgumentException.ThrowIfNullOrEmpty(ResultFieldName);

            return $"""
                {ResultFieldName} = vec3(texCoordV.x, texCoordV.y, 0);
                """;
        }
    }
}
