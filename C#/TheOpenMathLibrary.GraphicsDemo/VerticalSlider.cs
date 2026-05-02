namespace TheOpenMathLibrary.GraphicsDemo;

/// <summary>
/// Represents a simple vertical slider anchored to the right side of the demo window.
/// </summary>
public sealed class VerticalSlider
{
    private const float Width = 18f;
    private const float Height = 180f;
    private const float MarginRight = 18f;
    private const float MarginTop = 56f;
    private const float HandleHeight = 24f;
    private bool _wasPrimaryDown;

    /// <summary>
    /// Initializes a new instance of the <see cref="VerticalSlider"/> class.
    /// </summary>
    public VerticalSlider(float initialValue = 0.5f)
    {
        Value = Math.Clamp(initialValue, 0f, 1f);
    }

    /// <summary>
    /// Gets the normalized slider value in the range [0, 1].
    /// </summary>
    public float Value { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the slider is currently being dragged.
    /// </summary>
    public bool IsDragging { get; private set; }

    /// <summary>
    /// Gets the current slider layout for the given framebuffer size.
    /// </summary>
    public SliderLayout GetLayout(int windowWidth, int windowHeight)
    {
        float trackX = Math.Max(8f, windowWidth - MarginRight - Width);
        float trackY = Math.Clamp(MarginTop, 8f, Math.Max(8f, windowHeight - Height - 8f));
        float handleTravel = Height - HandleHeight;
        float handleY = trackY + (1f - Value) * handleTravel;
        return new SliderLayout(trackX, trackY, Width, Height, HandleHeight);
    }

    /// <summary>
    /// Updates the slider interaction state from the current mouse input.
    /// </summary>
    public void Update(bool primaryDown, float cursorX, float cursorY, int windowWidth, int windowHeight)
    {
        SliderLayout layout = GetLayout(windowWidth, windowHeight);
        bool pressedThisFrame = primaryDown && !_wasPrimaryDown;

        if (pressedThisFrame && layout.Contains(cursorX, cursorY))
        {
            IsDragging = true;
            SetValueFromCursor(cursorY, layout);
        }
        else if (!primaryDown)
        {
            IsDragging = false;
        }
        else if (IsDragging)
        {
            SetValueFromCursor(cursorY, layout);
        }

        _wasPrimaryDown = primaryDown;
    }

    private void SetValueFromCursor(float cursorY, SliderLayout layout)
    {
        float travel = layout.TrackHeight - layout.SliderHandleHeight;
        if (travel <= 0f)
        {
            Value = 0.5f;
            return;
        }

        float relative = Math.Clamp(cursorY - layout.TrackY - layout.SliderHandleHeight * 0.5f, 0f, travel);
        Value = 1f - relative / travel;
    }

    /// <summary>
    /// Describes the screen-space slider geometry.
    /// </summary>
    public readonly record struct SliderLayout(float TrackX, float TrackY, float TrackWidth, float TrackHeight, float SliderHandleHeight)
    {
        /// <summary>
        /// Determines whether the given point lies within the slider track bounds.
        /// </summary>
        public bool Contains(float x, float y)
        {
            return x >= TrackX && x <= TrackX + TrackWidth && y >= TrackY && y <= TrackY + TrackHeight;
        }
    }
}

