using OpenTK.Graphics.ES20;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components
{
    public interface IComponent
    {
        IEnumerable<IComponent> Components
        {
            get
            {
                foreach (var prop in GetType().GetProperties())
                {
                    if (typeof(IComponent).IsAssignableFrom(prop.PropertyType))
                    {
                        var propValue = prop.GetValue(this);
                        yield return (IComponent)propValue!;
                    }

                    if (prop.PropertyType.IsGenericType 
                        && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>) 
                        && prop.PropertyType.GenericTypeArguments.Length == 1 
                        && typeof(IComponent).IsAssignableFrom(prop.PropertyType.GenericTypeArguments[0]))
                    {
                        var propValue = prop.GetValue(this);
                        if (propValue != null)
                        {
                            foreach (var item in (System.Collections.IList)propValue)
                            {
                                yield return (IComponent)item;
                            }
                        }
                    }
                }
            }
        }
        GenerationManager? GenerationManager { get; set; }

        void Initialize();
        string Generate();
    }
}
