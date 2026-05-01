namespace TheOpenMathLibrary.Engineering
{
    /// <summary>
    /// Provides helper formulas for introductory civil engineering calculations.
    /// </summary>
    public class CivilEngineeringLibrary
    {
        /// <summary>
        /// Calculates the cement portion of a mix by ratio.
        /// </summary>
        public static double CementQuantity(double volume, double ratio)
        {
            return volume / ValidateMixDenominator(ratio, nameof(ratio));
        }

        /// <summary>
        /// Calculates the sand portion of a mix by ratio.
        /// </summary>
        public static double SandQuantity(double volume, double ratio)
        {
            return volume * ratio / ValidateMixDenominator(ratio, nameof(ratio));
        }

        /// <summary>
        /// Calculates the aggregate portion of a mix by ratio.
        /// </summary>
        public static double AggregateQuantity(double volume, double ratio)
        {
            return volume * ratio / ValidateMixDenominator(ratio, nameof(ratio));
        }

        /// <summary>
        /// Calculates slope as a percentage.
        /// </summary>
        public static double SlopeAsPercentage(double rise, double run)
        {
            EnsureNonZero(run, nameof(run));
            return rise / run * 100d;
        }

        /// <summary>
        /// Calculates slope as a ratio.
        /// </summary>
        public static double SlopeAsRatio(double rise, double run)
        {
            EnsureNonZero(run, nameof(run));
            return rise / run;
        }

        /// <summary>
        /// Calculates earthwork volume from cross-sectional area and depth.
        /// </summary>
        public static double EarthworkVolume(double area, double depth)
        {
            return area * depth;
        }

        /// <summary>
        /// Calculates the average of two cross-sectional areas.
        /// </summary>
        public static double AverageCrossSectionalArea(double area1, double area2)
        {
            return (area1 + area2) / 2d;
        }

        /// <summary>
        /// Calculates steel quantity from area and spacing.
        /// </summary>
        public static double SteelQuantity(double area, double spacing)
        {
            EnsureNonZero(spacing, nameof(spacing));
            return area / spacing;
        }

        /// <summary>
        /// Calculates the weight of steel per unit length from bar diameter.
        /// </summary>
        public static double WeightOfSteelPerUnitLength(double diameter)
        {
            return 0.006165d * diameter * diameter;
        }

        /// <summary>
        /// Gets the nominal unit weight of steel in kilograms per cubic meter.
        /// </summary>
        public static double UnitWeightOfSteel()
        {
            return 7850d;
        }

        /// <summary>
        /// Gets the nominal unit weight of concrete in kilograms per cubic meter.
        /// </summary>
        public static double UnitWeightOfConcrete()
        {
            return 2400d;
        }

        /// <summary>
        /// Gets the nominal unit weight of brick in kilograms per cubic meter.
        /// </summary>
        public static double UnitWeightOfBrick()
        {
            return 1920d;
        }

        /// <summary>
        /// Gets the nominal unit weight of water in kilograms per cubic meter.
        /// </summary>
        public static double UnitWeightOfWater()
        {
            return 1000d;
        }

        /// <summary>
        /// Calculates load-bearing capacity from area and unit weight.
        /// </summary>
        public static double LoadBearingCapacity(double area, double unitWeight)
        {
            return area * unitWeight;
        }

        /// <summary>
        /// Calculates slab load from live and dead loads.
        /// </summary>
        public static double SlabLoad(double liveLoad, double deadLoad)
        {
            return liveLoad + deadLoad;
        }

        /// <summary>
        /// Calculates cantilever beam deflection under end loading.
        /// </summary>
        public static double CantileverBeamDeflection(double load, double length, double modulus, double inertia)
        {
            EnsureNonZero(modulus, nameof(modulus));
            EnsureNonZero(inertia, nameof(inertia));
            return load * length * length * length / (3d * modulus * inertia);
        }

        /// <summary>
        /// Calculates the second moment of area of a rectangular section.
        /// </summary>
        public static double MomentOfInertiaOfRectangularSection(double baseAmount, double height)
        {
            return baseAmount * height * height * height / 12d;
        }

        /// <summary>
        /// Calculates the second moment of area of a circular section.
        /// </summary>
        public static double MomentOfInertiaOfCircularSection(double diameter)
        {
            return Math.PI * diameter * diameter * diameter * diameter / 64d;
        }

        /// <summary>
        /// Calculates bending moment.
        /// </summary>
        public static double BendingMoment(double force, double distance)
        {
            return force * distance;
        }

        /// <summary>
        /// Calculates shear force as force distributed over distance.
        /// </summary>
        public static double ShearForce(double force, double distance)
        {
            EnsureNonZero(distance, nameof(distance));
            return force / distance;
        }

        /// <summary>
        /// Calculates a simple brickwork volume measure.
        /// </summary>
        public static double BricksCalculation(double length, double width, double height)
        {
            return length * width * height;
        }

        /// <summary>
        /// Calculates dry material quantity for mortar by ratio.
        /// </summary>
        public static double DryMaterialQuantityForMortar(double volume, double ratio)
        {
            return volume * ratio / ValidateMixDenominator(ratio, nameof(ratio));
        }

        /// <summary>
        /// Calculates wet mortar volume by ratio.
        /// </summary>
        public static double WetMortarVolume(double volume, double ratio)
        {
            return volume / ValidateMixDenominator(ratio, nameof(ratio));
        }

        /// <summary>
        /// Calculates excavation volume.
        /// </summary>
        public static double ExcavationCalculation(double length, double width, double depth)
        {
            return length * width * depth;
        }

        /// <summary>
        /// Calculates a retaining-wall stability estimate.
        /// </summary>
        public static double RetainingWallStability(double height, double width, double density, double angle)
        {
            return height * width * density * 0.5d * Math.Sin(angle);
        }

        /// <summary>
        /// Calculates one-way slab thickness using a simplified load expression.
        /// </summary>
        public static double OneWaySlabThickness(double span, double load, double factor)
        {
            EnsureNonZero(factor, nameof(factor));
            return span * span * span * load / (8d * factor);
        }

        /// <summary>
        /// Calculates two-way slab thickness using a simplified load expression.
        /// </summary>
        public static double TwoWaySlabThickness(double span, double load, double factor)
        {
            EnsureNonZero(factor, nameof(factor));
            return span * span * span * load / (12d * factor);
        }

        /// <summary>
        /// Calculates compaction factor.
        /// </summary>
        public static double CompactionFactor(double initialVolume, double finalVolume)
        {
            EnsureNonZero(finalVolume, nameof(finalVolume));
            return initialVolume / finalVolume;
        }

        /// <summary>
        /// Calculates soil settlement from initial and final volumes.
        /// </summary>
        public static double SoilSettlement(double initialVolume, double finalVolume)
        {
            return initialVolume - finalVolume;
        }

        private static double ValidateMixDenominator(double ratio, string parameterName)
        {
            var denominator = 1d + ratio;
            if (denominator == 0d)
            {
                throw new ArgumentOutOfRangeException(parameterName, "The ratio must not make the denominator equal to zero.");
            }

            return denominator;
        }

        private static void EnsureNonZero(double value, string parameterName)
        {
            if (value == 0d)
            {
                throw new ArgumentOutOfRangeException(parameterName, "The value must not be zero.");
            }
        }
    }
}
