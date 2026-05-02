using System.Numerics;
using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace TheOpenMathLibrary.GraphicsDemo.Rendering;

/// <summary>
/// Represents a vertex used by the toroid renderer.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Vertex
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Vertex"/> struct.
    /// </summary>
    public Vertex(Vector3 position, Vector3 normal)
    {
        Position = position;
        Normal = normal;
    }

    /// <summary>
    /// Gets the position in object space.
    /// </summary>
    public Vector3 Position { get; }

    /// <summary>
    /// Gets the outward unit normal.
    /// </summary>
    public Vector3 Normal { get; }

    /// <summary>
    /// Gets the size of the vertex in bytes.
    /// </summary>
    public static uint SizeInBytes => (uint)Marshal.SizeOf<Vertex>();

    /// <summary>
    /// Creates the Vulkan binding description for this vertex layout.
    /// </summary>
    public static VertexInputBindingDescription GetBindingDescription()
    {
        return new VertexInputBindingDescription
        {
            Binding = 0,
            Stride = SizeInBytes,
            InputRate = VertexInputRate.Vertex,
        };
    }

    /// <summary>
    /// Creates the Vulkan attribute descriptions for this vertex layout.
    /// </summary>
    public static VertexInputAttributeDescription[] GetAttributeDescriptions()
    {
        return
        [
            new VertexInputAttributeDescription
            {
                Binding = 0,
                Location = 0,
                Format = Format.R32G32B32Sfloat,
                Offset = 0,
            },
            new VertexInputAttributeDescription
            {
                Binding = 0,
                Location = 1,
                Format = Format.R32G32B32Sfloat,
                Offset = 12,
            },
        ];
    }
}

