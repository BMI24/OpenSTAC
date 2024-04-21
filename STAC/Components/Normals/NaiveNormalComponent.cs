using STAC.Components.SDF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STAC.GenerationManager;

namespace STAC.Components.Normals
{
    public class NaiveNormalComponent : ComponentBase, INormalComponent
    {
        public GlobalIdentifier NormalFN { get; } = "naiveNormal";
        public required ISDFComponent SDFComponent { get; set; }

        public override string Generate()
        {
            ArgumentException.ThrowIfNullOrEmpty(NormalFN);

            return $$"""
                vec3 {{NormalFN}}(vec3 p) {
                    return normalize(vec3(
                        {{SDFComponent.ExactSDFName()}}(vec3(p.x + {{EPSILON_NAME}}, p.y, p.z)) - {{SDFComponent.ExactSDFName()}}(vec3(p.x - {{EPSILON_NAME}}, p.y, p.z)),
                        {{SDFComponent.ExactSDFName()}}(vec3(p.x, p.y + {{EPSILON_NAME}}, p.z)) - {{SDFComponent.ExactSDFName()}}(vec3(p.x, p.y - {{EPSILON_NAME}}, p.z)),
                        {{SDFComponent.ExactSDFName()}}(vec3(p.x, p.y, p.z  + {{EPSILON_NAME}})) - {{SDFComponent.ExactSDFName()}}(vec3(p.x, p.y, p.z - {{EPSILON_NAME}}))
                    ));
                }
                """;
        }
    }
}
