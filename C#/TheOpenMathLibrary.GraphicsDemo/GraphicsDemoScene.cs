using System.Numerics;
using TheOpenMathLibrary.GraphicsDemo.Geometry;

namespace TheOpenMathLibrary.GraphicsDemo;

/// <summary>
/// Defines shared scene constants for the standalone toroid demo.
/// </summary>
public static class GraphicsDemoScene
{
    /// <summary>
    /// Gets the toroid major radius.
    /// </summary>
    public const float MajorRadius = 1.45f;

    /// <summary>
    /// Gets the toroid minor radius.
    /// </summary>
    public const float MinorRadius = 0.45f;

    /// <summary>
    /// Gets the toroid major segment count.
    /// </summary>
    public const int MajorSegments = 96;

    /// <summary>
    /// Gets the toroid minor segment count.
    /// </summary>
    public const int MinorSegments = 48;

    /// <summary>
    /// Gets the base shading color.
    /// </summary>
    public static Vector3 BaseColor => new(0.17f, 0.60f, 0.94f);

    /// <summary>
    /// Gets the background clear color.
    /// </summary>
    public static Vector3 BackgroundColor => new(0.05f, 0.06f, 0.10f);

    /// <summary>
    /// Gets the fixed demo light direction.
    /// </summary>
    public static Vector3 LightDirection => Vector3.Normalize(new Vector3(0.35f, 0.75f, 0.55f));

    /// <summary>
    /// Creates the toroid mesh used by the demo.
    /// </summary>
    public static ToroidMesh CreateToroidMesh()
    {
        return ToroidMeshGenerator.Create(MajorRadius, MinorRadius, MajorSegments, MinorSegments);
    }
}

