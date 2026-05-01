namespace TheOpenMathLibrary.ActuarialCalculators.Tests;

[TestClass]
public class PiecewiseSpecialFunctionsLibraryTests
{
    private const double Tolerance = 1e-12;

    [DataTestMethod]
    [DataRow(0d, 1d)]
    [DataRow(-1d, 0d)]
    [DataRow(2d, 0d)]
    public void IndicatorFunction_ReturnsExpectedValue(double x, double expected)
    {
        var actual = PiecewiseSpecialFunctionsLibrary.IndicatorFunction(x);

        Assert.AreEqual(expected, actual, Tolerance);
    }

    [DataTestMethod]
    [DataRow(-1d, 0d)]
    [DataRow(0d, 0.5d)]
    [DataRow(3d, 1d)]
    public void StepFunction_ReturnsExpectedValue(double x, double expected)
    {
        var actual = PiecewiseSpecialFunctionsLibrary.StepFunction(x);

        Assert.AreEqual(expected, actual, Tolerance);
    }

    [DataTestMethod]
    [DataRow(-1d, 0d)]
    [DataRow(0d, 1d)]
    [DataRow(2d, 1d)]
    public void HeavisideStepFunction_ReturnsExpectedValue(double x, double expected)
    {
        var actual = PiecewiseSpecialFunctionsLibrary.HeavisideStepFunction(x);

        Assert.AreEqual(expected, actual, Tolerance);
    }

    [DataTestMethod]
    [DataRow(-1d, 0d)]
    [DataRow(-0.5d, 0.5d)]
    [DataRow(0d, 1d)]
    [DataRow(0.5d, 0.5d)]
    [DataRow(1d, 0d)]
    public void RectangularFunction_ReturnsExpectedValue(double x, double expected)
    {
        var actual = PiecewiseSpecialFunctionsLibrary.RectangularFunction(x);

        Assert.AreEqual(expected, actual, Tolerance);
    }

    [DataTestMethod]
    [DataRow(1.25d, 0.25d)]
    [DataRow(-0.25d, 0.75d)]
    public void SawtoothFunction_ReturnsExpectedValue(double x, double expected)
    {
        var actual = PiecewiseSpecialFunctionsLibrary.SawtoothFunction(x);

        Assert.AreEqual(expected, actual, Tolerance);
    }

    [DataTestMethod]
    [DataRow(0d, 1d)]
    [DataRow(0.25d, 0.5d)]
    [DataRow(0.5d, 0d)]
    public void TriangleWaveFunction_ReturnsExpectedValue(double x, double expected)
    {
        var actual = PiecewiseSpecialFunctionsLibrary.TriangleWaveFunction(x);

        Assert.AreEqual(expected, actual, Tolerance);
    }

    [DataTestMethod]
    [DataRow(0.25d, 0d)]
    [DataRow(0.5d, 1d)]
    [DataRow(0.75d, 1d)]
    public void SquareWaveFunction_ReturnsExpectedValue(double x, double expected)
    {
        var actual = PiecewiseSpecialFunctionsLibrary.SquareWaveFunction(x);

        Assert.AreEqual(expected, actual, Tolerance);
    }

    [TestMethod]
    public void SincFunction_ReturnsOneAtOrigin()
    {
        var actual = PiecewiseSpecialFunctionsLibrary.SincFunction(0d);

        Assert.AreEqual(1d, actual, Tolerance);
    }

    [TestMethod]
    public void SincFunction_ReturnsExpectedValueAwayFromOrigin()
    {
        var actual = PiecewiseSpecialFunctionsLibrary.SincFunction(Math.PI);

        Assert.AreEqual(0d, actual, Tolerance);
    }

    [TestMethod]
    public void DirichletKernel_ReturnsCentralLimitAtOrigin()
    {
        var actual = PiecewiseSpecialFunctionsLibrary.DirichletKernel(0d, 2);

        Assert.AreEqual(5d, actual, Tolerance);
    }

    [TestMethod]
    public void DirichletKernel_ReturnsExpectedValueAwayFromOrigin()
    {
        var actual = PiecewiseSpecialFunctionsLibrary.DirichletKernel(Math.PI / 2d, 1);

        Assert.AreEqual(1d, actual, Tolerance);
    }

    [TestMethod]
    public void DirichletKernel_ThrowsForNegativeOrder()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            PiecewiseSpecialFunctionsLibrary.DirichletKernel(0d, -1));
    }
}

