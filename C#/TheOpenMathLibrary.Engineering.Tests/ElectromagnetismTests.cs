namespace TheOpenMathLibrary.Engineering.Tests;
[TestClass]
public class ElectromagnetismTests
{
    private const double Tolerance = 1e-12;
    [TestMethod]
    public void ElectromagneticHelpers_ReturnExpectedValues()
    {
        Assert.AreEqual(5d, Electromagnetism.ElectricFieldPotentialGradient(10d, 2d), Tolerance);
        Assert.AreEqual(6d, Electromagnetism.ElectricFluxDensity(2d, 3d), Tolerance);
        Assert.AreEqual(4d, Electromagnetism.AbsolutePermittivity(8d, 2d), Tolerance);
        Assert.AreEqual(6d, Electromagnetism.ElectricDipoleMoment(2d, 3d), Tolerance);
        Assert.AreEqual(3d, Electromagnetism.ElectricPolarization(6d, 2d), Tolerance);
        Assert.AreEqual(3d, Electromagnetism.ElectricDisplacementField(6d, 2d), Tolerance);
        Assert.AreEqual(12d, Electromagnetism.ElectricDisplacementFlux(4d, 3d), Tolerance);
        Assert.AreEqual(10d, Electromagnetism.AbsoluteElectricPotential(5d, 2d), Tolerance);
    }
    [TestMethod]
    public void ElectroMagnetismAlias_RemainsBackwardCompatible()
    {
        Assert.AreEqual(10d, ElectroMagnetism.AbsoluteElectricPotential(5d, 2d), Tolerance);
    }
    [TestMethod]
    public void ElectromagneticHelpers_ThrowForZeroDivisors()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => Electromagnetism.ElectricFieldPotentialGradient(1d, 0d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => Electromagnetism.AbsolutePermittivity(1d, 0d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => Electromagnetism.ElectricPolarization(1d, 0d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => Electromagnetism.ElectricDisplacementField(1d, 0d));
    }
}
