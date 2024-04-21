using STAC.Components.March.Composite.Interfaces;
using STAC.Components.SDF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.March.Composite.Parts
{
    internal class IterationExceededKeinertPart : ComponentBase, IIterationExceededPart, IPreIterationPart, IPreDistChangePart
    {
        public CompositeMarchComponent? CompositeMarch { get; set; }

        public string GeneratePreIterationDefinitions()
        {
            return $$"""
                float pixelWidth = 1.0 / {{GenerationManager.SCREEN_SIZE_NAME}}.x;
                float candidateDist = {{GenerationManager.NEAR_PLANE_NAME}};
                float candidatePixelWidth = {{GenerationManager.INFINITY}};

                {{(CompositeMarch!.IsColorMarching ? "vec3 candidateColor;" : "")}}
                {{(CompositeMarch!.IsMaterialIdMarching ? "int candidateMaterialId;" : "")}}
                """;
        }

        public override string Generate()
        {
            ArgumentNullException.ThrowIfNull(CompositeMarch);

            return $$"""
                if (candidatePixelWidth > pixelWidth)
                {
                    dist = far;
                    {{CompositeMarch.ReturnStatement}}
                }
                else
                {
                    dist = candidateDist;
                    {{(CompositeMarch!.IsColorMarching ? $"{CompositeMarch.ColorOutputName} = candidateColor;" : "")}}
                    {{(CompositeMarch!.IsMaterialIdMarching ? $"{CompositeMarch.MaterialIdOutputName} = candidateMaterialId;" : "")}}
                    {{CompositeMarch.ReturnStatement}}
                }
                """;
        }

        public string GeneratePreDistChange()
        {
            return $$"""
                float currPixelWidth = radius / dist;
                if (currPixelWidth < candidatePixelWidth)
                {
                    candidatePixelWidth = currPixelWidth;
                    {{(CompositeMarch!.IsColorMarching ? $"candidateColor = {(CompositeMarch.SDFComponent as IColorSDFComponent)!.ColorOutputName};" : "")}}
                    {{(CompositeMarch!.IsMaterialIdMarching ? $"candidateMaterialId = {(CompositeMarch.SDFComponent as IMaterialSDFComponent)!.MaterialIdOutputName};" : "")}}
                    candidateDist = dist;
                }
                """;
        }
    }
}
