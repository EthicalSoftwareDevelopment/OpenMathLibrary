namespace TheOpenMathLibrary.GraphicsDemo;

/// <summary>
/// Detects rising-edge button presses for toggle-style input.
/// </summary>
public sealed class InputEdge
{
    private bool _wasPressed;

    /// <summary>
    /// Returns <see langword="true"/> only on the frame where the input changes from up to down.
    /// </summary>
    public bool IsTriggered(bool isPressed)
    {
        bool triggered = isPressed && !_wasPressed;
        _wasPressed = isPressed;
        return triggered;
    }
}

