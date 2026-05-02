using System.Numerics;
using System.Runtime.InteropServices;

namespace TheOpenMathLibrary.GraphicsDemo.Rendering;

/// <summary>
/// Represents the push-constant data sent to the toroid shaders.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct ScenePushConstants
{
    /// <summary>
    /// Gets or sets the transposed model-view-projection matrix.
    /// </summary>
    public Matrix4x4 Mvp { get; set; }

    /// <summary>
    /// Gets or sets the transposed model matrix.
    /// </summary>
    public Matrix4x4 Model { get; set; }

    /// <summary>
    /// Gets the total byte size of this push-constant payload.
    /// </summary>
    public static uint SizeInBytes => (uint)Marshal.SizeOf<ScenePushConstants>();
}

