using STAC.Components.March;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.Color
{
    internal class MarchProvidedColorComponent : ComponentBase, IColorComponent
    {
        public required IColorMarchingComponent ColorMarchingComponent { get; set; }
        public string ResultFieldName { get; set; } = "";

        public override void Initialize()
        {
            base.Initialize();
            ColorMarchingComponent.IsColorMarching = true;
        }
        public override string Generate()
        {
            ArgumentException.ThrowIfNullOrEmpty(ResultFieldName);

            return $$"""
                {
                    {{ResultFieldName}}.xyz = {{ColorMarchingComponent.ColorOutputName}};
                }
                """;
        }
    }
}
