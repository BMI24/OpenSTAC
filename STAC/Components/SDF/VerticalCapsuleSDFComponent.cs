using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.SDF
{
    public class VerticalCapsuleSDFComponent : ComponentBase, ISDFComponent
    {
        public GlobalIdentifier SDFName { get; } = "verticalCapsuleSDF";
        public Vector3 Center { get; set; }
        public float Radius { get; set; }
        public float Height { get; set; }

        public override string Generate()
        {
            ArgumentException.ThrowIfNullOrEmpty(SDFName);

            return $$"""
                float {{SDFName}}(vec3 p){
                    p = vec3{{Center}} - p;
                    p.y -= clamp( p.y, 0.0, {{Height}} * 1.0 );
                    return length( p ) - {{Radius}};
                }
                """;
        }
    }
}
