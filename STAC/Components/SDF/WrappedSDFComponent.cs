using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.SDF
{
    internal class WrappedSDFComponent : ComponentBase, ISDFComponent, ISeperateExactSDF
    {
        public GlobalIdentifier SDFName { get; } = "WrappedSDF";
        public float Radius { get; set; } = 1;
        public required ISDFComponent InnerSDF { get; init; }
        public required ISDFComponent OuterSDF { get; init; }
        public GlobalIdentifier DistToOuter { get; } = "DistToOuter";
        public GlobalIdentifier ExactSDFName => InnerSDF.SDFName;

        public override string Generate()
        {
            return $$"""
                {{InnerSDF}}

                {{OuterSDF}}

                float {{DistToOuter}};
                float {{SDFName}}(vec3 p)
                {
                    {{DistToOuter}} = {{OuterSDF.SDFName}}(p);
                    if ({{DistToOuter}} <= {{Radius}} + {{GenerationManager.EPSILON_NAME}})
                        return {{InnerSDF.SDFName}}(p);

                    return {{DistToOuter}};
                }
                """;
        }
    }
}
