using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using STAC.Components.March;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components.SDF
{
    internal class GridSDFComponent : ComponentBase, ISDFComponent, IPostCompilationHandler
    {
        public MarchingOutputType OutputType { get; set; }
        public GlobalIdentifier GridTextureN { get; } = "gridTBO";
        public GlobalIdentifier SDFName { get; } = "gridSDF";
        public GlobalIdentifier BoxSDFName { get; } = "boxSDF";
        public Vector3 Center { get; set; }

        Vector3i Dimension;
        float Spacing;
        Vector3 Origin;
        float[,,] SDFData = null!;

        public override void Initialize()
        {
            base.Initialize();
            ReadGridSDFFile("MiscData/icosahedron.sdf");
        }

        void ReadGridSDFFile(string path)
        {
            var lines = File.ReadAllLines(path);
            var dimVals = lines[0].Split(' ').Select(int.Parse).ToArray();
            Dimension = new(dimVals[0], dimVals[1], dimVals[2]);
            var originVals = lines[1].Split(' ').Select(float.Parse).ToArray();
            Origin = new(originVals[0], originVals[1], originVals[2]);
            Spacing = float.Parse(lines[2]);
            SDFData = new float[Dimension.X, Dimension.Y, Dimension.Z];

            int currLine = 3;
            for (int i = 0; i < Dimension[0]; i++)
            {
                for (int j = 0; j < Dimension[1]; j++)
                {
                    for (int k = 0; k < Dimension[2]; k++)
                    {
                        SDFData[i, j, k] = float.Parse(lines[currLine]);
                        currLine++;
                    }
                }
            }
        }

        public override string Generate()
        {
            return $$"""
            float {{BoxSDFName}}(vec3 c, vec3 p, vec3 b)
            {
                // from https://iquilezles.org/articles/distfunctions/ under MIT license
                vec3 d = abs(c - p) - b;
                return min(max(d.x,max(d.y,d.z)),0.0) + length(max(d,0.0));
            }
            uniform sampler3D {{GridTextureN}};
            vec3 SampleOutPos;
            float {{SDFName}}(vec3 pos) {
                vec3 distanceToCenter = pos - vec3{{Center}};
                vec3 samplePos = distanceToCenter / ({{Spacing}}*vec3{{Dimension}}) - 0.5;
                if (max(min(samplePos, vec3(1,1,1)),vec3(0,0,0)) != samplePos)
                {
                    // outside of texture, return distance to the perimeter
                    return {{BoxSDFName}}(vec3{{Center}} + vec3{{Dimension}} * {{Spacing}}, pos, vec3{{Dimension}} * {{Spacing / 2}}) + {{Spacing}};
                }
                SampleOutPos = samplePos;
                return texture({{GridTextureN}}, samplePos).r;
            }
            """;
        }

        public void OnPostCompilation(Shader shader)
        {
            GL.GenTextures(1, out int textureName);
            GL.ActiveTexture(TextureUnit.Texture0 + textureName);
            GL.BindTexture(TextureTarget.Texture3D, textureName);

            GL.TextureParameter(textureName, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TextureParameter(textureName, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            GL.TextureParameter(textureName, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToBorder);
            GL.TextureParameter(textureName, TextureParameterName.TextureBorderColor, new[] {Spacing});
            GL.TextureParameter(textureName, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TextureParameter(textureName, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexImage3D(TextureTarget.Texture3D, 0, PixelInternalFormat.R32f, Dimension[0], Dimension[1], Dimension[2], 0, PixelFormat.Red, PixelType.Float, SDFData);

            shader.SetInt(GridTextureN, textureName);
        }
    }
}
