namespace TheOpenMathLibrary.Engineering.Tests;
[TestClass]
public class GeneralEnergyDefinitionsTests
{
    private const double Tolerance = 1e-12;
    [TestMethod]
    public void GeneralEnergyHelpers_ReturnExpectedValues()
    {
        Assert.AreEqual(10d, GeneralEnergyDefinitions.MechanicalWork(5d, 2d, 0d), Tolerance);
        Assert.AreEqual(10d, GeneralEnergyDefinitions.WorkDoneOnMechanicalSystem(5d, 2d, 0d), Tolerance);
        Assert.AreEqual(24d, GeneralEnergyDefinitions.PotentialEnergy(2d, 3d, 4d), Tolerance);
        Assert.AreEqual(15d, GeneralEnergyDefinitions.MechanicalEnergy(6d, 9d), Tolerance);
        Assert.AreEqual(20d, GeneralEnergyDefinitions.MechanicalPower(5d, 4d), Tolerance);
    }
}
