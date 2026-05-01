namespace TheOpenMathLibrary.ActuarialCalculators.Tests;

[TestClass]
public class ArithmeticLibraryTests
{
    private const double Tolerance = 1e-12;

    [DataTestMethod]
    [DataRow(1, 1)]
    [DataRow(6, 1)]
    [DataRow(30, -1)]
    [DataRow(12, 0)]
    public void Mobius_ReturnsExpectedValue(int number, int expected)
    {
        var actual = ArithmeticLibrary.Mobius(number);

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Mobius_ThrowsForNumbersLessThanOne()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => ArithmeticLibrary.Mobius(0));
    }

    [TestMethod]
    public void Sigma_ReturnsSumOfPositiveDivisors()
    {
        var actual = ArithmeticLibrary.Sigma(12);

        Assert.AreEqual(28, actual);
    }

    [TestMethod]
    public void Sigma_ThrowsForNumbersLessThanOne()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => ArithmeticLibrary.Sigma(0));
    }

    [DataTestMethod]
    [DataRow(1, 1)]
    [DataRow(9, 6)]
    [DataRow(10, 4)]
    public void Totient_ReturnsExpectedValue(int number, int expected)
    {
        var actual = ArithmeticLibrary.Totient(number);

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Totient_ThrowsForNumbersLessThanOne()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => ArithmeticLibrary.Totient(0));
    }

    [DataTestMethod]
    [DataRow(1, 0)]
    [DataRow(2, 1)]
    [DataRow(10, 4)]
    public void PrimeCounting_ReturnsExpectedValue(int number, int expected)
    {
        var actual = ArithmeticLibrary.PrimeCounting(number);

        Assert.AreEqual(expected, actual);
    }

    [DataTestMethod]
    [DataRow(0, 1)]
    [DataRow(5, 7)]
    [DataRow(7, 15)]
    public void Partition_ReturnsExpectedValue(int number, int expected)
    {
        var actual = ArithmeticLibrary.Partition(number);

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Partition_ThrowsForNegativeInput()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => ArithmeticLibrary.Partition(-1));
    }

    [DataTestMethod]
    [DataRow(1, 0)]
    [DataRow(12, 3)]
    [DataRow(13, 1)]
    public void Omega_ReturnsExpectedPrimeFactorMultiplicity(int number, int expected)
    {
        var actual = ArithmeticLibrary.Omega(number);

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ChebyshevTheta_ReturnsSumOfPrimeLogarithms()
    {
        var actual = ArithmeticLibrary.ChebyshevTheta(5);

        Assert.AreEqual(Math.Log(30d), actual, Tolerance);
    }

    [TestMethod]
    public void ChebyshevPsi_ReturnsSumOfPrimePowerLogarithms()
    {
        var actual = ArithmeticLibrary.ChebyshevPsi(10);

        Assert.AreEqual(Math.Log(2520d), actual, Tolerance);
    }

    [DataTestMethod]
    [DataRow(1, 1)]
    [DataRow(4, 1)]
    [DataRow(12, -1)]
    public void Liouville_ReturnsExpectedParitySign(int number, int expected)
    {
        var actual = ArithmeticLibrary.Liouville(number);

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Liouville_ThrowsForNumbersLessThanOne()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => ArithmeticLibrary.Liouville(0));
    }
}

