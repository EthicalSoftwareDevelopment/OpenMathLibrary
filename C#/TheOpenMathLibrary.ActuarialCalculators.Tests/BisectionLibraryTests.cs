namespace TheOpenMathLibrary.ActuarialCalculators.Tests;

[TestClass]
public class BisectionLibraryTests
{
    private const double Tolerance = 1e-9;

    [TestMethod]
    public void Bisection_FindsRootWithinInterval()
    {
        var actual = BisectionLibrary.Bisection(1d, 2d, 1e-10, 100, value => value * value - 2d);

        Assert.AreEqual(Math.Sqrt(2d), actual, Tolerance);
    }

    [TestMethod]
    public void Bisection_ReturnsEndpointWhenAlreadyWithinTolerance()
    {
        var actual = BisectionLibrary.Bisection(0d, 2d, 1e-10, 100, value => value);

        Assert.AreEqual(0d, actual, Tolerance);
    }

    [TestMethod]
    public void Bisection_ThrowsWhenIntervalDoesNotBracketRoot()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            BisectionLibrary.Bisection(2d, 4d, 1e-6, 10, value => value * value + 1d));
    }

    [TestMethod]
    public void Bisection_ThrowsForInvalidTolerance()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            BisectionLibrary.Bisection(0d, 1d, 0d, 10, value => value));
    }

    [TestMethod]
    public void Bisection_ThrowsForInvalidIterationCount()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            BisectionLibrary.Bisection(0d, 1d, 1e-6, 0, value => value));
    }

    [TestMethod]
    public void Bisection_ThrowsWhenFunctionIsNull()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            BisectionLibrary.Bisection(0d, 1d, 1e-6, 10, null!));
    }

    [TestMethod]
    public void NewtonRaphson_FindsRootNearInitialEstimate()
    {
        var actual = BisectionLibrary.NewtonRaphson(1d, 1e-10, 50, value => value * value - 2d, value => 2d * value);

        Assert.AreEqual(Math.Sqrt(2d), actual, Tolerance);
    }

    [TestMethod]
    public void NewtonRaphson_ThrowsForNullFunction()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            BisectionLibrary.NewtonRaphson(1d, 1e-6, 10, null!, value => 2d * value));
    }

    [TestMethod]
    public void NewtonRaphson_ThrowsForNullDerivative()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            BisectionLibrary.NewtonRaphson(1d, 1e-6, 10, value => value, null!));
    }

    [TestMethod]
    public void NewtonRaphson_ThrowsWhenDerivativeEvaluatesToZero()
    {
        Assert.ThrowsException<InvalidOperationException>(() =>
            BisectionLibrary.NewtonRaphson(1d, 1e-6, 10, value => value * value + 1d, _ => 0d));
    }

    [TestMethod]
    public void Binomial_MatchesBisectionImplementation()
    {
        var bisection = BisectionLibrary.Bisection(1d, 2d, 1e-10, 100, value => value * value - 2d);
        var binomial = BisectionLibrary.Binomial(1d, 2d, 1e-10, 100, value => value * value - 2d);

        Assert.AreEqual(bisection, binomial, Tolerance);
    }
}

