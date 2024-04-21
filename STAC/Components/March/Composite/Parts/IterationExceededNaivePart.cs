using STAC.Components.March.Composite.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.March.Composite.Parts
{
    internal class IterationExceededNaivePart : ComponentBase, IIterationExceededPart
    {
        public CompositeMarchComponent? CompositeMarch { get; set; }

        public override string Generate()
        {
            ArgumentNullException.ThrowIfNull(CompositeMarch);

            return $"""
                dist = far;
                {CompositeMarch.ReturnStatement}
                """;
        }
    }
}
