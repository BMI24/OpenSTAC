using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.SDF
{
    internal interface IMarchBeginNotifySDF
    {
        public GlobalIdentifier MarchBeginName { get; }
        public GlobalIdentifier MarchEndName { get; }
    }
}
