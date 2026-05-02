using System.Diagnostics;
using System.Numerics;
using Silk.NET.GLFW;
using TheOpenMathLibrary.GraphicsDemo.Rendering;

namespace TheOpenMathLibrary.GraphicsDemo;

/// <summary>
/// Coordinates the demo window, camera, and Vulkan renderer.
/// </summary>
public sealed class GraphicsDemoApp
{
    private const string BaseTitle = "TheOpenMathLibrary.GraphicsDemo - Vulkan Toroid";
    private static readonly TimeSpan TargetFrameDuration = TimeSpan.FromSeconds(1d / 60d);
    private readonly GlfwWindowHost _windowHost = new(1280, 720, "TheOpenMathLibrary.GraphicsDemo - Vulkan Toroid");
    private readonly VerticalSlider _shaderSlider = new(0.5f);
    private readonly InputEdge _autoRotateToggle = new();
    private readonly InputEdge _screenshotToggle = new();
    private readonly InputEdge _wireframeToggle = new();

    /// <summary>
    /// Runs the standalone graphics demo.
    /// </summary>
    public void Run()
    {
        using (_windowHost)
        using (VulkanRenderer renderer = new(_windowHost, "TheOpenMathLibrary.GraphicsDemo"))
        {
            _windowHost.Initialize();
            renderer.Initialize();

            OrbitCamera camera = OrbitCamera.CreateDefault();
            Stopwatch stopwatch = Stopwatch.StartNew();
            TimeSpan previous = stopwatch.Elapsed;
            bool autoRotate = true;
            bool wireframe = false;
            float toroidRotation = 0f;

            while (!_windowHost.ShouldClose)
            {
                _windowHost.PollEvents();

                if (_windowHost.IsKeyDown(Keys.Escape))
                {
                    _windowHost.RequestClose();
                    continue;
                }

                TimeSpan current = stopwatch.Elapsed;
                float deltaSeconds = (float)(current - previous).TotalSeconds;
                previous = current;

                if (_wireframeToggle.IsTriggered(_windowHost.IsKeyDown(Keys.F1)) && renderer.SupportsWireframe)
                {
                    wireframe = !wireframe;
                }

                if (_autoRotateToggle.IsTriggered(_windowHost.IsKeyDown(Keys.R)))
                {
                    autoRotate = !autoRotate;
                }

                if (autoRotate)
                {
                    toroidRotation += deltaSeconds * 0.8f;
                }

                bool primaryMouseDown = _windowHost.IsMouseButtonDown(MouseButton.Left);
                _shaderSlider.Update(primaryMouseDown, _windowHost.CursorX, _windowHost.CursorY, _windowHost.FramebufferWidth, _windowHost.FramebufferHeight);

                _windowHost.SetTitle(BuildWindowTitle(renderer.SupportsWireframe, wireframe, autoRotate));

                camera.Update(GetCameraInput(_shaderSlider.IsDragging), deltaSeconds);

                if (!_windowHost.HasUsableFramebuffer)
                {
                    Thread.Sleep(50);
                    continue;
                }

                int width = _windowHost.FramebufferWidth;
                int height = _windowHost.FramebufferHeight;

                Matrix4x4 viewProjectionMatrix = camera.CreateViewProjectionMatrix(width / (float)height);
                Matrix4x4 modelMatrix = Matrix4x4.CreateRotationX(0.7f) * Matrix4x4.CreateRotationY(toroidRotation);
                DemoRenderOptions renderOptions = new(viewProjectionMatrix, modelMatrix, wireframe, _shaderSlider.Value);

                renderer.RenderFrame(renderOptions, width, height);

                if (_screenshotToggle.IsTriggered(_windowHost.IsKeyDown(Keys.P)))
                {
                    string path = SceneSnapshotExporter.Export(Environment.CurrentDirectory, renderOptions, width, height);
                    Console.WriteLine($"Exported snapshot: {path}");
                }

                TimeSpan frameElapsed = stopwatch.Elapsed - current;
                TimeSpan sleepTime = TargetFrameDuration - frameElapsed;
                if (sleepTime > TimeSpan.Zero)
                {
                    Thread.Sleep(sleepTime);
                }
            }

            renderer.WaitIdle();
        }
    }

    private OrbitCameraInput GetCameraInput(bool suppressMouseOrbit)
    {
        return new OrbitCameraInput(
            RotateLeft: _windowHost.IsKeyDown(Keys.Left),
            RotateRight: _windowHost.IsKeyDown(Keys.Right),
            RotateUp: _windowHost.IsKeyDown(Keys.Up),
            RotateDown: _windowHost.IsKeyDown(Keys.Down),
            ZoomIn: _windowHost.IsKeyDown(Keys.W) || _windowHost.IsKeyDown(Keys.Equal),
            ZoomOut: _windowHost.IsKeyDown(Keys.S) || _windowHost.IsKeyDown(Keys.Minus),
            IsMouseOrbiting: !suppressMouseOrbit && _windowHost.IsMouseButtonDown(MouseButton.Left),
            IsMouseZooming: _windowHost.IsMouseButtonDown(MouseButton.Right),
            MouseDeltaX: _windowHost.MouseDeltaX,
            MouseDeltaY: _windowHost.MouseDeltaY,
            ScrollDeltaY: _windowHost.ScrollDeltaY);
    }

    private static string BuildWindowTitle(bool supportsWireframe, bool wireframeEnabled, bool autoRotateEnabled)
    {
        string wireframeStatus = supportsWireframe
            ? (wireframeEnabled ? "wire:on" : "wire:off")
            : "wire:n/a";

        string rotationStatus = autoRotateEnabled ? "rotate:on" : "rotate:off";
        return $"{BaseTitle} | LMB orbit | RMB drag zoom | Wheel zoom | F1 {wireframeStatus} | R {rotationStatus} | P snapshot | Esc quit";
    }
}


