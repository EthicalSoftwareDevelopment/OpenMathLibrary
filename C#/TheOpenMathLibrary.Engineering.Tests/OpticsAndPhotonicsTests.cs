namespace TheOpenMathLibrary.Engineering.Tests;
[TestClass]
public class OpticsAndPhotonicsTests
{
    private const double Tolerance = 1e-12;
    [TestMethod]
    public void OpticsAndPhotonicsHelpers_ReturnExpectedValues()
    {
        Assert.AreEqual(3d, OpticsAndPhotonics.Magnification(6d, 2d), Tolerance);
        Assert.AreEqual(6d, OpticsAndPhotonics.ImageHeight(3d, 2d), Tolerance);
        Assert.AreEqual(3d, OpticsAndPhotonics.ImageDistance(2d, 6d), Tolerance);
        Assert.AreEqual(6d, OpticsAndPhotonics.PoyntingVector(2d, 3d), Tolerance);
        Assert.AreEqual(6d, OpticsAndPhotonics.PoyntingFlux(2d, 3d), Tolerance);
        Assert.AreEqual(10d / Math.Sqrt(2d), OpticsAndPhotonics.RmSElectricField(10d), Tolerance);
        Assert.AreEqual(10d / Math.Sqrt(2d), OpticsAndPhotonics.RMSElectricField(10d), Tolerance);
        Assert.AreEqual(4d, OpticsAndPhotonics.RadiationMomentum(12d, 3d), Tolerance);
        Assert.AreEqual(4d, OpticsAndPhotonics.RadiantIntensity(12d, 3d), Tolerance);
        Assert.AreEqual(81d, OpticsAndPhotonics.Radiosity(0.5d, 2d, 3d), Tolerance);
        Assert.AreEqual(4d, OpticsAndPhotonics.SpectralRadiance(12d, 3d), Tolerance);
        Assert.AreEqual(4d, OpticsAndPhotonics.SpectralIrradiance(12d, 3d), Tolerance);
        Assert.AreEqual(12.5d, OpticsAndPhotonics.EnergyDensity(3d, 4d), Tolerance);
        Assert.AreEqual(10d, OpticsAndPhotonics.KineticMomentum(5d, 2d), Tolerance);
        Assert.AreEqual(12.5d, OpticsAndPhotonics.DopplerEffect(10d, 20d, 5d), Tolerance);
        Assert.AreEqual(20d, OpticsAndPhotonics.CherenkovRadiation(30d, 1.5d), Tolerance);
        Assert.AreEqual(5d, OpticsAndPhotonics.EmWaveComponent(3d, 4d), Tolerance);
        Assert.AreEqual(5d, OpticsAndPhotonics.EMWaveComponent(3d, 4d), Tolerance);
        Assert.AreEqual(Math.PI / 6d, OpticsAndPhotonics.CriticalAngle(2d, 1d), Tolerance);
        Assert.AreEqual(0.5d, OpticsAndPhotonics.ThinLensEquation(2d, 4d, 4d), Tolerance);
        Assert.AreEqual(-3d, OpticsAndPhotonics.ImageDistancePlaneMirror(3d), Tolerance);
        Assert.AreEqual(0.5d, OpticsAndPhotonics.SphericalMirrorEquation(2d, 4d, 4d), Tolerance);
    }
    [TestMethod]
    public void OpticsAndPhotonicsHelpers_ThrowForInvalidInputs()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => OpticsAndPhotonics.Magnification(1d, 0d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => OpticsAndPhotonics.ImageDistance(2d, 2d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => OpticsAndPhotonics.RadiationMomentum(1d, 0d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => OpticsAndPhotonics.RadiantIntensity(1d, 0d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => OpticsAndPhotonics.SpectralRadiance(1d, 0d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => OpticsAndPhotonics.SpectralIrradiance(1d, 0d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => OpticsAndPhotonics.DopplerEffect(10d, 0d, 1d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => OpticsAndPhotonics.CherenkovRadiation(30d, 0d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => OpticsAndPhotonics.CriticalAngle(0d, 1d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => OpticsAndPhotonics.CriticalAngle(1d, 2d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => OpticsAndPhotonics.ThinLensEquation(0d, 2d, 3d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => OpticsAndPhotonics.SphericalMirrorEquation(2d, 0d, 3d));
    }
}
