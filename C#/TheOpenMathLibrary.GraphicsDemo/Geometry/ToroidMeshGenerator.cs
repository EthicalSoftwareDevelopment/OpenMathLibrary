using System.Numerics;
using TheOpenMathLibrary.GraphicsDemo.Rendering;

namespace TheOpenMathLibrary.GraphicsDemo.Geometry;

/// <summary>
/// Generates toroid mesh data for the standalone graphics demo.
/// </summary>
public static class ToroidMeshGenerator
{
    /// <summary>
    /// Creates a toroid mesh using the supplied radii and segment counts.
    /// </summary>
    /// <param name="majorRadius">The distance from the toroid center to the center of the tube.</param>
    /// <param name="minorRadius">The radius of the tube.</param>
    /// <param name="majorSegments">The number of segments around the major ring.</param>
    /// <param name="minorSegments">The number of segments around the tube.</param>
    /// <returns>A generated toroid mesh.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any parameter is outside its valid range.</exception>
    public static ToroidMesh Create(float majorRadius, float minorRadius, int majorSegments, int minorSegments)
    {
        if (majorRadius <= 0f)
        {
            throw new ArgumentOutOfRangeException(nameof(majorRadius), "The major radius must be greater than zero.");
        }

        if (minorRadius <= 0f)
        {
            throw new ArgumentOutOfRangeException(nameof(minorRadius), "The minor radius must be greater than zero.");
        }

        if (majorSegments < 3)
        {
            throw new ArgumentOutOfRangeException(nameof(majorSegments), "The major segment count must be at least 3.");
        }

        if (minorSegments < 3)
        {
            throw new ArgumentOutOfRangeException(nameof(minorSegments), "The minor segment count must be at least 3.");
        }

        Vertex[] vertices = new Vertex[majorSegments * minorSegments];
        uint[] indices = new uint[majorSegments * minorSegments * 6];

        int vertexIndex = 0;
        for (int major = 0; major < majorSegments; major++)
        {
            float majorAngle = major * MathF.Tau / majorSegments;
            float majorCos = MathF.Cos(majorAngle);
            float majorSin = MathF.Sin(majorAngle);

            for (int minor = 0; minor < minorSegments; minor++)
            {
                float minorAngle = minor * MathF.Tau / minorSegments;
                float minorCos = MathF.Cos(minorAngle);
                float minorSin = MathF.Sin(minorAngle);

                float ringRadius = majorRadius + minorRadius * minorCos;
                Vector3 position = new(
                    ringRadius * majorCos,
                    minorRadius * minorSin,
                    ringRadius * majorSin);

                Vector3 normal = Vector3.Normalize(new Vector3(
                    minorCos * majorCos,
                    minorSin,
                    minorCos * majorSin));

                vertices[vertexIndex++] = new Vertex(position, normal);
            }
        }

        int indexIndex = 0;
        for (int major = 0; major < majorSegments; major++)
        {
            int nextMajor = (major + 1) % majorSegments;

            for (int minor = 0; minor < minorSegments; minor++)
            {
                int nextMinor = (minor + 1) % minorSegments;

                uint a = (uint)(major * minorSegments + minor);
                uint b = (uint)(nextMajor * minorSegments + minor);
                uint c = (uint)(nextMajor * minorSegments + nextMinor);
                uint d = (uint)(major * minorSegments + nextMinor);

                indices[indexIndex++] = a;
                indices[indexIndex++] = b;
                indices[indexIndex++] = c;
                indices[indexIndex++] = a;
                indices[indexIndex++] = c;
                indices[indexIndex++] = d;
            }
        }

        return new ToroidMesh(vertices, indices);
    }
}

