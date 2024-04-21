using STAC.Components.March.Composite.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.March.Composite.Parts
{
    /// <summary>
    /// Follows "Accelerating Sphere Tracing" by Bálint et al. (2018)
    /// </summary>
    internal class StepLengthBalintPart : ComponentBase, IStepLengthPart, IPreIterationPart, IValidateSamplePart
    {
        public float RelaxationParameter { get; set; } = 1.6f;
        public CompositeMarchComponent? CompositeMarch { get; set; }
        public RelaxationFallbackBehavior RelaxationFallbackBehavior { get; set; } = RelaxationFallbackBehavior.KeepRelexation;

        public string GeneratePreIterationDefinitions()
        {
            if (RelaxationParameter < 1f)
                throw new ArgumentException("Must be > 1", nameof(RelaxationParameter));

            return $"""
                float prevRadius = 0;
                float prevRelaxation = 0;
                float omega = {RelaxationParameter - 1};
                """;
        }

        public override string Generate()
        {
            return $$"""
                prevRelaxation = omega * signedRadius * (stepLength - prevRadius + signedRadius) / (stepLength + prevRadius - signedRadius);
                stepLength = signedRadius + prevRelaxation;

                prevRadius = signedRadius;
                """;
        }

        public string GenerateValidateSample()
        {
            return $$"""
                bool overstepped = omega > 0 && (radius + abs(prevRadius) < abs(stepLength));
                if (overstepped)
                {
                    dist -= prevRelaxation;
                    stepLength -= prevRelaxation;
                    {{(RelaxationFallbackBehavior == RelaxationFallbackBehavior.StopRelexation ? "omega = 0;" : "")}}
                    continue;
                }
                """;
        }
    }
}
