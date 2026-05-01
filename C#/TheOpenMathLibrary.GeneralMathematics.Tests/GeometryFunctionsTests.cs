namespace TheOpenMathLibrary.GeneralMathematics.Tests;
[TestClass]
public class GeometryFunctionsTests
{
    private const double Tolerance = 1e-12;
    [TestMethod]
    public void GeometryFormulas_ReturnExpectedValues()
    {
        Assert.AreEqual(6d, GeometryFunctions.TriangleArea(3d, 4d), Tolerance);
        Assert.AreEqual(12d * Math.PI, GeometryFunctions.CylinderVolume(2d, 3d), Tolerance);
        Assert.AreEqual(32d / 3d * Math.PI, GeometryFunctions.SphereVolume(2d), Tolerance);
        Assert.AreEqual(24d, GeometryFunctions.RectangularPrismVolume(2d, 3d, 4d), Tolerance);
    }
    [TestMethod]
    public void GeometryFormulas_ThrowForNegativeDimensions()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => GeometryFunctions.TriangleArea(-1d, 2d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => GeometryFunctions.CylinderVolume(1d, -2d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => GeometryFunctions.SphereVolume(-1d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => GeometryFunctions.RectangularPrismVolume(1d, 2d, -3d));
    }
}
