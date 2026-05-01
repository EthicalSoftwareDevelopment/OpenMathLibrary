namespace TheOpenMathLibrary.Engineering.Tests;
[TestClass]
public class AngularFrequenciesTests
{
    private const double Tolerance = 1e-12;
    [TestMethod]
    public void AngularFrequencyHelpers_ReturnExpectedValues()
    {
        Assert.AreEqual(3d, AngularFrequencies.LinearUndampedUnforcedOscillator(3d), Tolerance);
        Assert.AreEqual(6.4d, AngularFrequencies.LinearUnforcedDHO(8d, 0.6d), Tolerance);
        Assert.AreEqual(6d, AngularFrequencies.LowAmplitudeAngularSHO(3d, 2d), Tolerance);
        Assert.AreEqual(6d, AngularFrequencies.LowAmplitudeSimplePendulum(3d, 2d), Tolerance);
    }
    [TestMethod]
    public void LinearUnforcedDHO_ThrowsForInvalidDampingCoefficient()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => AngularFrequencies.LinearUnforcedDHO(8d, 1.1d));
    }
}
