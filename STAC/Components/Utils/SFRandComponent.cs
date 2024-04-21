using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.Utils
{
    internal class SFRandComponent : ComponentBase
    {
        public GlobalIdentifier SFRandFN { get; } = "sfrand";
        public override string Generate()
        {
            return $$"""
                // https://iquilezles.org/articles/sfrand
                float {{SFRandFN}}( inout int mirand )
                {
                    mirand = mirand*0x343fd+0x269ec3;
                    float res = uintBitsToFloat((uint(mirand)>>9)|0x40000000u ); 
                    return( res-3.0 );
                }
                """;
        }
    }
}
