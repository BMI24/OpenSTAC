using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.SDF
{
    public class SceneSDFComponent : ComponentBase, ISDFComponent, IMarchBeginNotifySDF, ISeperateExactSDF
    {
        public GlobalIdentifier SDFName { get; } = "sceneSDF";
        public GlobalIdentifier MarchBeginName { get; } = "onMarchBegin";
        public List<ISDFComponent> SDFComponents { get; init; } = new List<ISDFComponent>();
        public GlobalIdentifier MarchEndName { get; } = "onMarchEnd";
        public GlobalIdentifier ExactSDFName => IsApproximate() ? _exactSDFName : SDFName;

        private bool IsApproximate()
        {
            return SDFComponents.Any(c => c is ISeperateExactSDF);
        }

        private GlobalIdentifier _exactSDFName = "exact_scene_sdf";

        public override string Generate()
        {
            ArgumentException.ThrowIfNullOrEmpty(SDFName);

            string exactSDF = IsApproximate() ? $$"""
                float {{_exactSDFName}}(vec3 p) {
                    float dist = {{GenerationManager.FAR_PLANE_NAME}} * 2;
                
                    {{string.Join(Environment.NewLine, SDFComponents.Select(c =>
                        $$""""
                        dist = min(dist, {{(c is ISeperateExactSDF eSDF ? eSDF.ExactSDFName : c.SDFName)}}(p));
                        """"))}}
                    return dist;
                }
                """ : "";

            return $$"""
                
                {{string.Join(Environment.NewLine, SDFComponents.Select(c => c.Generate()))}}

                {{exactSDF}}

                float {{SDFName}}(vec3 p) {
                    float dist = {{GenerationManager.FAR_PLANE_NAME}} * 2;

                    {{string.Join(Environment.NewLine, SDFComponents.Select(c =>
                        $$""""
                        dist = min(dist, {{c.SDFName}}(p));
                        """"))}}
                    return dist;
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
