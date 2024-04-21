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
    public class InfiniteCylinderSDFComponent : ComponentBase, ISDFComponent
    {
        public GlobalIdentifier SDFName { get; } = "infiniteCylinderSDF";
        public Vector2 Center { get; set; }
        public float Radius { get; set; }

        public override string Generate()
        {
            ArgumentException.ThrowIfNullOrEmpty(SDFName);

            return $$"""
                float {{SDFName}}(vec3 p){
                    return length(p.xz - vec2{{Center}})-{{Radius}};
                }
                """;
        }
    }
}
