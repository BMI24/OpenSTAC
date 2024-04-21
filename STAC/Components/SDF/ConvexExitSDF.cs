using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.SDF
{
    internal class ConvexExitSDF : ComponentBase, ISDFComponent, IMarchBeginNotifySDF, ISeperateExactSDF
    {
        public GlobalIdentifier SDFName { get; } = "wrappedConvexSDF";
        public GlobalIdentifier MarchBeginName { get; } = "onMarchBegin";
        public GlobalIdentifier MarchEndName { get; } = "onMarchEnd";
        public GlobalIdentifier MinDistName { get; } = "minDistToConvexSDF";
        public required ISDFComponent WrappedSDFComponent { get; set; }
        public GlobalIdentifier ExactSDFName => WrappedSDFComponent.SDFName;

        public override string Generate()
        {
            WrappedSDFComponent? w = WrappedSDFComponent as WrappedSDFComponent;

            return $$"""
                {{WrappedSDFComponent}}

                float {{MinDistName}} = 0;

                void {{MarchBeginName}}()
                {
                    {{MinDistName}} = {{GenerationManager.INFINITY}};
                }

                void {{MarchEndName}}()
                {
                    {{MinDistName}} = -1;
                }

                float {{SDFName}}(vec3 p)
                {
                    if ({{MinDistName}} < 0)
                        return {{WrappedSDFComponent.SDFName}}(p);

                    if ({{MinDistName}} == 0)
                        return {{GenerationManager.INFINITY}};

                    float dist = {{WrappedSDFComponent.SDFName}}(p);
                    if ({{(w != null ? w.DistToOuter : "dist")}} > {{MinDistName}})
                    {
                        {{MinDistName}} = 0;
                        return {{GenerationManager.INFINITY}};
                    }

                    {{MinDistName}} = {{(w != null ? $"max({w.DistToOuter}, {GenerationManager.EPSILON_NAME})": "dist")}};
                    return dist;
                }
            """;
        }
    }
}
