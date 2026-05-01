namespace TheOpenMathLibrary.Engineering.Tests;
[TestClass]
public class CivilEngineeringLibraryTests
{
    private const double Tolerance = 1e-12;
    [TestMethod]
    public void MaterialAndSlopeHelpers_ReturnExpectedValues()
    {
        Assert.AreEqual(2d, CivilEngineeringLibrary.CementQuantity(10d, 4d), Tolerance);
        Assert.AreEqual(8d, CivilEngineeringLibrary.SandQuantity(10d, 4d), Tolerance);
        Assert.AreEqual(8d, CivilEngineeringLibrary.AggregateQuantity(10d, 4d), Tolerance);
        Assert.AreEqual(50d, CivilEngineeringLibrary.SlopeAsPercentage(2d, 4d), Tolerance);
        Assert.AreEqual(0.5d, CivilEngineeringLibrary.SlopeAsRatio(2d, 4d), Tolerance);
        Assert.AreEqual(6d, CivilEngineeringLibrary.EarthworkVolume(3d, 2d), Tolerance);
        Assert.AreEqual(3d, CivilEngineeringLibrary.AverageCrossSectionalArea(2d, 4d), Tolerance);
        Assert.AreEqual(4d, CivilEngineeringLibrary.SteelQuantity(20d, 5d), Tolerance);
        Assert.AreEqual(0.6165d, CivilEngineeringLibrary.WeightOfSteelPerUnitLength(10d), Tolerance);
    }
    [TestMethod]
    public void UnitWeightsAndLoads_ReturnExpectedValues()
    {
        Assert.AreEqual(7850d, CivilEngineeringLibrary.UnitWeightOfSteel(), Tolerance);
        Assert.AreEqual(2400d, CivilEngineeringLibrary.UnitWeightOfConcrete(), Tolerance);
        Assert.AreEqual(1920d, CivilEngineeringLibrary.UnitWeightOfBrick(), Tolerance);
        Assert.AreEqual(1000d, CivilEngineeringLibrary.UnitWeightOfWater(), Tolerance);
        Assert.AreEqual(12d, CivilEngineeringLibrary.LoadBearingCapacity(3d, 4d), Tolerance);
        Assert.AreEqual(5d, CivilEngineeringLibrary.SlabLoad(2d, 3d), Tolerance);
    }
    [TestMethod]
    public void StructuralAndVolumeHelpers_ReturnExpectedValues()
    {
        Assert.AreEqual(8d / 3d, CivilEngineeringLibrary.CantileverBeamDeflection(12d, 2d, 3d, 4d), Tolerance);
        Assert.AreEqual(4.5d, CivilEngineeringLibrary.MomentOfInertiaOfRectangularSection(2d, 3d), Tolerance);
        Assert.AreEqual(Math.PI / 4d, CivilEngineeringLibrary.MomentOfInertiaOfCircularSection(2d), Tolerance);
        Assert.AreEqual(10d, CivilEngineeringLibrary.BendingMoment(5d, 2d), Tolerance);
        Assert.AreEqual(5d, CivilEngineeringLibrary.ShearForce(10d, 2d), Tolerance);
        Assert.AreEqual(24d, CivilEngineeringLibrary.BricksCalculation(2d, 3d, 4d), Tolerance);
        Assert.AreEqual(8d, CivilEngineeringLibrary.DryMaterialQuantityForMortar(10d, 4d), Tolerance);
        Assert.AreEqual(2d, CivilEngineeringLibrary.WetMortarVolume(10d, 4d), Tolerance);
        Assert.AreEqual(24d, CivilEngineeringLibrary.ExcavationCalculation(2d, 3d, 4d), Tolerance);
        Assert.AreEqual(6d, CivilEngineeringLibrary.RetainingWallStability(4d, 3d, 2d, Math.PI / 6d), Tolerance);
        Assert.AreEqual(2d, CivilEngineeringLibrary.OneWaySlabThickness(2d, 4d, 2d), Tolerance);
        Assert.AreEqual(4d / 3d, CivilEngineeringLibrary.TwoWaySlabThickness(2d, 4d, 2d), Tolerance);
        Assert.AreEqual(2d, CivilEngineeringLibrary.CompactionFactor(10d, 5d), Tolerance);
        Assert.AreEqual(3d, CivilEngineeringLibrary.SoilSettlement(10d, 7d), Tolerance);
    }
    [TestMethod]
    public void CivilEngineeringGuards_ThrowForInvalidInputs()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => CivilEngineeringLibrary.CementQuantity(10d, -1d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => CivilEngineeringLibrary.SlopeAsPercentage(1d, 0d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => CivilEngineeringLibrary.SlopeAsRatio(1d, 0d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => CivilEngineeringLibrary.SteelQuantity(10d, 0d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => CivilEngineeringLibrary.CantileverBeamDeflection(10d, 2d, 0d, 4d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => CivilEngineeringLibrary.CantileverBeamDeflection(10d, 2d, 3d, 0d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => CivilEngineeringLibrary.ShearForce(10d, 0d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => CivilEngineeringLibrary.DryMaterialQuantityForMortar(10d, -1d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => CivilEngineeringLibrary.WetMortarVolume(10d, -1d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => CivilEngineeringLibrary.OneWaySlabThickness(2d, 4d, 0d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => CivilEngineeringLibrary.TwoWaySlabThickness(2d, 4d, 0d));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => CivilEngineeringLibrary.CompactionFactor(10d, 0d));
    }
}
