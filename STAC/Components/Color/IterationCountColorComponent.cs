using STAC.Components.March;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.Color
{
    public class IterationCountColorComponent : ComponentBase, IColorComponent
    {
        public enum ColorMap
        {
            Gray,
            Viridis,
            Jet,
            Seismic
        }
        /// <summary>
        /// Color map functions taken from https://observablehq.com/@flimsyhat/webgl-color-maps
        /// </summary>
        static Dictionary<ColorMap, string> ColorMapDefinitions = new()
        {
            { ColorMap.Gray, "{\n return vec3(t);\n }\n" },
            { ColorMap.Viridis, """
                {
                const vec3 c0 = vec3(0.2777273272234177, 0.005407344544966578, 0.3340998053353061);
                    const vec3 c1 = vec3(0.1050930431085774, 1.404613529898575, 1.384590162594685);
                    const vec3 c2 = vec3(-0.3308618287255563, 0.214847559468213, 0.09509516302823659);
                    const vec3 c3 = vec3(-4.634230498983486, -5.799100973351585, -19.33244095627987);
                    const vec3 c4 = vec3(6.228269936347081, 14.17993336680509, 56.69055260068105);
                    const vec3 c5 = vec3(4.776384997670288, -13.74514537774601, -65.35303263337234);
                    const vec3 c6 = vec3(-5.435455855934631, 4.645852612178535, 26.3124352495832);

                    return c0+t*(c1+t*(c2+t*(c3+t*(c4+t*(c5+t*c6)))));
                }
                """},
            { ColorMap.Jet, """
                {
                    // adapted from https://github.com/kbinani/colormap-shaders/tree/master
                    // TODO: add license used: MIT

                    float r = t < 0.7 ? 4.0 * t - 1.5 : -4.0 * t + 4.5;
                    float g = t < 0.5 ? 4.0 * t - 0.5 : -4.0 * t + 3.5;
                    float b = t < 0.3 ? 4.0 * t + 0.5 : -4.0 * t + 2.5;

                    r = clamp(r, 0.0, 1.0);
                    g = clamp(g, 0.0, 1.0);
                    b = clamp(b, 0.0, 1.0);
                    return vec3(r, g, b);
                }
                """ },
            { ColorMap.Seismic, """
                {
                    // adapted from https://github.com/kbinani/colormap-shaders/tree/master
                    // TODO: add license used: MIT

                    float r;
                    if (t < 0.0) {
                        r = 3.0 / 255.0;
                    } else if (t < 0.238) {
                        r = ((-1810.0 * t + 414.49) * t + 3.87702) / 255.0;
                    } else if (t < 51611.0 / 108060.0) {
                        r = (344441250.0 / 323659.0 * t - 23422005.0 / 92474.0) / 255.0;
                    } else if (t < 25851.0 / 34402.0) {
                        r = 1.0;
                    } else if (t <= 1.0) {
                        r = (-688.04 * t + 772.02) / 255.0;
                    } else {
                        r = 83.0 / 255.0;
                    }
                    float g;
                    if (t < 0.0) {
                        g = 0.0;
                    } else if (t < 0.238) {
                        g = 0.0;
                    } else if (t < 51611.0 / 108060.0) {
                        g = ((-2010.0 * t + 2502.5950459) * t - 481.763180924) / 255.0;
                    } else if (t < 0.739376978894039) {
                        float xx = t - 51611.0 / 108060.0;
                        g = ((-914.74 * xx - 734.72) * xx + 255.) / 255.0;
                    } else {
                        g = 0.0;
                    }
                    float b;
                    if (t < 0.0) {
                        b =  19.0 / 255.0;
                    } else if (t < 0.238) {
                        float xx = t - 0.238;
                        b =  (((1624.6 * xx + 1191.4) * xx + 1180.2) * xx + 255.0) / 255.0;
                    } else if (t < 51611.0 / 108060.0) {
                        b =  1.0;
                    } else if (t < 174.5 / 256.0) {
                        b =  (-951.67322673866 * t + 709.532730938451) / 255.0;
                    } else if (t < 0.745745353439206) {
                        b =  (-705.250074130877 * t + 559.620050530617) / 255.0;
                    } else if (t <= 1.0) {
                        b =  ((-399.29 * t + 655.71) * t - 233.25) / 255.0;
                    } else {
                        b =  23.0 / 255.0;
                    }

                    return vec3(r, g, b);
                }
                """ }
        };
        HashSet<ColorMap> DivergingColorSets = new()
        {
            ColorMap.Jet,
            ColorMap.Seismic
        };
        public ColorMap UsedColorMap { get; set; } = ColorMap.Gray;
        public GlobalIdentifier ColorMapFN { get; } = "colorMap";
        public string ResultFieldName { get; set; } = string.Empty;
        public required IFlexibleMarchingComponent MarchingComponent { get; set; }
        public override void Initialize()
        {
            ArgumentException.ThrowIfNullOrEmpty(ColorMapFN);
            ArgumentNullException.ThrowIfNull(GenerationManager);

            GenerationManager.AddAdditionalGlobalDefinition($$"""
                vec3 {{ColorMapFN}}(float t)
                {{ColorMapDefinitions[UsedColorMap]}}
                """);

            MarchingComponent.OutputType = MarchingOutputType.Iterations;
        }

        public override string Generate()
        {
            ArgumentException.ThrowIfNullOrEmpty(ResultFieldName);

            return $$"""
                {
                    dist = dist/{{GenerationManager!.MAX_MARCHING_STEPS}};
                    {{(DivergingColorSets.Contains(UsedColorMap) ? "dist = dist * 0.5 + 0.5" : "")}};
                    {{ResultFieldName}}.xyz = {{ColorMapFN}}(dist);
                }
                """;
        }
    }
}
