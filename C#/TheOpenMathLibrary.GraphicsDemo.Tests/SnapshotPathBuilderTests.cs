namespace TheOpenMathLibrary.GraphicsDemo.Tests;

[TestClass]
public class SnapshotPathBuilderTests
{
    [TestMethod]
    public void Create_ReturnsTimestampedPpmPathUnderScreenshotsDirectory()
    {
        string root = Path.Combine(Path.GetTempPath(), "OpenMathLibrary-SnapshotPathTests");
        DateTimeOffset timestamp = new DateTimeOffset(2026, 5, 2, 14, 30, 15, TimeSpan.Zero).AddMilliseconds(123);

        string path = SnapshotPathBuilder.Create(root, timestamp);

        StringAssert.Contains(path, Path.Combine(root, "Screenshots"));
        StringAssert.EndsWith(path, ".ppm");
        StringAssert.Contains(path, "20260502-143015-123");
    }
}


