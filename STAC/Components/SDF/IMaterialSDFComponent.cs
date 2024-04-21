using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.SDF
{
    internal interface IMaterialSDFComponent : ISDFComponent
    {
        public GlobalIdentifier MaterialIdOutputName { get; }
    }
}
