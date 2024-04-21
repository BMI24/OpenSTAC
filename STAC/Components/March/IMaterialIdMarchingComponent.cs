using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.March
{
    internal interface IMaterialIdMarchingComponent : IMarchingComponent
    {
        [NotCustomizable]
        bool IsMaterialIdMarching { get; set; }
        GlobalIdentifier MaterialIdOutputName { get; }
    }
}
