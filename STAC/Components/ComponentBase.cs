using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components
{
    public abstract class ComponentBase : IComponent
    {
        public GenerationManager? GenerationManager { get; set; }

        public abstract string Generate();

        public virtual void Initialize() { }

        public override string? ToString()
        {
            return Generate();
        }
    }
}
