using System.Numerics;

namespace TheOpenMathLibrary.GraphicsDemo.Tests;

[TestClass]
public class SceneSnapshotExporterTests
{
    [TestMethod]
    public void Export_WritesBinaryPpmHeader()
    {
        string tempDirectory = Path.Combine(Path.GetTempPath(), "OpenMathLibrary-SceneSnapshotTests");
        Directory.CreateDirectory(tempDirectory);

        DemoRenderOptions renderOptions = new(Matrix4x4.Identity, Matrix4x4.Identity, false);
        string path = SceneSnapshotExporter.Export(tempDirectory, renderOptions, 32, 24);

        byte[] bytes = File.ReadAllBytes(path);
        string header = System.Text.Encoding.ASCII.GetString(bytes, 0, Math.Min(bytes.Length, 32));
        StringAssert.StartsWith(header, "P6\n32 24\n255\n");
    }
}

