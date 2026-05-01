namespace TheOpenMathLibrary.Engineering
{
    /// <summary>
    /// Provides classical mechanics helper formulas for mass distribution and rigid-body quantities.
    /// </summary>
    public class ClassicalMechanics
    {
        /// <summary>
        /// Calculates linear mass density.
        /// </summary>
        /// <param name="mass">The mass of the object.</param>
        /// <param name="length">The length over which the mass is distributed.</param>
        /// <returns>The mass per unit length.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="length"/> is zero.</exception>
        public static double LinearMassDensity(double mass, double length)
        {
            EnsureNonZero(length, nameof(length));
            return mass / length;
        }

        /// <summary>
        /// Calculates surface mass density.
        /// </summary>
        /// <param name="mass">The mass of the object.</param>
        /// <param name="area">The surface area over which the mass is distributed.</param>
        /// <returns>The mass per unit area.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="area"/> is zero.</exception>
        public static double SurfaceMassDensity(double mass, double area)
        {
            EnsureNonZero(area, nameof(area));
            return mass / area;
        }

        /// <summary>
        /// Calculates volumetric mass density.
        /// </summary>
        /// <param name="mass">The mass of the object.</param>
        /// <param name="volume">The volume over which the mass is distributed.</param>
        /// <returns>The mass per unit volume.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="volume"/> is zero.</exception>
        public static double VolumetricMassDensity(double mass, double volume)
        {
            EnsureNonZero(volume, nameof(volume));
            return mass / volume;
        }

        /// <summary>
        /// Calculates the moment of mass using radius squared weighting.
        /// </summary>
        /// <param name="mass">The mass.</param>
        /// <param name="radius">The radial distance.</param>
        /// <returns>The moment of mass.</returns>
        public static double MomentOfMass(double mass, double radius)
        {
            return mass * radius * radius;
        }

        /// <summary>
        /// Calculates the center of mass of a two-body system on a line.
        /// </summary>
        /// <param name="mass1">The first mass.</param>
        /// <param name="mass2">The second mass.</param>
        /// <param name="radius1">The position of the first mass.</param>
        /// <param name="radius2">The position of the second mass.</param>
        /// <returns>The center-of-mass position.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the combined mass is zero.</exception>
        public static double CenterOfMass(double mass1, double mass2, double radius1, double radius2)
        {
            var totalMass = mass1 + mass2;
            EnsureNonZero(totalMass, nameof(mass2));
            return (mass1 * radius1 + mass2 * radius2) / totalMass;
        }

        /// <summary>
        /// Calculates the reduced mass of a two-body system.
        /// </summary>
        /// <param name="mass1">The first mass.</param>
        /// <param name="mass2">The second mass.</param>
        /// <returns>The reduced mass.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the combined mass is zero.</exception>
        public static double ReducedMass(double mass1, double mass2)
        {
            var totalMass = mass1 + mass2;
            EnsureNonZero(totalMass, nameof(mass2));
            return mass1 * mass2 / totalMass;
        }

        /// <summary>
        /// Calculates the moment of inertia for a point mass at a distance from the axis of rotation.
        /// </summary>
        /// <param name="mass">The mass.</param>
        /// <param name="radius">The distance to the axis.</param>
        /// <returns>The moment of inertia.</returns>
        public static double MomentOfInertia(double mass, double radius)
        {
            return mass * radius * radius;
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
