using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using STAC.Components;
using STAC.Components.CameraRay;
using STAC.Components.March;
using STAC.Components.March.Composite;
using STAC.Components.March.Composite.Parts;
using STAC.Components.SDF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static STAC.Components.Color.IterationCountColorComponent;
using static STAC.PipelineProvider;

namespace STAC
{
    internal class BenchmarkWindow : GameWindow
    {
        private readonly float[] _vertices = new float[9];

        private int _vertexBufferObject;
        private int _vertexArrayObject;

        private Shader? Shader;
        public MainComponent? MainComponent;
        public SynchronizationContext? MainThreadSyncContext;
        public int FRAMES_PER_BENCHMARK = 10000;
        public string BenchmarkDirectoryPath;
        public string ShaderDirectoryPath;
        public string ScreenshotDirectoryPath;
        const bool ShouldTakeScreenshot = true;

        public BenchmarkWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            string benchmarkId = $"benchmark-{DateTime.Now.ToString("s").Replace(":", ".")}";
            BenchmarkDirectoryPath = Path.Combine("benchmark", benchmarkId);
            Directory.CreateDirectory(BenchmarkDirectoryPath);
            ShaderDirectoryPath = Path.Combine(BenchmarkDirectoryPath, "shaders");
            Directory.CreateDirectory(ShaderDirectoryPath);
            ScreenshotDirectoryPath = Path.Combine(BenchmarkDirectoryPath, "img");
            Directory.CreateDirectory(ScreenshotDirectoryPath);


            var benchmarkLogPath = Path.Combine(BenchmarkDirectoryPath, "log.txt");

            BenchmarkFileWriter = File.CreateText(benchmarkLogPath);
            BenchmarkFileWriter.AutoFlush = true;
            BenchmarkFileWriter.WriteLine("scene_name,init_ms,total_ms,average_ms");
        }

        StreamWriter BenchmarkFileWriter;

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            if (MainComponent == null)
            {
                List<(IPipelineSetup Setup, string Name, (Vector3 camPos, Vector3 camLookAt)? cam)> rawShaders = new()
                {
                    // (MCDemoMaterialsShaded(), "DemoMaterialsShaded", null),
                    // (MCDemoShaded(), "DemoShaded", null),
                    // (MCDemoFlat(), "DemoFlat", null),
                    // (MCSpheresFlat(), "GridPhong", null),
                    // (MCSpheresShaded(), "GridPhong", null),
                    // (MCIterationsSpheresKeinert(), "GridPhong", null),
                };


                // SDF = SDFTwoSpheres,
                // Camera = CameraDynamic,
                // ColorCtor = ColorPhongShaded,
                // MarchCtor = MarchNaive,
                // NormalCtor = NormalNaive
                float[] relaxationParamters = new[] { 1.1f, 1.2f, 1.3f, 1.4f, 1.5f, 1.6f, 1.7f, 1.8f, 1.9f, 2.0f, 2.1f, 2.2f, 2.3f, 2.4f, 2.5f, 2.6f, 2.7f, 2.8f, 2.9f };
                List<MarchTypes> marchTypes = new(){
                        new(MarchNaive, "Naive")
                };
                //foreach (var relaxation in relaxationParamters)
                //{
                //    marchTypes.Add(new(MarchComposite(new StepLengthKeinertPart { RelaxationFallbackBehavior = RelaxationFallbackBehavior.StopRelexation, RelaxationParameter = relaxation }, new IterationExceededNaivePart()), $"KeinertStopRelaxation{relaxation}_"));
                //    marchTypes.Add(new(MarchComposite(new StepLengthKeinertPart { RelaxationFallbackBehavior = RelaxationFallbackBehavior.KeepRelexation, RelaxationParameter = relaxation }, new IterationExceededNaivePart()), $"KeinertKeepRelaxation{relaxation}_"));
                //    marchTypes.Add(new(MarchComposite(new StepLengthBalintPart { RelaxationFallbackBehavior = RelaxationFallbackBehavior.StopRelexation, RelaxationParameter = relaxation }, new IterationExceededNaivePart()), $"BalintStopRelaxation{relaxation}_"));
                //    marchTypes.Add(new(MarchComposite(new StepLengthBalintPart { RelaxationFallbackBehavior = RelaxationFallbackBehavior.KeepRelexation, RelaxationParameter = relaxation }, new IterationExceededNaivePart()), $"BalintKeepRelaxation{relaxation}_"));
                //}
                //, (new(20f, 10f, 30f), new(20f, 11f, 30f))
                foreach (var mc in new MainComponentFlat[] {
                    new(MCFlat(SDFTwoSpheres), "SDFTwoSpheres", null),
                    new(MCFlat(SDFTwoSpheresEarlyExit), "SDFTwoSpheresEarlyExit", null),
                    new(MCFlat(ToSceneSDF(SDFDemoComponents)), "SDFDemoComponents", null),
                    new(MCFlat(ToSceneSDF(SDFDemoTeapotWrappedComponents)), "SDFDemoTeapotWrappedComponents", null),
                    new(MCFlat(ToSceneSDF(SDFDemoComplexWrappedComponents)), "SDFDemoComplexWrappedComponents", null),
                    new(MCFlat(ToSceneSDF(SDFDemoAllWrappedComponents)), "SDFDemoAllWrappedComponents", null),
                })
                {

                    foreach (var m in marchTypes)
                    {

                        var c = mc.Ctor();
                        c.MarchCtor = m.Ctor;
                        //c.ColorCtor = ColorIteration<IFlexibleMarchingComponent>(ColorMap.Gray);
                        rawShaders.Add((c, mc.Name + m.Name, mc.Cam));
                    }
                }

                foreach (var (Setup, Name, cam) in rawShaders)
                {
                    if (Setup.Camera is not StaticCameraRayComponent)
                        Setup.Camera = new StaticCameraRayComponent
                        {
                            CameraPosition = cam?.camPos ?? new(20f, 10f, 30f),
                            CameraCenter = cam?.camLookAt ?? new(7, 0, -5),
                            CameraUp = new(0, 1, 0)
                        };
                }

                int[] iterations = new[] { 80, 100 };
                ShadersToBenchmark = rawShaders.Select(s => (s.Setup.Generate(), s.Name)).SelectMany(x => iterations, (x, iterations) => (x.Item1, x.Name + iterations, iterations)).ToList();
                UseNextShaderForBenchmark();
            }
            else
            {
                ShadersToBenchmark = new() { (MainComponent, "Unknown", 100) };
                UseNextShaderForBenchmark();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            CursorState = CursorState.Normal;
            MainThreadSyncContext?.Post((state) => Dispose(), null);
        }

