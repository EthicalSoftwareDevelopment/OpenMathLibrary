namespace TheOpenMathLibrary.ActuarialCalculators.Tests;

[TestClass]
public class TranscendentalLibraryTests
{
    private const double Tolerance = 1e-12;

    [TestMethod]
    public void Exponential_RaisesValueToPower()
    {
        var actual = TranscendentalLibrary.Exponential(3d, 4d);

        Assert.AreEqual(81d, actual, Tolerance);
    }

    [TestMethod]
    public void HyperbolicSine_ReturnsExpectedValue()
    {
        var actual = TranscendentalLibrary.HyperbolicSine(1.5d);

        Assert.AreEqual(Math.Sinh(1.5d), actual, Tolerance);
    }

    [TestMethod]
    public void Logarithm_ReturnsNaturalLogarithm()
    {
        var actual = TranscendentalLibrary.Logarithm(Math.E * Math.E);

        Assert.AreEqual(2d, actual, Tolerance);
    }

    [TestMethod]
    public void Logarithm_ThrowsForNonPositiveInput()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => TranscendentalLibrary.Logarithm(0d));
    }

    [TestMethod]
    public void CommonLogarithm10_ReturnsBase10Logarithm()
    {
        var actual = TranscendentalLibrary.CommonLogarithm10(1000d);

        Assert.AreEqual(3d, actual, Tolerance);
    }

    [TestMethod]
    public void CommonLogarithm10_ThrowsForNonPositiveInput()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => TranscendentalLibrary.CommonLogarithm10(-1d));
    }

    [TestMethod]
    public void BinaryLogarithm2_ReturnsBase2Logarithm()
    {
        var actual = TranscendentalLibrary.BinaryLogarithm2(8d);

        Assert.AreEqual(3d, actual, Tolerance);
    }

    [TestMethod]
    public void BinaryLogarithm2_ThrowsForNonPositiveInput()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => TranscendentalLibrary.BinaryLogarithm2(0d));
    }
}

