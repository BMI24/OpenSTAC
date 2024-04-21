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
    public class BoxSDFComponent : ComponentBase, ISDFComponent
    {
        public GlobalIdentifier SDFName { get; } = "boxSDF";
        public Vector3 Center { get; set; }
        public Vector3 Scale { get; set; }
        public float Radius { get; set; }

        public override string Generate()
        {
            ArgumentException.ThrowIfNullOrEmpty(SDFName);

            return $$"""
                float {{SDFName}}(vec3 p)
                {
                    vec3 q = abs(vec3{{Center}} - p) - vec3{{Scale}};
                    return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0) - {{Radius}};
                }
                """;
        }
    }
}
