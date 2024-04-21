using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using SEB;
using STAC.Components.SDF;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace STAC.BoundingShapeGeneration
{
    internal class BoundingGenerator : IDisposable
    {
        public class PointSet : SEB.PointSet
        {
            public void Add(Vector3d p) => Points.Add(p);
            public List<Vector3d> Points = new();
            public int Size => Points.Count;
            public int Dimension { get; } = 3;

            public double Coord(int i, int j)
            {
                return j switch
                {
                    0 => Points[i].X,
                    1 => Points[i].Y,
                    2 => Points[i].Z,
                    _ => throw new Exception()
                };
            }
        }
        public static GameWindow? Window { get; set; }
        private static BoundingGenerator? ValidInstance = null;

        public int Width { get; } = 512;
        public int Height { get; } = 512;
        public int LocalGroupWidth { get; } = 16;
        public int LocalGroupHeight { get; } = 16;
        public PointSet? Points;
        private bool disposedValue;
        public static int TexIndex { get; private set; }
        public static int ThresholdIndex { get; private set; }

        public static void Initialize(GameWindow window)
        {
            Window = window;

            TexIndex = GL.GenTexture();
            ThresholdIndex = GL.GenTexture();

            GenerateDestTex(ThresholdIndex, 1, 1);
        }
        public static WrappedSDFComponent CreateSphereWrapper(ISDFComponent sdf, Vector2? expectedRange = null, int precision = 2)
        {

            Vector2 range = expectedRange ?? new(-100, 100);
            using BoundingGenerator instance = new(sdf) { Points = new() };
            Vector2 minSearchSpacePlane = new(range.X, range.X);
            Vector2 maxSearchSpacePlane = new(range.Y, range.Y);
            Vector2 searchSpaceAlongDirection = new(range.X, range.Y);

            (float minX, float maxX) = instance.FindNonemptyRange(new(1, 0, 0), new(0, 1, 0), minSearchSpacePlane, maxSearchSpacePlane, searchSpaceAlongDirection, 0.001f);
            (float minY, float maxY) = instance.FindNonemptyRange(new(0, 1, 0), new(0, 0, 1), minSearchSpacePlane, maxSearchSpacePlane, searchSpaceAlongDirection, 0.001f);
            (float minZ, float maxZ) = instance.FindNonemptyRange(new(0, 0, 1), new(0, 1, 0), minSearchSpacePlane, maxSearchSpacePlane, searchSpaceAlongDirection, 0.001f);

            var miniball = new Miniball(instance.Points);

            SphereSDFComponent wrapper = new()
            {
                Center = new((float)miniball.Center[0], (float)miniball.Center[1], (float)miniball.Center[2]),
                Radius = (float)miniball.Radius
            };

            return new WrappedSDFComponent { InnerSDF = sdf, OuterSDF = wrapper };
        }
        public static void DumpPoints(PointSet points, string filename)
        {
            using FileStream stream = new FileStream(filename, FileMode.Create);
            using StreamWriter writer = new StreamWriter(stream);
            writer.Write($"""
                ply
                format ascii 1.0
                element vertex {points.Size}
                property float x
                property float y
                property float z
                end_header
                """);
            foreach (var point in points.Points)
            {
                writer.WriteLine($"{point.X} {point.Y} {point.Z}");
            }
        }

        public static WrappedSDFComponent CreateAABBWrapper(ISDFComponent sdf, Vector2? expectedRange = null)
        {
            Vector2 range = expectedRange ?? new(-100, 100);
            using BoundingGenerator instance = new(sdf);
            Vector2 minSearchSpacePlane = new(range.X, range.X);
            Vector2 maxSearchSpacePlane = new(range.Y, range.Y);
            Vector2 searchSpaceAlongDirection = new(range.X, range.Y);

            (float minX, float maxX) = instance.FindNonemptyRange(new(1, 0, 0), new(0, 1, 0), minSearchSpacePlane, maxSearchSpacePlane, searchSpaceAlongDirection, 0.001f);
            (float minY, float maxY) = instance.FindNonemptyRange(new(0, 1, 0), new(0, 0, 1), minSearchSpacePlane, maxSearchSpacePlane, searchSpaceAlongDirection, 0.001f);
            (float minZ, float maxZ) = instance.FindNonemptyRange(new(0, 0, 1), new(0, 1, 0), minSearchSpacePlane, maxSearchSpacePlane, searchSpaceAlongDirection, 0.001f);
            Vector3 bounds = new(maxX - minX, maxY - minY, maxZ - minZ);
            bounds /= 2;
            Vector3 center = new(minX + bounds.X, minY + bounds.Y, minZ + bounds.Z);

            BoxSDFComponent wrapper = new() { Center = center, Scale = bounds };
            return new WrappedSDFComponent { InnerSDF = sdf, OuterSDF = wrapper };
        }
        public static void Demo()
        {
            CreateSphereWrapper(new TeapotSDFComponent());
            return;
            BoundingGenerator instance = new(new TeapotSDFComponent());
            Vector2 minSearchSpacePlane = new(-100, -100);
            Vector2 maxSearchSpacePlane = new(100, 100);
            Vector2 searchSpaceAlongDirection = new(-100, 100);
            var x = instance.FindNonemptyRange(new(1, 0, 0), new(0, 1, 0), minSearchSpacePlane, maxSearchSpacePlane, searchSpaceAlongDirection, 0.001f);
            var y = instance.FindNonemptyRange(new(0, 1, 0), new(0, 0, 1), minSearchSpacePlane, maxSearchSpacePlane, searchSpaceAlongDirection, 0.001f);
            var z = instance.FindNonemptyRange(new(0, 0, 1), new(0, 1, 0), minSearchSpacePlane, maxSearchSpacePlane, searchSpaceAlongDirection, 0.001f);
        }

        public BoundingGenerator(ISDFComponent sdfComponent)
        {
            ArgumentNullException.ThrowIfNull(Window, "Call Initialize(Window window) first!");

            SDFComponent = sdfComponent;
            GenerateDestTex(TexIndex, Width, Height);
            ComputeProgramId = SetupComputeProgram();

            GL.UseProgram(ComputeProgramId);

            GL.Uniform1(GL.GetUniformLocation(ComputeProgramId, "destTex"), TexIndex);
            GL.Uniform1(GL.GetUniformLocation(ComputeProgramId, "thresholdVal"), ThresholdIndex);

            ValidInstance = this;
        }

        
        
        public ISDFComponent SDFComponent { get; set; }
        public int ComputeProgramId { get; private set; }
        
        private (float Begin, float End) FindNonemptyRange(Vector3 searchPlaneX, Vector3 searchPlaneY, Vector2 minSearchSpacePlane, Vector2 maxSearchSpacePlane, Vector2 searchSpaceAlongDirection, float maxUncertainty)
        {
            CheckValidity();
            // first: narrow Y search space roughly (just one step)

            (float beginY, float endY, float uncY) = FindNonemptyRange(searchPlaneY, searchPlaneX, minSearchSpacePlane.Yx, maxSearchSpacePlane.Yx, searchSpaceAlongDirection);

            minSearchSpacePlane.Y = beginY - uncY;
            maxSearchSpacePlane.Y = endY + uncY;
            (float begin, float end, float initialUncertainty) = FindNonemptyRange(searchPlaneX, searchPlaneY, minSearchSpacePlane, maxSearchSpacePlane, searchSpaceAlongDirection);
            float beginUnc = initialUncertainty, endUnc = initialUncertainty;

            float lastUnc = 0;
            int ySplits = 1;
            while (beginUnc > maxUncertainty)
            {
                (begin, _, beginUnc) = FindNonemptyRange(searchPlaneX, searchPlaneY,
                    new Vector2(begin - beginUnc, minSearchSpacePlane.Y),
                    new Vector2(begin + beginUnc, maxSearchSpacePlane.Y),
                    searchSpaceAlongDirection, ySplits);

                if (lastUnc >= beginUnc)
                    ySplits++;

                lastUnc = beginUnc;
            }

            lastUnc = 0;
            ySplits = 1;

            while (endUnc > maxUncertainty)
            {
                (_, end, endUnc) = FindNonemptyRange(searchPlaneX, searchPlaneY,
                    new Vector2(end - endUnc, minSearchSpacePlane.Y),
                    new Vector2(end + endUnc, maxSearchSpacePlane.Y),
                    searchSpaceAlongDirection, ySplits);

                if (lastUnc >= endUnc)
                    ySplits++;

                lastUnc = endUnc;
            }

            return (begin - beginUnc, end + endUnc);
        }

        private void CheckValidity()
        {
            if (this != ValidInstance)
                throw new Exception($"A new {nameof(BoundingGenerator)} was created, making this one invalid.");
        }

        private (float Begin, float End, float UncertaintyRange) FindNonemptyRange(Vector3 searchPlaneX, Vector3 searchPlaneY, Vector2 minSearchSpacePlane, Vector2 maxSearchSpacePlane, Vector2 searchSpaceAlongDirection, int ySplits)
        {
            CheckValidity();

            float yBegin = minSearchSpacePlane.Y;
            float ySplitLength = (maxSearchSpacePlane.Y - minSearchSpacePlane.Y) / ySplits;
            float begin = float.PositiveInfinity;
            float end = float.NegativeInfinity;
            float unc = 0;
            for (int i = 0; i < ySplits; i++)
            {
                (float localBegin, float localEnd, unc) = FindNonemptyRange(searchPlaneX, searchPlaneY, 
                    new(minSearchSpacePlane.X, yBegin + i * ySplitLength),
                    new(maxSearchSpacePlane.X, yBegin + (i + 1) * ySplitLength), 
                    searchSpaceAlongDirection);
                begin = Math.Min(localBegin, begin);
                end = Math.Max(localEnd, end);
            }

            if (begin < minSearchSpacePlane.X || begin > end || end > maxSearchSpacePlane.X)
            {
                // this only happens with approximate SDFs! as a fallback, just pretend its just outside of our search plane
                // may actually still be too small -> could result in a bouding box thats too small
                return (minSearchSpacePlane.X - unc, maxSearchSpacePlane.X + unc, unc);
            }
            return (begin, end, unc);
        }
        private (float Begin, float End, float UncertaintyRange) FindNonemptyRange(Vector3 searchPlaneX, Vector3 searchPlaneY, Vector2 minSearchSpacePlane, Vector2 maxSearchSpacePlane, Vector2 searchSpaceAlongDirection)
        {
            CheckValidity();

            GL.UseProgram(ComputeProgramId);

            GL.Uniform3(GL.GetUniformLocation(ComputeProgramId, "SearchPlaneX"), searchPlaneX);
            GL.Uniform3(GL.GetUniformLocation(ComputeProgramId, "SearchPlaneY"), searchPlaneY);
            GL.Uniform2(GL.GetUniformLocation(ComputeProgramId, "MinSearchSpacePlane"), minSearchSpacePlane);
            GL.Uniform2(GL.GetUniformLocation(ComputeProgramId, "MaxSearchSpacePlane"), maxSearchSpacePlane);
            GL.Uniform2(GL.GetUniformLocation(ComputeProgramId, "SearchSpaceAlongDirection"), searchSpaceAlongDirection);

            GL.DispatchCompute((int)Math.Ceiling(Width / (float)LocalGroupWidth), (int)Math.Ceiling(Height / (float)LocalGroupHeight), 1);
            var distField = ReadTexture(TexIndex, Width, Height);


            int begin, end;
            if (Points == null)
            {
                begin = FindBegin(distField);
                end = FindEnd(distField);
            }
            else
            {
                (begin, end) = ScanResult(distField);
            }

            var normalizedBegin = begin / ((float)Width - 1);
            var normalizedEnd = end / ((float)Width - 1);
            

            var thresh = ReadValue(ThresholdIndex);

            return (Vector2.Lerp(minSearchSpacePlane, maxSearchSpacePlane, normalizedBegin).X, Vector2.Lerp(minSearchSpacePlane, maxSearchSpacePlane, normalizedEnd).X, thresh);
        }

        private (int begin, int end) ScanResult(Vector4[,] distField)
        {
            int begin = int.MaxValue, end = int.MinValue;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var val = distField[x, y].X;
                    if (val <= 0)
                    {
                        begin = Math.Min(begin, x);
                        end = Math.Max(end, x);
                        if (val < 0)
                        {
                            Points!.Add(distField[x, y].Yzw);
                        }
                    }
                }
            }
            return (begin, end);
        }

        private int FindBegin(Vector4[,] distField)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var val = distField[x, y].X;
                    if (val <= 0)
                    {
                        return x;
                    }
                }
            }

            return int.MaxValue;
        }

        private int FindEnd(Vector4[,] distField)
        {
            for (int x = Width - 1; x >= 0; x--)
            {
                for (int y = 0; y < Height; y++)
                {
                    var val = distField[x, y].X;
                    if (val <= 0)
                    {
                        return x;
                    }
                }
            }
            return int.MinValue;
        }

        private static void GenerateDestTex(int texHandle, int width, int height)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + texHandle);
            GL.BindTexture(TextureTarget.Texture2D, texHandle);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba32f, width, height, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);

            // to enable writing to the texture:
            GL.BindImageTexture(texHandle, texHandle, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba32f);
        }

        private int SetupComputeProgram()
        {
            int progHandle = GL.CreateProgram();
            int cs = GL.CreateShader(ShaderType.ComputeShader);

            string source = GetBoundingComputeShaderCode();

            GL.ShaderSource(cs, source);
            GL.CompileShader(cs);
            GL.GetShader(cs, ShaderParameter.CompileStatus, out int rvalue);
            if (rvalue != (int)All.True)
            {
                throw new Exception(GL.GetShaderInfoLog(cs));
            }
            GL.AttachShader(progHandle, cs);

            GL.LinkProgram(progHandle);
            GL.GetProgram(progHandle, GetProgramParameterName.LinkStatus, out rvalue);
            if (rvalue != (int)All.True)
            {
                throw new Exception(GL.GetProgramInfoLog(progHandle));
            }

            GL.DetachShader(cs, progHandle);
            GL.DeleteShader(cs);

            return progHandle;
        }

        private string GetBoundingComputeShaderCode()
        {
            ArgumentNullException.ThrowIfNull(Window, "Call Initialize(Window window) first!");

            GenerationManager generationManager = new()
            {
                Window = Window
            };

            generationManager.InitializeComponents(SDFComponent);

            return $$"""
                #version 430
                uniform writeonly image2D destTex; 
                uniform writeonly image2D thresholdVal;
                
                layout (local_size_x = {{LocalGroupWidth}}, local_size_y = {{LocalGroupHeight}}) in; 
                
                vec3 hitLocation;

                {{SDFComponent}}

                float minDistToRay(vec3 start, vec3 dir, float far, float thresh)
                {
                    float initialFar = far;
                    float minDist = 2 * far;
                    vec3 pos = start;
                    float prevRadius = far / 1000;
                    for (int i = 0; i < 100000; i++)
                    {
                        float radius = {{SDFComponent.SDFName}}(pos);
                        

                        if (radius < 0.000001)
                        {
                            hitLocation = pos;
                            return -(initialFar - far);
                        }
                
                
                        if (far < 0)
                            return minDist;

                        float stepWidth = radius;
                        if (radius >= thresh)
                        {
                            // if radius < thresh, radius*radius-thresh*thresh < 0 -> sqrt of negative value
                            stepWidth = sqrt(radius*radius-thresh*thresh);
                        }

                        pos += dir * stepWidth;
                        far -= stepWidth;

                        // we are overestimating the minDist here
                        // but: if actual distance < threshold, then radius < thresh too (prove somewhere?)

                        minDist = min(radius, minDist);
                        prevRadius = radius;

                    }
                    return 1.0/0.0;
                }

                uniform vec2 MinSearchSpacePlane;
                uniform vec2 MaxSearchSpacePlane;
                uniform vec3 SearchPlaneX;
                uniform vec3 SearchPlaneY;
                uniform vec2 SearchSpaceAlongDirection;

                vec3 getOrigin(ivec2 id)
                {
                    vec2 normalizedPos = id / vec2({{Width}} - 1, {{Height}} - 1);
                    vec2 minOrigin = MinSearchSpacePlane;
                    vec2 maxOrigin = MaxSearchSpacePlane;
                    vec2 originOnPlane = mix(minOrigin, maxOrigin, normalizedPos);
                    vec3 origin = originOnPlane.x * SearchPlaneX + originOnPlane.y * SearchPlaneY;

                    return origin;
                }
            
                void main()
                {
                    ivec2 storePos = ivec2(gl_GlobalInvocationID.xy);
                    vec3 origin = getOrigin(storePos);
                    
                    vec3 d1 = origin - getOrigin(storePos + ivec2(1,0));
                    vec3 d2 = origin - getOrigin(storePos + ivec2(0,1));
                    float diff1squared = dot(d1, d1);
                    float diff2squared = dot(d2, d2);
                    float threshold = sqrt(diff1squared + diff2squared) / 2;
                    
                    vec3 dir = cross(SearchPlaneX, SearchPlaneY);
                    origin += dir * SearchSpaceAlongDirection[0];

                    float minDist = minDistToRay(origin, dir, SearchSpaceAlongDirection[1] - SearchSpaceAlongDirection[0], threshold);

                    
                    float result = 0;
                    if (minDist > 0)
                        result = minDist < threshold ? 0 : minDist;
                    else
                        result = minDist;
                        
                    
                    // store at storePos.yx to make scanning it faster (see FindBegin and FindEnd)
                    // (if stored at storePos, the methods would get cache misses too often because y and x would have to be inverted)
                    imageStore(destTex, storePos.yx, vec4(result, hitLocation));
                    if (storePos.x == 0 && storePos.y == 0)
                        imageStore(thresholdVal, storePos, vec4(threshold));
                }
                """;
        }
        public float ReadValue(int index) => ReadTexture(index, 1, 1)[0, 0].X;
        public Vector4[,] ReadTexture(int index, int width, int height)
        {
            Vector4[,] texture = new Vector4[width, height];

            // i dont think this works, but does not matter currently
            GL.ActiveTexture(TextureUnit.Texture0 + index);
            GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.Float, texture);

            return texture;
        }

        [SupportedOSPlatform("windows6.1")]
        private void SaveTexture(float[,] texture)
        {
            Bitmap newBitmap = new Bitmap(Width, Height);

            for (int j = 0; j < Height; j++)
            {
                for (int i = 0; i < Width; i++)
                {
                    Color newColor = Color.FromArgb((int)(texture[i, j] / 300 * 255), (int)(texture[i, j] / 300 * 255), (int)(texture[i, j] / 300 * 255));

                    newBitmap.SetPixel(i, j, newColor);
                }
            }

            newBitmap.Save("out.png");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                Points = null;
                GL.DeleteProgram(ComputeProgramId);
                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~BoundingGenerator()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
