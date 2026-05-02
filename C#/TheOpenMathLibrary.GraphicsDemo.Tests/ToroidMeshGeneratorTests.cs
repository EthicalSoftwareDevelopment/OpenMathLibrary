using System.Numerics;
using TheOpenMathLibrary.GraphicsDemo.Geometry;

namespace TheOpenMathLibrary.GraphicsDemo.Tests;

[TestClass]
public class ToroidMeshGeneratorTests
{
    private const float Tolerance = 1e-4f;

    [TestMethod]
    public void Create_ReturnsExpectedVertexAndIndexCounts()
    {
        ToroidMesh mesh = ToroidMeshGenerator.Create(2f, 0.5f, 12, 8);

        Assert.AreEqual(96, mesh.Vertices.Length);
        Assert.AreEqual(576, mesh.Indices.Length);
    }

    [TestMethod]
    public void Create_GeneratesUnitLengthNormals()
    {
        ToroidMesh mesh = ToroidMeshGenerator.Create(2f, 0.5f, 10, 6);

        foreach (var vertex in mesh.Vertices)
        {
            Assert.AreEqual(1f, vertex.Normal.Length(), Tolerance);
        }
    }

    [TestMethod]
    public void Create_FirstVertexMatchesExpectedOuterPoint()
    {
        ToroidMesh mesh = ToroidMeshGenerator.Create(2f, 0.5f, 16, 12);
        Vector3 position = mesh.Vertices[0].Position;

        Assert.AreEqual(2.5f, position.X, Tolerance);
        Assert.AreEqual(0f, position.Y, Tolerance);
        Assert.AreEqual(0f, position.Z, Tolerance);
    }

    [TestMethod]
    public void Create_ThrowsForInvalidParameters()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => ToroidMeshGenerator.Create(0f, 0.5f, 8, 8));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => ToroidMeshGenerator.Create(2f, -0.5f, 8, 8));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => ToroidMeshGenerator.Create(2f, 0.5f, 2, 8));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => ToroidMeshGenerator.Create(2f, 0.5f, 8, 2));
    }
}

