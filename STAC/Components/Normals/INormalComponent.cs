﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.Normals
{
    public interface INormalComponent : IComponent
    {
        GlobalIdentifier NormalFN { get; }
    }
}
