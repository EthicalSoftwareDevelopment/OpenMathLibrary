namespace TheOpenMathLibrary.GraphicsDemo.Tests;

[TestClass]
public class VerticalSliderTests
{
    [TestMethod]
    public void Update_ClickingInsideSliderStartsDraggingAndChangesValue()
    {
        VerticalSlider slider = new();
        VerticalSlider.SliderLayout layout = slider.GetLayout(1280, 720);

        slider.Update(true, layout.TrackX + 4f, layout.TrackY + 8f, 1280, 720);

        Assert.IsTrue(slider.IsDragging);
        Assert.IsTrue(slider.Value > 0.5f);
    }

    [TestMethod]
    public void Update_ReleasingMouseStopsDragging()
    {
        VerticalSlider slider = new();
        VerticalSlider.SliderLayout layout = slider.GetLayout(1280, 720);

        slider.Update(true, layout.TrackX + 4f, layout.TrackY + 8f, 1280, 720);
        slider.Update(false, layout.TrackX + 4f, layout.TrackY + 8f, 1280, 720);

        Assert.IsFalse(slider.IsDragging);
    }
}


