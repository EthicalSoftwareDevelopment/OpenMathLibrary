namespace TheOpenMathLibrary.Engineering.Tests;
[TestClass]
public class FluidDynamicsTests
{
    private const double Tolerance = 1e-12;
    [TestMethod]
    public void FluidDynamicsHelpers_ReturnExpectedValues()
    {
        Assert.AreEqual(10d / Math.PI, FluidDynamics.FlowVelocity(10d, 2d), Tolerance);
        Assert.AreEqual(6d, FluidDynamics.VelocityPseudovector(3d, 2d), Tolerance);
        Assert.AreEqual(3d / Math.PI, FluidDynamics.VolumeFlux(12d, 2d), Tolerance);
        Assert.AreEqual(5d, FluidDynamics.MassCurrentPerVolume(10d, 2d), Tolerance);
        Assert.AreEqual(6d, FluidDynamics.MassFlowRate(2d, 3d), Tolerance);
        Assert.AreEqual(6d, FluidDynamics.MassCurrentDensity(2d, 3d), Tolerance);
        Assert.AreEqual(18d, FluidDynamics.MomentumCurrentDensity(2d, 3d), Tolerance);
        Assert.AreEqual(5d, FluidDynamics.PressureGradient(10d, 2d), Tolerance);
        Assert.AreEqual(24d, FluidDynamics.BuoyancyForce(2d, 3d, 4d), Tolerance);
        Assert.AreEqual(59d, FluidDynamics.BernoullisEquation(1d, 2d, 3d, 4d, 5d), Tolerance);
        Assert.AreEqual(59d, FluidDynamics.EulersEquations(1d, 2d, 3d, 4d, 5d), Tolerance);
        Assert.AreEqual(12d, FluidDynamics.ConvectiveAcceleration(3d, 4d), Tolerance);
        Assert.AreEqual(59d, FluidDynamics.NavierStokesEquations(1d, 2d, 3d, 4d, 5d), Tolerance);
    }
    [TestMethod]
    public void FluidDynamicsHelpers_ThrowForZeroDivisors()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => FluidDynamics.FlowVelocity(10d, 0d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => FluidDynamics.VolumeFlux(12d, 0d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => FluidDynamics.MassCurrentPerVolume(10d, 0d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => FluidDynamics.PressureGradient(10d, 0d));
    }
}
