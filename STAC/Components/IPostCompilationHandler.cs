using LearnOpenTK.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components
{
    internal interface IPostCompilationHandler
    {
        void OnPostCompilation(Shader shader);
    }
}
