using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.SDF
{
    internal class HorizontalPlaneSDFComponent : ComponentBase, ISDFComponent
    {
        public GlobalIdentifier SDFName { get; } = "planeSDF";
        public float Height { get; set; }
        public override string Generate()
        {
            return $$"""
                float {{SDFName}}(vec3 p)
                {
                    return p.y - {{Height}};
                }
                """;
        }
    }
}
