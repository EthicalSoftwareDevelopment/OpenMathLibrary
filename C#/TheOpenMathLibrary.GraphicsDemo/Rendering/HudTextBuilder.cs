using System.Numerics;

namespace TheOpenMathLibrary.GraphicsDemo.Rendering;

/// <summary>
/// Builds a simple in-scene HUD from a tiny bitmap font.
/// </summary>
public static class HudTextBuilder
{
    private static readonly Dictionary<char, byte[]> Glyphs = new()
    {
        [' '] = [0, 0, 0, 0, 0, 0, 0],
        ['0'] = [0b01110, 0b10001, 0b10011, 0b10101, 0b11001, 0b10001, 0b01110],
        ['1'] = [0b00100, 0b01100, 0b00100, 0b00100, 0b00100, 0b00100, 0b01110],
        ['2'] = [0b01110, 0b10001, 0b00001, 0b00010, 0b00100, 0b01000, 0b11111],
        ['3'] = [0b11110, 0b00001, 0b00001, 0b01110, 0b00001, 0b00001, 0b11110],
        ['4'] = [0b00010, 0b00110, 0b01010, 0b10010, 0b11111, 0b00010, 0b00010],
        ['5'] = [0b11111, 0b10000, 0b10000, 0b11110, 0b00001, 0b00001, 0b11110],
        ['6'] = [0b01110, 0b10000, 0b10000, 0b11110, 0b10001, 0b10001, 0b01110],
        ['7'] = [0b11111, 0b00001, 0b00010, 0b00100, 0b01000, 0b01000, 0b01000],
        ['8'] = [0b01110, 0b10001, 0b10001, 0b01110, 0b10001, 0b10001, 0b01110],
        ['9'] = [0b01110, 0b10001, 0b10001, 0b01111, 0b00001, 0b00001, 0b01110],
        ['A'] = [0b01110, 0b10001, 0b10001, 0b11111, 0b10001, 0b10001, 0b10001],
        ['B'] = [0b11110, 0b10001, 0b10001, 0b11110, 0b10001, 0b10001, 0b11110],
        ['C'] = [0b01110, 0b10001, 0b10000, 0b10000, 0b10000, 0b10001, 0b01110],
        ['D'] = [0b11100, 0b10010, 0b10001, 0b10001, 0b10001, 0b10010, 0b11100],
        ['E'] = [0b11111, 0b10000, 0b10000, 0b11110, 0b10000, 0b10000, 0b11111],
        ['F'] = [0b11111, 0b10000, 0b10000, 0b11110, 0b10000, 0b10000, 0b10000],
        ['G'] = [0b01110, 0b10001, 0b10000, 0b10111, 0b10001, 0b10001, 0b01110],
        ['H'] = [0b10001, 0b10001, 0b10001, 0b11111, 0b10001, 0b10001, 0b10001],
        ['I'] = [0b01110, 0b00100, 0b00100, 0b00100, 0b00100, 0b00100, 0b01110],
        ['K'] = [0b10001, 0b10010, 0b10100, 0b11000, 0b10100, 0b10010, 0b10001],
        ['L'] = [0b10000, 0b10000, 0b10000, 0b10000, 0b10000, 0b10000, 0b11111],
        ['M'] = [0b10001, 0b11011, 0b10101, 0b10101, 0b10001, 0b10001, 0b10001],
        ['N'] = [0b10001, 0b11001, 0b10101, 0b10011, 0b10001, 0b10001, 0b10001],
        ['O'] = [0b01110, 0b10001, 0b10001, 0b10001, 0b10001, 0b10001, 0b01110],
        ['P'] = [0b11110, 0b10001, 0b10001, 0b11110, 0b10000, 0b10000, 0b10000],
        ['Q'] = [0b01110, 0b10001, 0b10001, 0b10001, 0b10101, 0b10010, 0b01101],
        ['R'] = [0b11110, 0b10001, 0b10001, 0b11110, 0b10100, 0b10010, 0b10001],
        ['S'] = [0b01111, 0b10000, 0b10000, 0b01110, 0b00001, 0b00001, 0b11110],
        ['T'] = [0b11111, 0b00100, 0b00100, 0b00100, 0b00100, 0b00100, 0b00100],
        ['U'] = [0b10001, 0b10001, 0b10001, 0b10001, 0b10001, 0b10001, 0b01110],
        ['W'] = [0b10001, 0b10001, 0b10001, 0b10101, 0b10101, 0b10101, 0b01010],
        ['Y'] = [0b10001, 0b10001, 0b01010, 0b00100, 0b00100, 0b00100, 0b00100],
        ['Z'] = [0b11111, 0b00001, 0b00010, 0b00100, 0b01000, 0b10000, 0b11111],
        [':'] = [0b00000, 0b00100, 0b00100, 0b00000, 0b00100, 0b00100, 0b00000],
    };

