using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STAC.Components.March.Composite.Interfaces;

namespace STAC.Components.March.Composite.Parts
{
    internal class StepLengthNaivePart : ComponentBase, IStepLengthPart
    {
        public CompositeMarchComponent? CompositeMarch { get; set; }

        public override string Generate()
        {
            return "stepLength = signedRadius;";
        }
    }
}
