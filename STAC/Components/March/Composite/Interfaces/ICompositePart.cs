using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.March.Composite.Interfaces
{
    public interface ICompositePart : IComponent
    {
        CompositeMarchComponent? CompositeMarch { get; set; }
    }
}
