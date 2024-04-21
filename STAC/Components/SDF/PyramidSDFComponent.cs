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
    /// including a fix proposed by ljs_harbin at https://www.shadertoy.com/view/Ws3SDl
    /// </summary>
    public class PyramidSDFComponent : ComponentBase, ISDFComponent
    {
        public GlobalIdentifier SDFName { get; } = "pyramidSDF";
        public Vector3 Center { get; set; }
        public float Height { get; set; }

        public override string Generate()
        {
            ArgumentException.ThrowIfNullOrEmpty(SDFName);

            return $$"""
                float {{SDFName}}(vec3 p){
                    p = vec3{{Center}} - p;
                    if (p.y <= 0.0)
                        return length(max(abs(p)-vec3(0.5,0.0,0.5),0.0));

                    float m2 = {{Height}}*{{Height}} + 0.25;

                    p.xz = abs(p.xz);
                    p.xz = (p.z>p.x) ? p.zx : p.xz;
                    p.xz -= 0.5;

                    vec3 q = vec3( p.z, {{Height}}*p.y - 0.5*p.x, {{Height}}*p.x + 0.5*p.y);

                    float s = max(-q.x,0.0);
                    float t = clamp( (q.y-0.5*p.z)/(m2+0.25), 0.0, 1.0 );

                    float a = m2*(q.x+s)*(q.x+s) + q.y*q.y;
                    float b = m2*(q.x+0.5*t)*(q.x+0.5*t) + (q.y-m2*t)*(q.y-m2*t);

                    float d2 = min(q.y,-q.x*m2-q.y*0.5) > 0.0 ? 0.0 : min(a,b);

                    return sqrt( (d2+q.z*q.z)/m2 ) * sign(max(q.z,-p.y));
                }
                """;
        }
    }
}
