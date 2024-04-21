using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.SDF
{
    public interface ISDFComponent : IComponent
    {
        GlobalIdentifier SDFName { get; }
    }
}
