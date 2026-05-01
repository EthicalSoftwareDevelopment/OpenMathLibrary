namespace TheOpenMathLibrary.GeneralMathematics.Tests;
[TestClass]
public class WaveTheoryTests
{
    private const double Tolerance = 1e-12;
    [TestMethod]
    public void WaveRelationships_ReturnExpectedValues()
    {
        Assert.AreEqual(5d, WaveTheory.Wavelength(10d, 2d), Tolerance);
        Assert.AreEqual(2d * Math.PI * 2d / 10d, WaveTheory.WaveVector(10d, 2d), Tolerance);
        Assert.AreEqual(2d, WaveTheory.Frequency(10d, 5d), Tolerance);
        Assert.AreEqual(4d * Math.PI, WaveTheory.AngularFrequency(2d), Tolerance);
    }
    [TestMethod]
    public void OscillationAndPhaseHelpers_ReturnExpectedValues()
    {
        Assert.AreEqual(2d, WaveTheory.OscillatoryVelocity(2d, 3d, Math.PI / 6d, 0d), Tolerance);
        Assert.AreEqual(6d, WaveTheory.OscillatoryAcceleration(2d, 3d, 0d, 0d), Tolerance);
        Assert.AreEqual(5d, WaveTheory.PhaseVelocity(10d, 2d), Tolerance);
        Assert.AreEqual(5d, WaveTheory.GroupVelocity(10d, 2d), Tolerance);
        Assert.AreEqual(4d, WaveTheory.TimeDelay(5d, 1d), Tolerance);
        Assert.AreEqual(1d, WaveTheory.PhaseDifference(1.5d, 0.5d), Tolerance);
        Assert.AreEqual(6d - 2d * Math.PI + 0.5d, WaveTheory.Phase(2d, 3d, 1d, 0.5d), Tolerance);
    }
    [TestMethod]
    public void WaveRelationships_ThrowForZeroDivisors()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => WaveTheory.Wavelength(10d, 0d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => WaveTheory.WaveVector(0d, 2d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => WaveTheory.Frequency(10d, 0d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => WaveTheory.PhaseVelocity(10d, 0d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => WaveTheory.GroupVelocity(10d, 0d));
    }
}
