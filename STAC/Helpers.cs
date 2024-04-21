using STAC.Components.SDF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC
{
    internal static class Helpers
    {
        public static string SJoin(this IEnumerable<string> source)
        {
            return string.Join("", source);
        }

        public static string ExactSDFName(this ISDFComponent component)
        {
            return component is ISeperateExactSDF eSDF ? eSDF.ExactSDFName : component.SDFName;
                
        }
    }
}
