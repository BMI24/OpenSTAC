using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.SDF
{
    internal class MaterialSceneSDFComponent : ComponentBase, IMaterialSDFComponent, IMarchBeginNotifySDF, ISeperateExactSDF
    {
        public GlobalIdentifier MaterialIdOutputName { get; } = "mat_id";
        public GlobalIdentifier SDFName { get; } = "scene_sdf";
        public List<ISDFComponent> ComponentsForInitialization => SDFComponents.Select(x => x.Component).ToList();
        public List<(ISDFComponent Component, int MaterialId)> SDFComponents { get; init; } = new();
        public GlobalIdentifier MarchBeginName { get; } = "onMarchBegin";
        public GlobalIdentifier MarchEndName { get; } = "onMarchEnd";
        public GlobalIdentifier MinByXName { get; } = "min_by_x";
        public GlobalIdentifier ExactSDFName => IsApproximate() ? _exactSDFName : SDFName;

        private bool IsApproximate()
        {
            return ComponentsForInitialization.Any(c => c is ISeperateExactSDF);
        }

        private GlobalIdentifier _exactSDFName = "exact_scene_sdf";

        public override string Generate()
        {
            ArgumentException.ThrowIfNullOrEmpty(SDFName);

            string exactSDF = IsApproximate() ? $$"""
                float {{_exactSDFName}}(vec3 p) {
                    vec2 dist = vec2({{GenerationManager.FAR_PLANE_NAME}} * 2, 0.0);
                
                    {{string.Join(Environment.NewLine, SDFComponents.Select(c =>
                        $$""""
                        dist = {{MinByXName}}(dist, vec2({{(c.Component is ISeperateExactSDF eSDF ? eSDF.ExactSDFName : c.Component.SDFName )}}(p), {{c.MaterialId}}));
                        """"))}}
                    
                    {{MaterialIdOutputName}} = int(dist.y);
                    return dist.x;
                }
                """ : "";

            return $$"""
                {{string.Join(Environment.NewLine, SDFComponents.Select(c => c.Component.Generate()))}}

                vec2 {{MinByXName}}(vec2 a, vec2 b)
                {
                    // taken from https://stackoverflow.com/a/45653286
                    return mix( a, b, step( b.x, a.x ) );
                }

                int {{MaterialIdOutputName}} = 0;
                
                {{exactSDF}}

                float {{SDFName}}(vec3 p) {
                    vec2 dist = vec2({{GenerationManager.FAR_PLANE_NAME}} * 2, 0.0);

                    {{string.Join(Environment.NewLine, SDFComponents.Select(c =>
                        $$""""
                        dist = {{MinByXName}}(dist, vec2({{c.Component.SDFName}}(p), {{c.MaterialId}}));
                        """"))}}
                    
                    {{MaterialIdOutputName}} = int(dist.y);
                    return dist.x;
                }

                void {{MarchBeginName}}()
                {
                    {{SDFComponents.OfType<IMarchBeginNotifySDF>().Select(sdf => sdf.MarchBeginName + "();").SJoin()}}
                }

                void {{MarchEndName}}()
                {
                    {{SDFComponents.OfType<IMarchBeginNotifySDF>().Select(sdf => sdf.MarchEndName + "();").SJoin()}}
                }
                """;
        }
    }
}
