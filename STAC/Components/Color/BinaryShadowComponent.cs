using OpenTK.Mathematics;
using STAC.Components.March;
using STAC.Components.Normals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.Color
{
    public class BinaryShadowComponent : ComponentBase, IColorComponent
    {
        public record struct Light(Vector3 Position, Vector3 Intensity);
        public required INormalComponent NormalComponent { get; set; }
        public IMarchingComponent? LightVisibilityMarchingComponent { get; set; }
        public string ResultFieldName { get; set; } = "";
        public GlobalIdentifier IlluminationName { get; } = "binaryIllumination";
        public GlobalIdentifier ContributionToLightName { get; } = "contribToLight";
        public List<Light> Lights { get; init; } = new();
        public required IColorComponent ColorComponent { get; set; }

        public override void Initialize()
        {
            ArgumentNullException.ThrowIfNull(GenerationManager);
            ArgumentException.ThrowIfNullOrEmpty(IlluminationName);

            GenerationManager.AddAdditionalGlobalDefinition($$"""
                vec3 {{ContributionToLightName}}(vec3 diffuse_col, vec3 p, vec3 cam, vec3 lightPos, vec3 lightIntensity) {
                    float distanceToLight = length(lightPos - p);
                    vec3 directionToLight = normalize(lightPos - p);
                    if ({{LightVisibilityMarchingComponent!.MarchFN}}(p, directionToLight, 0.1, distanceToLight * 1.1) < distanceToLight)
                    {
                        lightIntensity = vec3(0);
                    }
                    else
                    {
                        lightIntensity *= diffuse_col;
                    }
                    return lightIntensity;
                }

                vec3 {{IlluminationName}}(vec3 diffuse_col, vec3 p, vec3 cam) {
                    vec3 color = vec3(0);

                    {{string.Join(Environment.NewLine, Lights.Select(l =>$$""""
                        color += {{ContributionToLightName}}(diffuse_col, p, cam, vec3{{l.Position}}, vec3{{l.Intensity}});
                        """"))}}

                    return color;
                }
                """);
        }

        public override string Generate()
        {
            ArgumentException.ThrowIfNullOrEmpty(ResultFieldName);
            ArgumentNullException.ThrowIfNull(ColorComponent);

            return $$"""
                vec3 color;
                {{ColorComponent.WithFieldName("color")}}

                vec3 p = cam + dist * dir;
                {{ResultFieldName}} = {{IlluminationName}}(color.xyz, p, cam);
                """;
        }
    }
}
