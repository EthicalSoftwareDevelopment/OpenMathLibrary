namespace TheOpenMathLibrary.GeneralMathematics.Tests;
[TestClass]
public class TrigonometryLibraryTests
{
    private const double Tolerance = 1e-12;
    [TestMethod]
    public void PrimaryTrigonometricFunctions_ReturnExpectedValues()
    {
        const double angle = Math.PI / 4d;
        Assert.AreEqual(Math.Sqrt(2d) / 2d, TrigonometryLibrary.Sine(angle), Tolerance);
        Assert.AreEqual(Math.Sqrt(2d) / 2d, TrigonometryLibrary.Cosine(angle), Tolerance);
        Assert.AreEqual(1d, TrigonometryLibrary.Tangent(angle), Tolerance);
        Assert.AreEqual(1d, TrigonometryLibrary.Cotangent(angle), Tolerance);
        Assert.AreEqual(Math.Sqrt(2d), TrigonometryLibrary.Secant(angle), Tolerance);
        Assert.AreEqual(Math.Sqrt(2d), TrigonometryLibrary.Cosecant(angle), Tolerance);
    }
    [TestMethod]
    public void ReciprocalFunctions_ThrowAtUndefinedPoints()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => TrigonometryLibrary.Cotangent(0d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => TrigonometryLibrary.Secant(Math.PI / 2d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => TrigonometryLibrary.Cosecant(0d));
    }
    [TestMethod]
    public void DerivedTrigonometricFunctions_ReturnExpectedValues()
    {
        const double angle = Math.PI / 3d;
        var sine = Math.Sin(angle);
        var cosine = Math.Cos(angle);
        Assert.AreEqual(1d, TrigonometryLibrary.Exsecant(angle), Tolerance);
        Assert.AreEqual((1d / sine) - 1d, TrigonometryLibrary.Excosecant(angle), Tolerance);
        Assert.AreEqual(1d - cosine, TrigonometryLibrary.Versine(angle), Tolerance);
        Assert.AreEqual(1d - sine, TrigonometryLibrary.Coversine(angle), Tolerance);
        Assert.AreEqual(1d + cosine, TrigonometryLibrary.Vercosine(angle), Tolerance);
        Assert.AreEqual(1d + sine, TrigonometryLibrary.Covercosine(angle), Tolerance);
        Assert.AreEqual(0.5d * (1d - cosine), TrigonometryLibrary.Haversine(angle), Tolerance);
        Assert.AreEqual(0.5d * (1d - sine), TrigonometryLibrary.Hacoversine(angle), Tolerance);
        Assert.AreEqual(0.5d * (1d + cosine), TrigonometryLibrary.Havercosine(angle), Tolerance);
        Assert.AreEqual(0.5d * (1d + sine), TrigonometryLibrary.Hacovercosine(angle), Tolerance);
    }
}
