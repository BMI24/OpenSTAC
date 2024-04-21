using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL4;
using STAC.Components.Normals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StbImageSharp;
using System.IO;

namespace STAC.Components.Color
{
    internal class TriplanarMappingColorComponent : ComponentBase, IColorComponent, IPostCompilationHandler
    {
        public string ResultFieldName { get; set; } = "";
        public GlobalIdentifier TextureName { get; } = "triplanarTexture";
        public GlobalIdentifier ColorResultName { get; } = "triplanarColor";
        public required INormalComponent NormalComponent { get; set; }
        ImageResult? Image { get; set; }


        public override void Initialize()
        {
            base.Initialize();
            ArgumentNullException.ThrowIfNull(GenerationManager);


            ReadTexture("MiscData/wall.jpg");

            GenerationManager.AddAdditionalGlobalDefinition($"""
                vec3 {ColorResultName};
                uniform sampler2D {TextureName};
                """);
            GenerationManager.AddPostHitCall($$"""
                {
                    // taken from https://github.com/JonasFolletete/glsl-triplanar-mapping under MIT

                    vec3 hitPos = cam + dist * dir;
                    vec3 normalBlend = abs({{NormalComponent.NormalFN}}(hitPos));
                    normalBlend = normalize(max(normalBlend, 0.00001));
                    normalBlend /= vec3(normalBlend.x + normalBlend.y + normalBlend.z);
                	vec3 xColor = texture2D({{TextureName}}, hitPos.yz).rgb;
                	vec3 yColor = texture2D({{TextureName}}, hitPos.xz).rgb;
                	vec3 zColor = texture2D({{TextureName}}, hitPos.xy).rgb;

                    {{ColorResultName}} = xColor * normalBlend.x + yColor * normalBlend.y + zColor * normalBlend.z;
                }
                """);
        }

        private void ReadTexture(string path)
        {
            StbImage.stbi_set_flip_vertically_on_load(1);
            Image = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);
        }

        public override string Generate()
        {
            return $"{ResultFieldName} = {ColorResultName};";
        }

        public void OnPostCompilation(Shader shader)
        {
            ArgumentNullException.ThrowIfNull(Image);

            GL.GenTextures(1, out int textureName);
            GL.ActiveTexture(TextureUnit.Texture0 + textureName);
            GL.BindTexture(TextureTarget.Texture2D, textureName);

            GL.TextureParameter(textureName, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TextureParameter(textureName, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TextureParameter(textureName, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TextureParameter(textureName, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Image.Width, Image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, Image.Data);

            shader.SetInt(TextureName, textureName);
        }
    }
}
