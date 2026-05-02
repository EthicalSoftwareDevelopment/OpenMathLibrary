using TheOpenMathLibrary.GraphicsDemo.Rendering;

namespace TheOpenMathLibrary.GraphicsDemo.Geometry;

/// <summary>
/// Represents a generated toroid mesh local to the graphics demo.
/// </summary>
public sealed class ToroidMesh
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToroidMesh"/> class.
    /// </summary>
    public ToroidMesh(Vertex[] vertices, uint[] indices)
    {
        Vertices = vertices ?? throw new ArgumentNullException(nameof(vertices));
        Indices = indices ?? throw new ArgumentNullException(nameof(indices));
    }

    /// <summary>
    /// Gets the mesh vertices.
    /// </summary>
    public Vertex[] Vertices { get; }

    /// <summary>
    /// Gets the triangle indices.
    /// </summary>
    public uint[] Indices { get; }
}

