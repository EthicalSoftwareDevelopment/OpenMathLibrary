namespace TheOpenMathLibrary.Engineering.Tests;
[TestClass]
public class MechanicalOscillationsTests
{
    private const double Tolerance = 1e-12;
    [TestMethod]
    public void MechanicalOscillationHelpers_ReturnExpectedValuesAndAliases()
    {
        var expected = 0.5d * 2d * 3d * 3d * 4d * 4d;
        Assert.AreEqual(expected, MechanicalOscillations.ShmEnergy(2d, 3d, 4d), Tolerance);
        Assert.AreEqual(expected, MechanicalOscillations.SHMEnergy(2d, 3d, 4d), Tolerance);
        Assert.AreEqual(expected, MechanicalOscillations.DhmEnergy(2d, 3d, 4d), Tolerance);
        Assert.AreEqual(expected, MechanicalOscillations.DHMEnergy(2d, 3d, 4d), Tolerance);
    }
}
