using Silk.NET.GLFW;

namespace TheOpenMathLibrary.GraphicsDemo.Rendering;

/// <summary>
/// Owns the native GLFW window used by the Vulkan demo.
/// </summary>
public unsafe sealed class GlfwWindowHost : IDisposable
{
    private readonly GlfwCallbacks.ScrollCallback _scrollCallback;
    private readonly int _initialWidth;
    private readonly int _initialHeight;
    private readonly string _title;
    private bool _disposed;
    private bool _framebufferResizedSinceLastPoll;
    private bool _hasCursorSample;
    private bool _initialized;
    private double _lastCursorX;
    private double _lastCursorY;
    private float _mouseDeltaX;
    private float _mouseDeltaY;
    private float _scrollDeltaY;
    private int _previousFramebufferHeight;
    private int _previousFramebufferWidth;
    private WindowHandle* _window;

    /// <summary>
    /// Initializes a new instance of the <see cref="GlfwWindowHost"/> class.
    /// </summary>
    public GlfwWindowHost(int width, int height, string title)
    {
        _initialWidth = width;
        _initialHeight = height;
        _title = title;
        Glfw = Glfw.GetApi();
        _scrollCallback = OnScroll;
    }

    /// <summary>
    /// Gets the GLFW API binding.
    /// </summary>
    public Glfw Glfw { get; }

    /// <summary>
    /// Gets the native window handle.
    /// </summary>
    public WindowHandle* Handle => _window;

    /// <summary>
    /// Gets the current framebuffer width.
    /// </summary>
    public int FramebufferWidth
    {
        get
        {
            EnsureInitialized();
            Glfw.GetFramebufferSize(_window, out int width, out _);
            return width;
        }
    }

    /// <summary>
    /// Gets the current framebuffer height.
    /// </summary>
    public int FramebufferHeight
    {
        get
        {
            EnsureInitialized();
            Glfw.GetFramebufferSize(_window, out _, out int height);
            return height;
        }
    }

    /// <summary>
    /// Gets the mouse delta along the X axis since the last poll.
    /// </summary>
    public float MouseDeltaX => _mouseDeltaX;

    /// <summary>
    /// Gets the mouse delta along the Y axis since the last poll.
    /// </summary>
    public float MouseDeltaY => _mouseDeltaY;

    /// <summary>
    /// Gets the accumulated mouse wheel delta since the last poll.
    /// </summary>
    public float ScrollDeltaY => _scrollDeltaY;

    /// <summary>
    /// Gets a value indicating whether the framebuffer size changed during the latest poll cycle.
    /// </summary>
    public bool FramebufferResizedSinceLastPoll => _framebufferResizedSinceLastPoll;

    /// <summary>
    /// Gets a value indicating whether the window currently has a usable framebuffer.
    /// </summary>
    public bool HasUsableFramebuffer => FramebufferWidth > 0 && FramebufferHeight > 0;

    /// <summary>
    /// Initializes GLFW and creates a no-API window.
    /// </summary>
    public void Initialize()
    {
        if (_initialized)
        {
            return;
        }

        if (!Glfw.Init())
        {
            throw new InvalidOperationException("GLFW could not be initialized.");
        }

        Glfw.DefaultWindowHints();
        Glfw.WindowHint(WindowHintClientApi.ClientApi, ClientApi.NoApi);
        Glfw.WindowHint(WindowHintBool.Resizable, true);

        _window = Glfw.CreateWindow(_initialWidth, _initialHeight, _title, null, null);
        if (_window is null)
        {
            Glfw.Terminate();
            throw new InvalidOperationException("The GLFW window could not be created.");
        }

        Glfw.SetScrollCallback(_window, _scrollCallback);

        Glfw.GetFramebufferSize(_window, out _previousFramebufferWidth, out _previousFramebufferHeight);
        Glfw.GetCursorPos(_window, out _lastCursorX, out _lastCursorY);
        _hasCursorSample = true;

        _initialized = true;
    }

    /// <summary>
    /// Processes the pending window events.
    /// </summary>
    public void PollEvents()
    {
        EnsureInitialized();
        _framebufferResizedSinceLastPoll = false;
        _mouseDeltaX = 0f;
        _mouseDeltaY = 0f;
        _scrollDeltaY = 0f;

        Glfw.PollEvents();

        Glfw.GetFramebufferSize(_window, out int width, out int height);
        _framebufferResizedSinceLastPoll = width != _previousFramebufferWidth || height != _previousFramebufferHeight;
        _previousFramebufferWidth = width;
        _previousFramebufferHeight = height;

        Glfw.GetCursorPos(_window, out double cursorX, out double cursorY);
        if (_hasCursorSample)
        {
            _mouseDeltaX = (float)(cursorX - _lastCursorX);
            _mouseDeltaY = (float)(cursorY - _lastCursorY);
        }

        _lastCursorX = cursorX;
        _lastCursorY = cursorY;
        _hasCursorSample = true;
    }

    /// <summary>
    /// Gets a value indicating whether the window should close.
    /// </summary>
    public bool ShouldClose
    {
        get
        {
            EnsureInitialized();
            return Glfw.WindowShouldClose(_window);
        }
    }

    /// <summary>
    /// Requests that the native window close.
    /// </summary>
    public void RequestClose()
    {
        EnsureInitialized();
        Glfw.SetWindowShouldClose(_window, true);
    }

    /// <summary>
    /// Sets the native window title.
    /// </summary>
    public void SetTitle(string title)
    {
        EnsureInitialized();
        Glfw.SetWindowTitle(_window, title);
    }

    /// <summary>
    /// Determines whether the specified key is currently pressed.
    /// </summary>
    public bool IsKeyDown(Keys key)
    {
        EnsureInitialized();
        InputAction action = (InputAction)Glfw.GetKey(_window, key);
        return action == InputAction.Press || action == InputAction.Repeat;
    }

    /// <summary>
    /// Determines whether the specified mouse button is currently pressed.
    /// </summary>
    public bool IsMouseButtonDown(MouseButton button)
    {
        EnsureInitialized();
        InputAction action = (InputAction)Glfw.GetMouseButton(_window, (int)button);
        return action == InputAction.Press || action == InputAction.Repeat;
    }

    /// <summary>
    /// Gets the Vulkan instance extensions required by GLFW.
    /// </summary>
    public byte** GetRequiredInstanceExtensions(out uint count)
    {
        EnsureInitialized();
        return Glfw.GetRequiredInstanceExtensions(out count);
    }

    private void OnScroll(WindowHandle* window, double offsetX, double offsetY)
    {
        _scrollDeltaY += (float)offsetY;
    }

    private void EnsureInitialized()
    {
        if (!_initialized || _window is null)
        {
            throw new InvalidOperationException("The GLFW window host has not been initialized.");
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        if (_window is not null)
        {
            Glfw.DestroyWindow(_window);
            _window = null;
        }

        if (_initialized)
        {
            Glfw.Terminate();
        }

        _disposed = true;
    }
}