        private void OnScreenSizeChanged()
        {
            Shader!.SetVector2(GenerationManager.SCREEN_SIZE_NAME, new Vector2(Size.X, Size.Y));
        }

        int FrameCount = 0;
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            if (FrameCount > FRAMES_PER_BENCHMARK)
            {
                UseNextShaderForBenchmark();
            }


            GL.Clear(ClearBufferMask.ColorBufferBit);
            Shader!.Use();



            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            if (FrameCount == -1)
            {
                TakeScreenshot(Path.Combine(ScreenshotDirectoryPath, ShadersToBenchmark[SelectedShaderIndex].Name + ".bmp"));

                ShaderStopwatch.Restart();
            }
            SwapBuffers();
            FrameCount++;
        }

        public void TakeScreenshot(string path)
        {
            if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
            {
                Bitmap bmp = new(Size.X, Size.Y);
                System.Drawing.Imaging.BitmapData data =
                    bmp.LockBits(new Rectangle(0, 0, Size.X, Size.Y), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                GL.ReadPixels(0, 0, Size.X, Size.Y, PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
                bmp.UnlockBits(data);

                bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
                bmp.Save(path);
            }
            else
                throw new NotImplementedException("Screenshots can only be taken on Windows");
        }


        int SelectedShaderIndex = -1;
        List<(MainComponent Component, string Name, int MaxIterations)> ShadersToBenchmark = new();
        Stopwatch ShaderStopwatch = new();
        TimeSpan InitializationDuration;
        private void UseNextShaderForBenchmark()
        {
            if (ShaderStopwatch.IsRunning)
            {
                var runtime = ShaderStopwatch.Elapsed.TotalMilliseconds;
                BenchmarkFileWriter.WriteLine($"{ShadersToBenchmark[SelectedShaderIndex].Name},{InitializationDuration.TotalMilliseconds},{runtime},{runtime / FRAMES_PER_BENCHMARK}");
            }



            SelectedShaderIndex++;
            if (SelectedShaderIndex == ShadersToBenchmark.Count)
            {
                Close();
                return;
            }


            FrameCount = ShouldTakeScreenshot ? -1 : 0;

            ShaderStopwatch.Restart();
            var mainComponent = ShadersToBenchmark[SelectedShaderIndex].Component;
            Console.WriteLine("========================================================");
            Console.WriteLine(ShadersToBenchmark[SelectedShaderIndex].Name);
            Console.WriteLine("========================================================");

            GenerationManager manager = new() { Window = this, MainComponent = mainComponent };
            manager.MAX_MARCHING_STEPS = ShadersToBenchmark[SelectedShaderIndex].MaxIterations.ToString();
            Shader?.Delete();
            string fracSource = manager.GetFragShaderSource();
            File.WriteAllText(Path.Combine(ShaderDirectoryPath, ShadersToBenchmark[SelectedShaderIndex].Name + ".frag"), fracSource);
            Shader = new(File.ReadAllText("Shaders/shader.vert"), fracSource);
            manager.OnPostCompilation(Shader);
            Shader.Use();
            InitializationDuration = ShaderStopwatch.Elapsed;
            ShaderStopwatch.Restart();


            OnScreenSizeChanged();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
            OnScreenSizeChanged();
        }
    }

    internal record struct MainComponentFlat(Func<PipelineSetup<IFullMarchingComponent>> Ctor, string Name, (Vector3 camPos, Vector3 camLookAt)? Cam);

    internal record struct MarchTypes(Func<ISDFComponent, CompositeMarchComponent> Ctor, string Name, int IterationCount = 100);
}
