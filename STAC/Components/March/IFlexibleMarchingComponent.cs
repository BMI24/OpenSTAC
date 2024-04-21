using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.March
{
    public interface IFlexibleMarchingComponent : IMarchingComponent
    {
        [NotCustomizable]
        MarchingOutputType OutputType { get; set; }
    }
}
