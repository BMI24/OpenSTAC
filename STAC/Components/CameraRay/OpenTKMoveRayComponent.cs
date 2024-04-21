using LearnOpenTK.Common;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.CameraRay
{
    public class OpenTKMoveRayComponent : ComponentBase, ICameraRayComponent
    {
        public Vector3 CameraPosition { private get; init; }
        public float Yaw { private get; init; } = 0.0f;
        public float Pitch { private get; init; } = 0.0f;
        public string ResultFieldName { get; set; } = "";
        public GlobalIdentifier ViewMatrixUniformName { get; } = "viewToWorld";
        public GlobalIdentifier RayDirectionFunctionName { get; } = "rayDirection";

        public override void Initialize()
        {
            base.Initialize();
            ArgumentNullException.ThrowIfNull(GenerationManager);
            if (GenerationManager.Window is not Window window)
                throw new Exception($"OpenTKMovementComponent only works with {nameof(window)}");

            GenerationManager.AddAdditionalGlobalDefinition($$"""
                uniform mat4 {{ViewMatrixUniformName}};

                vec3 {{RayDirectionFunctionName}}(float fov, vec2 fragPos, vec3 cam) {
                    float fov_scale = tan(radians(fov) / 2.0);
                    float aspect_ratio = {{GenerationManager.SCREEN_SIZE_NAME}}.x / {{GenerationManager.SCREEN_SIZE_NAME}}.y;
                    float x = (-1.0 + 2.0 * fragPos.x) * aspect_ratio * fov_scale;
                    float y = ( 1.0 - 2.0 * fragPos.y) * fov_scale;
                    return normalize(cam - (vec4(x, y, 1, 1) * {{ViewMatrixUniformName}}).xyz);
                }
                """);

            window.CursorState = CursorState.Grabbed;
            window.ViewMatrixName = ViewMatrixUniformName;
            window.Camera ??= new Camera(CameraPosition, 1)
            {
                Yaw = Yaw,
                Pitch = Pitch
            };
        }

        public override string Generate()
        {
            ArgumentException.ThrowIfNullOrEmpty(ResultFieldName);
            ArgumentException.ThrowIfNullOrEmpty(ViewMatrixUniformName);

            return $$"""
                vec3 cam = (vec4(0, 0, 0, 1) * {{ViewMatrixUniformName}}).xyz;
                dir = rayDirection({{GenerationManager.FIELD_OF_FIEW}}, texCoordV, cam);

                """;
        }
    }
}
