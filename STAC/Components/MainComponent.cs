using OpenTK.Mathematics;
using STAC.Components.CameraRay;
using STAC.Components.Color;
using STAC.Components.March;
using STAC.Components.Normals;
using STAC.Components.SDF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components
{
    public class MainComponent : ComponentBase
    {
        public required IColorComponent ColorComponent { get; set; }
        public required IMarchingComponent MarchingComponent { get; set; }
        public required INormalComponent NormalComponent { get; set; }
        public required ISDFComponent SDFComponent { get; set; }
        public required ICameraRayComponent CameraRayComponent { get; set; }
        public List<IComponent> LooseComponents { get; set; } = new();

        public override string Generate()
        {
            ArgumentNullException.ThrowIfNull(GenerationManager);


            return $$"""
                #version 450 core
                uniform vec2 {{GenerationManager.SCREEN_SIZE_NAME}};

                const float {{GenerationManager.NEAR_PLANE_NAME}} = 0.0;
                const float {{GenerationManager.FAR_PLANE_NAME}} = 100.0;
                const float {{GenerationManager.EPSILON_NAME}} = 0.0001;

                {{LooseComponents.Select(c => c.Generate()).SJoin()}}

                {{SDFComponent}}

                {{MarchingComponent}}

                {{NormalComponent}}
                
                {{GenerationManager.AdditionalFunctionDefinitions}}

                out vec3 outputColor;
                in vec2 texCoordV;

                void main() {

                    vec3 dir;
                    {{CameraRayComponent.WithFieldName("dir")}}

                    float dist = {{MarchingComponent.MarchFN}}(cam, dir, {{GenerationManager.NEAR_PLANE_NAME}}, {{GenerationManager.FAR_PLANE_NAME}});
                    {{GenerationManager.PostHitCalls}}

                    {{ColorComponent.WithFieldName("outputColor")}}
                }
                """;
        }
    }
}
