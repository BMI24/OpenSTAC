using STAC.Components.SDF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.March
{
    public class MarchErrorComponent : ComponentBase, IFlexibleMarchingComponent
    {
        public MarchingOutputType OutputType
        { 
            get => MarchComponent1.OutputType;
            set => MarchComponent1.OutputType = MarchComponent2.OutputType = value;
        }

        public GlobalIdentifier MarchFN { get; } = "marchError";
        public required IFlexibleMarchingComponent MarchComponent1 { get; set; }
        public required IFlexibleMarchingComponent MarchComponent2 { get; set; }

        public override string Generate()
        {
            return $$"""
                {{MarchComponent1}}
                {{MarchComponent2}}

                float {{MarchFN}}(vec3 origin, vec3 dir, float near, float far){
                    float val1 = {{MarchComponent1.MarchFN}}(origin, dir, near, far);
                    float val2 = {{MarchComponent2.MarchFN}}(origin, dir, near, far);
                    return val2 - val1;
                }
                """;
        }
    }
}
