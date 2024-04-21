using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.SDF
{
    public interface IColorSDFComponent : ISDFComponent
    {
        public bool IsColorSDF { get; set; }
        public GlobalIdentifier ColorOutputName { get; }
    }
}
