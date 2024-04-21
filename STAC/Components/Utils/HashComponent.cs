using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.Utils
{
    internal class HashComponent : ComponentBase
    {
        public GlobalIdentifier IHashFN { get; } = "ihash";
        public GlobalIdentifier HashFN { get; } = "hash";
        public override string Generate()
        {
            return $$"""
                // hash by Hugo Elias
                uint {{IHashFN}}( uint n )
                {
                	n = (n << 13) ^ n;
                    n = (n*(n*n*15731u+789221u)+1376312589u)&0x7fffffffu;
                    return n;
                }

                // hash by Hugo Elias
                float {{HashFN}}( int n )
                {
                	n = (n << 13) ^ n;
                    n = (n*(n*n*15731+789221)+1376312589)&0x7fffffff;
                    return 1.0 - float(n)*(1.0/1073741824.0);
                }
                """;
        }
    }
}
