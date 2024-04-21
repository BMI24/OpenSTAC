using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.SDF
{
    /// <summary>
    /// licensed from https://iquilezles.org/articles/distfunctions/ under MIT
    /// </summary>
    public class TorusSDFComponent : ComponentBase, ISDFComponent
    {
        public GlobalIdentifier SDFName { get; } = "torusSDF";
        public Vector3 Center { get; set; }
        public float Radius { get; set; }
        public float Thickness { get; set; }

        public override string Generate()
        {
            ArgumentException.ThrowIfNullOrEmpty(SDFName);

            return $$"""
                float {{SDFName}}(vec3 p)
                {
                    p = vec3{{Center}} - p;
                    vec2 q = vec2(length(p.xz)-{{Radius}},p.y);
                    return length(q)-{{Thickness}};
                }
                """;
        }
    }
}
