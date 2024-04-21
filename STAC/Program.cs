using LearnOpenTK.Common;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using STAC.BoundingShapeGeneration;
using STAC.Components;
using System.Globalization;

namespace STAC
{
    public class Program
    {
        public record struct RunParams(Vector2i WindowPosition, Camera? Camera);

        public static GameWindow Run(MainComponent? mainComponent = null, RunParams? runParams = null)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

            var nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(1920, 1080),
                Title = "STAC",
                // This is needed to run on macos
                Flags = ContextFlags.ForwardCompatible,
            };

#if BENCHMARK
            BenchmarkWindow window = new(GameWindowSettings.Default, nativeWindowSettings)
            {
                MainComponent = mainComponent,
            };
#else
            Window window = new Window(GameWindowSettings.Default, nativeWindowSettings)
            {
                MainComponent = mainComponent,
                Camera = runParams?.Camera
            };
#endif
            BoundingGenerator.Initialize(window);
            //BoundingGenerator.Demo();
            //return null!;
            if (runParams == null)
            {
                window.Run();
            }
            else
            {
                window.Location = runParams.Value.WindowPosition;
                window.Context.MakeNoneCurrent();
                window.MainThreadSyncContext = SynchronizationContext.Current;
                new Thread(() =>
                {
                    if (window.MainThreadSyncContext == null)
                    {
                        window.MainThreadSyncContext = SynchronizationContext.Current;
                    }
                    else
                    {
                        window.Context.MakeCurrent();
                    }
                    window.Run();
                }).Start();
            }
            return window;
        }
        private static void Main()
        {
            Run();
        }
    }
}