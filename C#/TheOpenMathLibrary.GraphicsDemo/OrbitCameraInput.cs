namespace TheOpenMathLibrary.GraphicsDemo;

/// <summary>
/// Represents camera movement input for the toroid demo.
/// </summary>
public readonly record struct OrbitCameraInput(
    bool RotateLeft,
    bool RotateRight,
    bool RotateUp,
    bool RotateDown,
    bool ZoomIn,
    bool ZoomOut,
    bool IsMouseOrbiting,
    bool IsMouseZooming,
    float MouseDeltaX,
    float MouseDeltaY,
    float ScrollDeltaY);


