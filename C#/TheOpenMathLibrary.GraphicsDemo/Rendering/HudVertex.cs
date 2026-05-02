using System.Numerics;
using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace TheOpenMathLibrary.GraphicsDemo.Rendering;

/// <summary>
/// Represents a 2D HUD vertex for overlay rendering.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct HudVertex
{
    /// <summary>
    /// Gets or sets the clip-space position.
    /// </summary>
    public Vector2 Position;

    /// <summary>
    /// Gets or sets the RGB color.
    /// </summary>
    public Vector3 Color;

    /// <summary>
    /// Gets the size of the vertex in bytes.
    /// </summary>
    public static uint SizeInBytes => (uint)Marshal.SizeOf<HudVertex>();

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
                Format = Format.R32G32Sfloat,
                Offset = 0,
            },
            new VertexInputAttributeDescription
            {
                Binding = 0,
                Location = 1,
                Format = Format.R32G32B32Sfloat,
                Offset = 8,
            },
        ];
    }
}

