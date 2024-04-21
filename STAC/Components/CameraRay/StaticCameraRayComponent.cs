using LearnOpenTK.Common;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.CameraRay
{
    public class StaticCameraRayComponent : ComponentBase, ICameraRayComponent
    {
        public string ResultFieldName { get; set; } = "";
        public GlobalIdentifier ViewMatrixFN { get; } = "viewMatrix";
        public GlobalIdentifier RayDirectionFN { get; } = "rayDirection";
        public Vector3 CameraPosition { get; set; }
        public Vector3 CameraCenter { get; set; }
        public Vector3 CameraUp { get; set; }

        public override void Initialize()
        {
            base.Initialize();

            ArgumentException.ThrowIfNullOrEmpty(ViewMatrixFN);
            ArgumentException.ThrowIfNullOrEmpty(RayDirectionFN);
            ArgumentNullException.ThrowIfNull(GenerationManager);

            GenerationManager.AddAdditionalGlobalDefinition($$"""
                vec3 {{RayDirectionFN}}(float fov, vec2 fragCoord) {
                    vec2 xy = fragCoord - {{GenerationManager.SCREEN_SIZE_NAME}} / 2.0;
                    float z = {{GenerationManager.SCREEN_SIZE_NAME}}.y / tan(radians(fov) / 2.0);
                    return normalize(vec3(xy, -z));
                }
                mat4 {{ViewMatrixFN}}(vec3 cam, vec3 center, vec3 up) {
                    // Based on gluLookAt man page
                    vec3 f = normalize(center - cam);
                    vec3 s = normalize(cross(f, up));
                    vec3 u = cross(s, f);
                    return mat4(
                        vec4(s, 0.0),
                        vec4(u, 0.0),
                        vec4(-f, 0.0),
                        vec4(0.0, 0.0, 0.0, 1)
                    );
                }
                """);
        }
        public override string Generate()
        {
            ArgumentException.ThrowIfNullOrEmpty(ResultFieldName);

            return $$"""
                dir = {{RayDirectionFN}}({{GenerationManager.FIELD_OF_FIEW}}, texCoordV * {{GenerationManager.SCREEN_SIZE_NAME}});
                vec3 cam = vec3{{CameraPosition}};
                
                mat4 viewToWorld = {{ViewMatrixFN}}(cam, vec3{{CameraCenter}}, vec3{{CameraUp}});
                
                dir = (viewToWorld * vec4(dir, 0.0)).xyz;
                """;
        }
    }
}
