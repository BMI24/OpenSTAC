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
    public class NaiveMarchComponent : CompositeMarchComponent
    {
        public NaiveMarchComponent()
        {
            MarchFN = "naiveMarch";
        }
        public override void Initialize()
        {
            Parts.Clear();
            Parts.Add(new StepLengthNaivePart());
            Parts.Add(new IterationExceededNaivePart());
            base.Initialize();
        }
    }
}
