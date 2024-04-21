using STAC.Components.Normals;
using STAC.Components.SDF;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.Color
{
    internal class AmbientOcclusionColorComponent : ComponentBase, IColorComponent
    {
        public string ResultFieldName { get; set; } = "";
        public GlobalIdentifier CalcAOName { get; } = "calcAO";
        public required IColorComponent ColorComponent { get; set; }
        public required INormalComponent NormalComponent { get; set; }
        public required ISDFComponent SDFComponent { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            ArgumentNullException.ThrowIfNull(GenerationManager);
            // todo: maybe add a CachedNormal?

            GenerationManager.AddAdditionalGlobalDefinition($$"""
                float {{CalcAOName}}(vec3 pos) {
                    // taken from https://www.shadertoy.com/view/Xds3zN licensed by Inigo Quilez under MIT
                    vec3 nor = {{NormalComponent.NormalFN}}(pos);
                    float occ = 0.0;
                    float sca = 1.0;
                    for( int i=0; i<5; i++ )
                    {
                        float h = 0.01 + 0.12*float(i)/4.0;
                        float d = {{SDFComponent.ExactSDFName()}}( pos + h*nor ).x;
                        occ += (h-d)*sca;
                        sca *= 0.95;
                        if( occ>0.35 ) break;
                    }
                    return clamp( 1.0 - 3.0*occ, 0.0, 1.0 ) * (0.5+0.5*nor.y);
                }
                """);
        }

        public override string Generate()
        {
            ArgumentNullException.ThrowIfNull(ColorComponent);

            return $$"""
                {{ColorComponent.WithFieldName(ResultFieldName)}}

                {{ResultFieldName}} *= {{CalcAOName}}(p);
                """;
        }
    }
}
