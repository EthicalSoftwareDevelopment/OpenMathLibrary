using System.Numerics;
using System.Text;
using TheOpenMathLibrary.GraphicsDemo.Geometry;

namespace TheOpenMathLibrary.GraphicsDemo;

/// <summary>
/// Exports a CPU-rendered snapshot of the current toroid scene.
/// </summary>
public static class SceneSnapshotExporter
{
    /// <summary>
    /// Exports the current scene to a binary PPM image and returns the output path.
    /// </summary>
    public static string Export(string rootDirectory, DemoRenderOptions renderOptions, int width, int height)
    {
        if (width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width), "The width must be greater than zero.");
        }

        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(height), "The height must be greater than zero.");
        }

        ToroidMesh mesh = GraphicsDemoScene.CreateToroidMesh();
        string path = SnapshotPathBuilder.Create(rootDirectory, DateTimeOffset.Now);
        ExportPpm(path, mesh, renderOptions, width, height);
        return path;
    }

    internal static void ExportPpm(string path, ToroidMesh mesh, DemoRenderOptions renderOptions, int width, int height)
    {
        byte[] pixels = new byte[width * height * 3];
        float[] depthBuffer = new float[width * height];
        Array.Fill(depthBuffer, float.PositiveInfinity);
        FillBackground(pixels, GraphicsDemoScene.BackgroundColor);

        ScreenVertex[] vertices = TransformVertices(mesh.Vertices, renderOptions, width, height);
        for (int index = 0; index < mesh.Indices.Length; index += 3)
        {
            ScreenVertex a = vertices[mesh.Indices[index]];
            ScreenVertex b = vertices[mesh.Indices[index + 1]];
            ScreenVertex c = vertices[mesh.Indices[index + 2]];
            if (!a.IsVisible || !b.IsVisible || !c.IsVisible)
            {
                continue;
            }

            if (renderOptions.UseWireframe)
            {
                DrawLine(pixels, depthBuffer, width, height, a, b);
                DrawLine(pixels, depthBuffer, width, height, b, c);
                DrawLine(pixels, depthBuffer, width, height, c, a);
            }
            else
            {
                RasterizeTriangle(pixels, depthBuffer, width, height, a, b, c);
            }
        }

        WritePpm(path, width, height, pixels);
    }

    private static ScreenVertex[] TransformVertices(Rendering.Vertex[] vertices, DemoRenderOptions renderOptions, int width, int height)
    {
        ScreenVertex[] result = new ScreenVertex[vertices.Length];
        Matrix4x4 modelViewProjection = renderOptions.ModelMatrix * renderOptions.ViewProjectionMatrix;

        for (int index = 0; index < vertices.Length; index++)
        {
            Rendering.Vertex vertex = vertices[index];
            Vector4 clip = Vector4.Transform(new Vector4(vertex.Position, 1f), modelViewProjection);
            if (clip.W <= 0.0001f)
            {
                result[index] = default;
                continue;
            }

            Vector3 ndc = new(clip.X / clip.W, clip.Y / clip.W, clip.Z / clip.W);
            if (ndc.Z < -1.2f || ndc.Z > 1.2f)
            {
                result[index] = default;
                continue;
            }

            float x = (ndc.X * 0.5f + 0.5f) * (width - 1);
            float y = (1f - (ndc.Y * 0.5f + 0.5f)) * (height - 1);
            Vector3 worldNormal = Vector3.Normalize(Vector3.TransformNormal(vertex.Normal, renderOptions.ModelMatrix));
            float intensity = MathF.Max(Vector3.Dot(worldNormal, GraphicsDemoScene.LightDirection), 0.15f);

            result[index] = new ScreenVertex(x, y, ndc.Z, intensity, true);
        }

        return result;
    }

    private static void FillBackground(byte[] pixels, Vector3 color)
    {
        byte r = ToByte(color.X);
        byte g = ToByte(color.Y);
        byte b = ToByte(color.Z);

        for (int index = 0; index < pixels.Length; index += 3)
        {
            pixels[index] = r;
            pixels[index + 1] = g;
            pixels[index + 2] = b;
        }
    }

    private static void RasterizeTriangle(byte[] pixels, float[] depthBuffer, int width, int height, ScreenVertex a, ScreenVertex b, ScreenVertex c)
    {
        float area = Edge(a.X, a.Y, b.X, b.Y, c.X, c.Y);
        if (MathF.Abs(area) < 1e-5f)
        {
            return;
        }

        int minX = Math.Max(0, (int)MathF.Floor(MathF.Min(a.X, MathF.Min(b.X, c.X))));
        int maxX = Math.Min(width - 1, (int)MathF.Ceiling(MathF.Max(a.X, MathF.Max(b.X, c.X))));
        int minY = Math.Max(0, (int)MathF.Floor(MathF.Min(a.Y, MathF.Min(b.Y, c.Y))));
        int maxY = Math.Min(height - 1, (int)MathF.Ceiling(MathF.Max(a.Y, MathF.Max(b.Y, c.Y))));

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                float sampleX = x + 0.5f;
                float sampleY = y + 0.5f;
                float w0 = Edge(b.X, b.Y, c.X, c.Y, sampleX, sampleY);
                float w1 = Edge(c.X, c.Y, a.X, a.Y, sampleX, sampleY);
                float w2 = Edge(a.X, a.Y, b.X, b.Y, sampleX, sampleY);

                bool sameSign = area > 0f
                    ? w0 >= 0f && w1 >= 0f && w2 >= 0f
                    : w0 <= 0f && w1 <= 0f && w2 <= 0f;

                if (!sameSign)
                {
                    continue;
                }

                w0 /= area;
                w1 /= area;
                w2 /= area;

                float depth = w0 * a.Z + w1 * b.Z + w2 * c.Z;
                int pixelIndex = y * width + x;
                if (depth >= depthBuffer[pixelIndex])
                {
                    continue;
                }

                depthBuffer[pixelIndex] = depth;
                float intensity = Math.Clamp(w0 * a.Intensity + w1 * b.Intensity + w2 * c.Intensity, 0f, 1f);
                WritePixel(pixels, pixelIndex, GraphicsDemoScene.BaseColor * intensity);
            }
        }
    }

    private static void DrawLine(byte[] pixels, float[] depthBuffer, int width, int height, ScreenVertex start, ScreenVertex end)
    {
        float dx = end.X - start.X;
        float dy = end.Y - start.Y;
        int steps = Math.Max(1, (int)MathF.Ceiling(MathF.Max(MathF.Abs(dx), MathF.Abs(dy))));

        for (int step = 0; step <= steps; step++)
        {
            float t = step / (float)steps;
            int x = (int)MathF.Round(start.X + dx * t);
            int y = (int)MathF.Round(start.Y + dy * t);
            if ((uint)x >= width || (uint)y >= height)
            {
                continue;
            }

            int pixelIndex = y * width + x;
            float depth = start.Z + (end.Z - start.Z) * t;
            if (depth >= depthBuffer[pixelIndex])
            {
                continue;
            }

            depthBuffer[pixelIndex] = depth;
            float intensity = Math.Clamp(start.Intensity + (end.Intensity - start.Intensity) * t, 0f, 1f);
            WritePixel(pixels, pixelIndex, GraphicsDemoScene.BaseColor * intensity);
        }
    }

    private static float Edge(float ax, float ay, float bx, float by, float cx, float cy)
    {
        return (cx - ax) * (by - ay) - (cy - ay) * (bx - ax);
    }

    private static void WritePixel(byte[] pixels, int pixelIndex, Vector3 color)
    {
        int offset = pixelIndex * 3;
        pixels[offset] = ToByte(color.X);
        pixels[offset + 1] = ToByte(color.Y);
        pixels[offset + 2] = ToByte(color.Z);
    }

    private static byte ToByte(float value)
    {
        return (byte)Math.Clamp((int)MathF.Round(value * 255f), 0, 255);
    }

    private static void WritePpm(string path, int width, int height, byte[] pixels)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        byte[] header = Encoding.ASCII.GetBytes($"P6\n{width} {height}\n255\n");

        using FileStream stream = File.Create(path);
        stream.Write(header, 0, header.Length);
        stream.Write(pixels, 0, pixels.Length);
    }

    private readonly record struct ScreenVertex(float X, float Y, float Z, float Intensity, bool IsVisible);
}

