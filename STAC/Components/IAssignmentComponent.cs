using STAC.Components.Color;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components
{
    public interface IAssignmentComponent : IComponent
    {
        [NotCustomizable]
        string ResultFieldName { get; set; }

        IAssignmentComponent WithFieldName(string resultFieldName)
        {
            ResultFieldName = resultFieldName;
            return this;
        }
    }
}
