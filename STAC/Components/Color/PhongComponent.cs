using OpenTK.Mathematics;
using STAC.Components.March;
using STAC.Components.Normals;
using STAC.Components.SDF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.Color
{
    public class PhongComponent : ComponentBase, IColorComponent
    {
        public enum LightVisibilityFactor
        {
            Off,
            Binary,
            Continous
        }
        public record struct Light(Vector3 Position, Vector3 Intensity);
        public required IColorComponent AmbientColorComponent { get; set; }
        public required IColorComponent DiffuseColorComponent { get; set; }
        public required IColorComponent SpecularColorComponent { get; set; }
        public required INormalComponent NormalComponent { get; set; }
        public IMarchingComponent? LightVisibilityMarchingComponent { get; set; }
        public string ResultFieldName { get; set; } = "";
        public GlobalIdentifier PhongContributionToLightName { get; } = "phongContributionToLight";
        public GlobalIdentifier PhongIlluminationName { get; } = "phongIllumination";
        public GlobalIdentifier NormalName { get; } = "normal";
        public GlobalIdentifier SoftshadowName { get; } = "softshadow";
        public Vector3 AmbientLight { get; set; }
        public List<Light> Lights { get; init; } = new();
        public float Shininess { get; set; }
        public LightVisibilityFactor LightVisibilityFactorUsed { get; set; }
        public required ISDFComponent SDFComponent { get; set; }
        public float LightSize = 0.05f;

        public override void Initialize()
        {
            // currently still heavily inspired from https://www.shadertoy.com/view/lt33z7

            ArgumentNullException.ThrowIfNull(GenerationManager);
            ArgumentException.ThrowIfNullOrEmpty(PhongContributionToLightName);
            ArgumentException.ThrowIfNullOrEmpty(PhongIlluminationName);
            if (LightVisibilityFactorUsed == LightVisibilityFactor.Binary)
            {
                ArgumentNullException.ThrowIfNull(LightVisibilityMarchingComponent);
            }

            string lightVisibilityApplication = LightVisibilityFactorUsed switch
            {
                LightVisibilityFactor.Off => "",
                LightVisibilityFactor.Binary => $$"""
                    float distanceToLight = length(lightPos - p);
                    vec3 directionToLight = normalize(lightPos - p);
                    if ({{LightVisibilityMarchingComponent!.MarchFN}}(p + (lightPos - p) * 0.01, directionToLight, 0, distanceToLight * 1.05) * 0.98 < distanceToLight)
                    {
                        lightIntensity = vec3(0);
                    }
                    """,
                LightVisibilityFactor.Continous => $$"""
                    float distanceToLight = length(lightPos - p);
                    vec3 directionToLight = normalize(lightPos - p);
                    lightIntensity *= softshadow(p, directionToLight, 0.01, distanceToLight, {{LightSize}});
                    """,
                _ => throw new NotImplementedException() 
            };

            GenerationManager.AddAdditionalGlobalDefinition($$"""
                {{(LightVisibilityFactorUsed == LightVisibilityFactor.Continous ? $$"""
                    float softshadow(vec3 origin, in vec3 dir, float nearclip, float farclip, float w)
                    {
                        // from https://www.shadertoy.com/view/WdyXRD under MIT
                        float res = 1.0;
                        float t = nearclip;
                        for( int i=0; i<200 && t<farclip; i++ )
                        {
                            float h = {{SDFComponent.ExactSDFName()}}(origin + t*dir);
                            res = min( res, h/(w*t) );
                            t += clamp(h, 0.005, 0.50);
                            if( res<-1.0 || t>farclip ) break;
                        }
                        res = max(res,-1.0);
                        return 0.25*(1.0+res)*(1.0+res)*(2.0-res);
                    }
                    """ : "")}};

                vec3 {{PhongContributionToLightName}}(vec3 diffuse_col, vec3 specular_col, float shininess, vec3 p, vec3 cam, vec3 lightPos, vec3 lightIntensity) {
                    vec3 normal = {{NormalComponent.NormalFN}}(p);
                    vec3 light_dir = normalize(lightPos - p);
                    vec3 eye_direction = normalize(cam - p);
                    vec3 reflection = normalize(reflect(-light_dir, normal));

                    float dotLN = dot(light_dir, normal);
                    float dotRV = dot(reflection, eye_direction);

                    if (dotLN < 0.0) {
                        // Light not visible from this point on the surface
                        return vec3(0.0, 0.0, 0.0);
                    }
                    
                    {{lightVisibilityApplication}}

                    if (dotRV < 0.0) {
                        // Light reflection in opposite direction as viewer, apply only diffuse
                        // component
                        return lightIntensity * (diffuse_col * dotLN);
                    }
                    return lightIntensity * (diffuse_col * dotLN + specular_col * pow(dotRV, shininess));
                }

                vec3 {{PhongIlluminationName}}(vec3 ambient_col, vec3 diffuse_col, vec3 specular_col, float shininess, vec3 p, vec3 cam) {
                    const vec3 ambientLight = vec3{{AmbientLight}};
                    vec3 color = ambientLight * ambient_col;

                    {{string.Join(Environment.NewLine, Lights.Select(l =>$$""""
                        color += {{PhongContributionToLightName}}(diffuse_col, specular_col, shininess, p, cam, vec3{{l.Position}}, vec3{{l.Intensity}});
                        """"))}}

                    return color;
                }
                """);
        }

        public override string Generate()
        {
            ArgumentException.ThrowIfNullOrEmpty(ResultFieldName);
            ArgumentNullException.ThrowIfNull(AmbientColorComponent);
            ArgumentNullException.ThrowIfNull(DiffuseColorComponent);
            ArgumentNullException.ThrowIfNull(SpecularColorComponent);

            return $$"""
                vec3 ambient_col, diffuse_col, specular_col;
                {{AmbientColorComponent.WithFieldName("ambient_col")}}
                {{DiffuseColorComponent.WithFieldName("diffuse_col")}}
                {{SpecularColorComponent.WithFieldName("specular_col")}}
                float shininess = {{Shininess}};

                vec3 p = cam + dist * dir;
                {{ResultFieldName}} = {{PhongIlluminationName}}(ambient_col.xyz, diffuse_col.xyz, specular_col.xyz, shininess, p, cam);
                """;
        }
    }
}