    /// <summary>
    /// Builds clip-space vertices for the supplied HUD lines.
    /// </summary>
    public static HudVertex[] Build(IReadOnlyList<string> lines, uint width, uint height, float sliderValue)
    {
        if (lines.Count == 0 || width == 0 || height == 0)
        {
            return [];
        }

        const float pixelSize = 3f;
        const float left = 14f;
        const float top = 14f;
        const float lineSpacing = 8f;
        const float charSpacing = 2f;

        List<HudVertex> vertices = new();

        int longestLine = lines.Max(line => line.Length);
        float panelWidth = left + longestLine * (5f * pixelSize + charSpacing) + 14f;
        float panelHeight = top + lines.Count * (7f * pixelSize + lineSpacing) + 8f;
        AddRectangle(vertices, 8f, 8f, panelWidth, panelHeight, new Vector3(0.02f, 0.02f, 0.06f), width, height);
        AddRectangle(vertices, 8f, 8f, panelWidth, 20f, new Vector3(0.12f, 0.18f, 0.30f), width, height);

        AddSlider(vertices, sliderValue, width, height);

        for (int lineIndex = 0; lineIndex < lines.Count; lineIndex++)
        {
            string line = lines[lineIndex].ToUpperInvariant();
            float baselineY = top + lineIndex * (7f * pixelSize + lineSpacing);
            float cursorX = left;

            foreach (char character in line)
            {
                AddGlyph(vertices, character, cursorX, baselineY, pixelSize, new Vector3(0.95f, 0.96f, 1.0f), width, height);
                cursorX += 5f * pixelSize + charSpacing;
            }
        }

        return [.. vertices];
    }

    private static void AddGlyph(List<HudVertex> vertices, char character, float originX, float originY, float pixelSize, Vector3 color, uint width, uint height)
    {
        if (!Glyphs.TryGetValue(character, out byte[]? rows))
        {
            rows = Glyphs[' '];
        }

        for (int row = 0; row < rows.Length; row++)
        {
            for (int column = 0; column < 5; column++)
            {
                int mask = 1 << (4 - column);
                if ((rows[row] & mask) == 0)
                {
                    continue;
                }

                AddPixel(vertices, originX + column * pixelSize, originY + row * pixelSize, pixelSize, color, width, height);
            }
        }
    }

    private static void AddRectangle(List<HudVertex> vertices, float x, float y, float rectangleWidth, float rectangleHeight, Vector3 color, uint width, uint height)
    {
        Vector2 topLeft = ToClipSpace(x, y, width, height);
        Vector2 topRight = ToClipSpace(x + rectangleWidth, y, width, height);
        Vector2 bottomLeft = ToClipSpace(x, y + rectangleHeight, width, height);
        Vector2 bottomRight = ToClipSpace(x + rectangleWidth, y + rectangleHeight, width, height);

        AddTriangle(vertices, topLeft, bottomLeft, topRight, color);
        AddTriangle(vertices, topRight, bottomLeft, bottomRight, color);
    }

    private static void AddPixel(List<HudVertex> vertices, float x, float y, float pixelSize, Vector3 color, uint width, uint height)
    {
        Vector2 topLeft = ToClipSpace(x, y, width, height);
        Vector2 topRight = ToClipSpace(x + pixelSize, y, width, height);
        Vector2 bottomLeft = ToClipSpace(x, y + pixelSize, width, height);
        Vector2 bottomRight = ToClipSpace(x + pixelSize, y + pixelSize, width, height);

        AddTriangle(vertices, topLeft, bottomLeft, topRight, color);
        AddTriangle(vertices, topRight, bottomLeft, bottomRight, color);
    }

    private static void AddTriangle(List<HudVertex> vertices, Vector2 a, Vector2 b, Vector2 c, Vector3 color)
    {
        vertices.Add(new HudVertex { Position = a, Color = color });
        vertices.Add(new HudVertex { Position = b, Color = color });
        vertices.Add(new HudVertex { Position = c, Color = color });
    }

    private static void AddSlider(List<HudVertex> vertices, float sliderValue, uint width, uint height)
    {
        float trackHeight = MathF.Min(180f, height - 112f);
        float trackWidth = 18f;
        float trackX = width - 36f;
        float trackY = 56f;
        float handleHeight = 24f;
        float travel = trackHeight - handleHeight;
        float handleY = trackY + (1f - Math.Clamp(sliderValue, 0f, 1f)) * travel;

        AddRectangle(vertices, trackX - 6f, trackY - 8f, trackWidth + 12f, trackHeight + 16f, new Vector3(0.02f, 0.02f, 0.06f), width, height);
        AddRectangle(vertices, trackX, trackY, trackWidth, trackHeight, new Vector3(0.22f, 0.28f, 0.36f), width, height);
        AddRectangle(vertices, trackX + 3f, trackY + 3f, trackWidth - 6f, trackHeight - 6f, new Vector3(0.10f, 0.12f, 0.18f), width, height);
        AddRectangle(vertices, trackX - 3f, handleY, trackWidth + 6f, handleHeight, new Vector3(0.17f, 0.60f, 0.94f), width, height);
        AddRectangle(vertices, trackX, handleY + 4f, trackWidth, handleHeight - 8f, new Vector3(0.90f, 0.96f, 1.0f), width, height);
    }

    private static Vector2 ToClipSpace(float pixelX, float pixelY, uint width, uint height)
    {
        float x = 2f * pixelX / width - 1f;
        float y = 2f * pixelY / height - 1f;
        return new Vector2(x, y);
    }
}


