using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.Utils
{
    internal class IQFBMComponent : ComponentBase
    {
        public GlobalIdentifier FBMName { get; } = "fbm";
        public required IQNoiseComponent NoiseComponent { get; set; }
        public override string Generate()
        {
            return $$"""
                // taken from https://www.shadertoy.com/view/NtlSDs
                // https://iquilezles.org/articles/fbm
                float fbm( vec3 p )
                {
                #if 0
                    // original code
                    return 0.5000*{{NoiseComponent.NoiseFN}}( p*1.0 ) + 
                           0.2500*{{NoiseComponent.NoiseFN}}( p*2.0 ) + 
                           0.1250*{{NoiseComponent.NoiseFN}}( p*4.0 ) +
                           0.0625*{{NoiseComponent.NoiseFN}}( p*8.0 );
                #else
                    // equivalent code, but compiles MUCH faster
                    float f = 0.0;
                    float s = 0.5;
                    for( int i=0; i<4; i++ )
                    {
                        f += s*{{NoiseComponent.NoiseFN}}( p );
                        s *= 0.5;
                        p *= 2.0;
                    }
                    return f;
                #endif
                }
                """;
        }
    }
}
