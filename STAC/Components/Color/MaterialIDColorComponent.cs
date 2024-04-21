using STAC.Components.March;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.Color
{
    internal class MaterialIDColorComponent : ComponentBase, IColorComponent
    {
        public required IMaterialIdMarchingComponent MaterialMarchingComponent { get; set; }
        public string ResultFieldName { get; set; } = "";
        public required List<(int MaterialID, IColorComponent ColorComponent)> MaterialIDToComponent { get; set; }
        public List<IColorComponent> ComponentsForInitialization => MaterialIDToComponent.Select(x => x.ColorComponent).ToList();

        public override void Initialize()
        {
            base.Initialize();
            MaterialMarchingComponent.IsMaterialIdMarching = true;
        }
        public override string Generate()
        {
            ArgumentException.ThrowIfNullOrEmpty(ResultFieldName);

            return $$$""""
                {
                    switch ({{{MaterialMarchingComponent.MaterialIdOutputName}}}) {
                    {{{
                        MaterialIDToComponent.Select(e => $$"""
                        case {{e.MaterialID}}:
                            {{e.ColorComponent.WithFieldName(ResultFieldName)}}
                            break;
                        """).SJoin()
                    }}}
                    default:
                        // float to color mapping from Inigo Quilez ( https://www.shadertoy.com/view/Xds3zN ) under MIT
                        {{{ResultFieldName}}}.xyz = 0.2 + 0.2*sin( {{{MaterialMarchingComponent.MaterialIdOutputName}}}*2.0 + vec3(0.0,1.0,2.0) );
                        break;
                    }
                }
                """";
        }
    }
}
