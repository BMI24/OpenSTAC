using STAC.Components.March.Composite;
using STAC.Components.March.Composite.Parts;
using STAC.Components.SDF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.March
{
    public class EnhancedKeinertMarchComponent : CompositeMarchComponent
    {
        public EnhancedKeinertMarchComponent()
        {
            MarchFN = "keinertMarch";
        }
        public bool UseRelaxation { get; set; } = true;
        public bool UseIntersectionPointSelection { get; set; } = true;
        public override void Initialize()
        {
            Parts.Clear();
            Parts.Add(UseRelaxation ? new StepLengthKeinertPart() : new StepLengthNaivePart());
            Parts.Add(UseIntersectionPointSelection ? new IterationExceededKeinertPart() : new IterationExceededNaivePart());
            base.Initialize();
        }
    }
}
