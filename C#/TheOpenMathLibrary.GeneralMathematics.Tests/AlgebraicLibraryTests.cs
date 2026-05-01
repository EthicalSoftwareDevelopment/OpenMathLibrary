namespace TheOpenMathLibrary.GeneralMathematics.Tests;
[TestClass]
public class AlgebraicLibraryTests
{
    private const double Tolerance = 1e-12;
    [TestMethod]
    public void Linear_ReturnsInput()
    {
        var actual = AlgebraicLibrary.Linear(3.5d);
        Assert.AreEqual(3.5d, actual, Tolerance);
    }
    [TestMethod]
    public void Cubic_ReturnsThirdPower()
    {
        var actual = AlgebraicLibrary.Cubic(-2d);
        Assert.AreEqual(-8d, actual, Tolerance);
    }
    [TestMethod]
    public void Quartic_ReturnsFourthPower()
    {
        var actual = AlgebraicLibrary.Quartic(3d);
        Assert.AreEqual(81d, actual, Tolerance);
    }
    [TestMethod]
    public void Quintic_ReturnsFifthPower()
    {
        var actual = AlgebraicLibrary.Quintic(2d);
        Assert.AreEqual(32d, actual, Tolerance);
    }
    [TestMethod]
    public void SquareRoot_ReturnsPrincipalRoot()
    {
        var actual = AlgebraicLibrary.SquareRoot(49d);
        Assert.AreEqual(7d, actual, Tolerance);
    }
    [TestMethod]
    public void SquareRoot_ThrowsForNegativeInput()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => AlgebraicLibrary.SquareRoot(-1d));
    }
    [TestMethod]
    public void CubeRoot_ReturnsRealCubeRoot()
    {
        var actual = AlgebraicLibrary.CubeRoot(-27d);
        Assert.AreEqual(-3d, actual, Tolerance);
    }
}
