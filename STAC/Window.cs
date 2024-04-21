using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using STAC.Components.Color;
using STAC.Components.March;
using STAC.Components;
using System.ComponentModel;

namespace STAC
{
    public class Window : GameWindow
    {
        private readonly float[] _vertices = new float[9];

        private int _vertexBufferObject;
        private int _vertexArrayObject;

        private Shader? Shader;
        public MainComponent? MainComponent;
        public SynchronizationContext? MainThreadSyncContext;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

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

            GenerationManager manager = new GenerationManager { Window = this, MainComponent = MainComponent };
            Shader = new(File.ReadAllText("Shaders/shader.vert"), manager.GetFragShaderSource());
            manager.OnPostCompilation(Shader);
            Shader.Use();
            OnScreenSizeChanged();
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
            if (Camera != null)
                Camera.AspectRatio = Size.X / (float)Size.Y;
        }

        DateTime LastFpsUpdate = DateTime.MinValue;
        int FrameCount = 0;
        int LastFrameFPS = 0;
        string? OriginalWindowTitle = null;
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            OriginalWindowTitle ??= Title;
            FrameCount++;

            if ((DateTime.UtcNow - LastFpsUpdate).TotalSeconds >= 1)
            {
                LastFrameFPS = FrameCount;
                FrameCount = 0;
                LastFpsUpdate = DateTime.UtcNow;
            }
            

            Title = $"{OriginalWindowTitle} FP: curr {1 / e.Time :0.} | avg {LastFrameFPS :0.}";


            GL.Clear(ClearBufferMask.ColorBufferBit);
            Shader!.Use();


            if (Camera != null && !string.IsNullOrWhiteSpace(ViewMatrixName))
                Shader.SetMatrix4(ViewMatrixName, Camera.GetViewMatrix().Inverted());

            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            SwapBuffers();
        }

        private bool _firstMove = true;
        private Vector2 _lastPos;
        public Camera? Camera { get; set; }
        public string? ViewMatrixName { get; set; }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            if (Camera != null)
            {
                const float cameraSpeed = 1.5f;
                const float sensitivity = 0.05f;

                if (input.IsKeyDown(Keys.W))
                {
                    Camera.Position += Camera.Front * cameraSpeed * (float)e.Time; // Forward
                }

                if (input.IsKeyDown(Keys.S))
                {
                    Camera.Position -= Camera.Front * cameraSpeed * (float)e.Time; // Backwards
                }
                if (input.IsKeyDown(Keys.A))
                {
                    Camera.Position += Camera.Right * cameraSpeed * (float)e.Time; // Left
                }
                if (input.IsKeyDown(Keys.D))
                {
                    Camera.Position -= Camera.Right * cameraSpeed * (float)e.Time; // Right
                }
                if (input.IsKeyDown(Keys.Space))
                {
                    Camera.Position += Camera.Up * cameraSpeed * (float)e.Time; // Up
                }
                if (input.IsKeyDown(Keys.LeftShift))
                {
                    Camera.Position -= Camera.Up * cameraSpeed * (float)e.Time; // Down
                }

                // Get the mouse state
                var mouse = MouseState;

                if (_firstMove) // This bool variable is initially set to true.
                {
                    _lastPos = new Vector2(mouse.X, mouse.Y);
                    _firstMove = false;
                }
                else
                {
                    // Calculate the offset of the mouse position
                    var deltaX = mouse.X - _lastPos.X;
                    var deltaY = mouse.Y - _lastPos.Y;
                    _lastPos = new Vector2(mouse.X, mouse.Y);

                    // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                    Camera.Yaw -= deltaX * sensitivity;
                    Camera.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top
                }
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
            OnScreenSizeChanged();
        }
    }
}
