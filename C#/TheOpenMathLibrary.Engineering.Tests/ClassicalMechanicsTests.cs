namespace TheOpenMathLibrary.Engineering.Tests;
[TestClass]
public class ClassicalMechanicsTests
{
    private const double Tolerance = 1e-12;
    [TestMethod]
    public void ClassicalMechanicsHelpers_ReturnExpectedValues()
    {
        Assert.AreEqual(5d, ClassicalMechanics.LinearMassDensity(10d, 2d), Tolerance);
        Assert.AreEqual(2d, ClassicalMechanics.SurfaceMassDensity(4d, 2d), Tolerance);
        Assert.AreEqual(4d, ClassicalMechanics.VolumetricMassDensity(8d, 2d), Tolerance);
        Assert.AreEqual(18d, ClassicalMechanics.MomentOfMass(2d, 3d), Tolerance);
        Assert.AreEqual(3.4d, ClassicalMechanics.CenterOfMass(2d, 3d, 1d, 5d), Tolerance);
        Assert.AreEqual(1.2d, ClassicalMechanics.ReducedMass(2d, 3d), Tolerance);
        Assert.AreEqual(18d, ClassicalMechanics.MomentOfInertia(2d, 3d), Tolerance);
    }
    [TestMethod]
    public void ClassicalMechanicsHelpers_ThrowForZeroDivisors()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => ClassicalMechanics.LinearMassDensity(1d, 0d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => ClassicalMechanics.SurfaceMassDensity(1d, 0d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => ClassicalMechanics.VolumetricMassDensity(1d, 0d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => ClassicalMechanics.CenterOfMass(1d, -1d, 0d, 1d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => ClassicalMechanics.ReducedMass(1d, -1d));
    }
}
