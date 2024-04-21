using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.Utils
{
    internal class IQNoiseComponent : ComponentBase
    {
        public GlobalIdentifier NoiseFN { get; } = "noise";
        public required HashComponent HashComponent { get; set; }
        public override string Generate()
        {
            return $$"""
                // from Inigo Quilez https://www.shadertoy.com/view/NtlSDs
                float {{NoiseFN}}( vec3 p )
                {
                	ivec3 ip = ivec3(floor(p));
                     vec3 fp = fract(p);

                    vec3 w = fp*fp*(3.0-2.0*fp);

                    int n = ip.x + ip.y*57 + 113*ip.z;

                	return mix(mix(mix( {{HashComponent.HashFN}}(n+(0+57*0+113*0)),
                                        {{HashComponent.HashFN}}(n+(1+57*0+113*0)),w.x),
                                   mix( {{HashComponent.HashFN}}(n+(0+57*1+113*0)),
                                        {{HashComponent.HashFN}}(n+(1+57*1+113*0)),w.x),w.y),
                               mix(mix( {{HashComponent.HashFN}}(n+(0+57*0+113*1)),
                                        {{HashComponent.HashFN}}(n+(1+57*0+113*1)),w.x),
                                   mix( {{HashComponent.HashFN}}(n+(0+57*1+113*1)),
                                        {{HashComponent.HashFN}}(n+(1+57*1+113*1)),w.x),w.y),w.z);
                }
                """;
        }
    }
}
