namespace TheOpenMathLibrary.Engineering.Tests;
[TestClass]
public class DerivedDynamicQuantitiesTests
{
    private const double Tolerance = 1e-12;
    [TestMethod]
    public void DerivedDynamicQuantityHelpers_ReturnExpectedValues()
    {
        Assert.AreEqual(6d, DerivedDynamicQuantities.Momentum(2d, 3d), Tolerance);
        Assert.AreEqual(8d, DerivedDynamicQuantities.Force(2d, 4d), Tolerance);
        Assert.AreEqual(15d, DerivedDynamicQuantities.Impulse(5d, 3d), Tolerance);
        Assert.AreEqual(24d, DerivedDynamicQuantities.AngularMomentum(2d, 3d, 4d), Tolerance);
        Assert.AreEqual(10d, DerivedDynamicQuantities.Torque(5d, 2d), Tolerance);
        Assert.AreEqual(20d, DerivedDynamicQuantities.AngularImpulse(10d, 2d), Tolerance);
    }
}
