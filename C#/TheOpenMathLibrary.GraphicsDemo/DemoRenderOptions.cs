using System.Numerics;

namespace TheOpenMathLibrary.GraphicsDemo;

/// <summary>
/// Represents the per-frame render options for the toroid demo.
/// </summary>
public readonly record struct DemoRenderOptions(
    Matrix4x4 ViewProjectionMatrix,
    Matrix4x4 ModelMatrix,
    bool UseWireframe,
    float ShaderSliderValue = 0.5f);

