namespace TheOpenMathLibrary.Engineering.Tests;
[TestClass]
public class KinematicsTests
{
    private const double Tolerance = 1e-12;
    [TestMethod]
    public void KinematicsHelpers_ReturnExpectedValues()
    {
        Assert.AreEqual(5d, Kinematics.Velocity(10d, 2d), Tolerance);
        Assert.AreEqual(4d, Kinematics.Acceleration(8d, 2d), Tolerance);
        Assert.AreEqual(3d, Kinematics.Jerk(6d, 2d), Tolerance);
        Assert.AreEqual(2d, Kinematics.Jounce(4d, 2d), Tolerance);
        Assert.AreEqual(6d, Kinematics.AngularAcceleration(12d, 2d), Tolerance);
        Assert.AreEqual(7d, Kinematics.AngularJerk(14d, 2d), Tolerance);
    }
    [TestMethod]
    public void KinematicsHelpers_ThrowForZeroTime()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => Kinematics.Velocity(1d, 0d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => Kinematics.Acceleration(1d, 0d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => Kinematics.Jerk(1d, 0d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => Kinematics.Jounce(1d, 0d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => Kinematics.AngularAcceleration(1d, 0d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => Kinematics.AngularJerk(1d, 0d));
    }
}
