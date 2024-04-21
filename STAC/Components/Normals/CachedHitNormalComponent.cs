using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.Normals
{
    internal class CachedHitNormalComponent : ComponentBase, INormalComponent
    {
        public GlobalIdentifier NormalFN { get; } = "cachedHitNormal";
        public GlobalIdentifier CachedNormalName { get; } = "cachedNormal";
        public required INormalComponent NormalComponent { get; set; }

        public override void Initialize()
        {
            base.Initialize();

            GenerationManager!.AddPostHitCall($"{CachedNormalName} = {NormalComponent.NormalFN}(cam + dir * dist);");
        }
        public override string Generate()
        {
            ArgumentException.ThrowIfNullOrEmpty(NormalFN);

            return $$"""
                {{NormalComponent}}
                vec3 {{CachedNormalName}};
                vec3 {{NormalFN}}(vec3 p) {
                    return {{CachedNormalName}};
                }
                """;
        }
    }
}
