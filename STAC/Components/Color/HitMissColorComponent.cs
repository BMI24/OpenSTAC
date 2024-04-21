using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.Color
{
    public class HitMissColorComponent : ComponentBase, IColorComponent
    {
        public string ResultFieldName { get; set; } = "";
        public required IColorComponent HitColorComponent { get; set; }
        public required IColorComponent MissColorComponent { get; set; }


        public override string Generate()
        {
            ArgumentNullException.ThrowIfNull(HitColorComponent);
            ArgumentNullException.ThrowIfNull(MissColorComponent);
            ArgumentException.ThrowIfNullOrEmpty(ResultFieldName);

            
            return $$"""
                if (dist > {{GenerationManager.FAR_PLANE_NAME}} - {{GenerationManager.EPSILON_NAME}}){
                    {{MissColorComponent.WithFieldName(ResultFieldName)}}
                    return;
                }
                
                {{HitColorComponent.WithFieldName(ResultFieldName)}}
                """;
        }
    }
}
