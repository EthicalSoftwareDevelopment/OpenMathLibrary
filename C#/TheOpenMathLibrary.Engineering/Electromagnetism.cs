using System;

namespace TheOpenMathLibrary.Engineering
{
    /// <summary>
    /// Provides helper formulas for introductory electromagnetism calculations.
    /// </summary>
    public class Electromagnetism
    {
        /// <summary>
        /// Calculates potential gradient magnitude from electric field and potential difference.
        /// </summary>
        /// <param name="electricField">The electric field magnitude.</param>
        /// <param name="potential">The electric potential difference.</param>
        /// <returns>The potential gradient.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="potential"/> is zero.</exception>
        public static double ElectricFieldPotentialGradient(double electricField, double potential)
        {
            EnsureNonZero(potential, nameof(potential));
            return electricField / potential;
        }

        /// <summary>
        /// Calculates electric flux density from electric field and permittivity.
        /// </summary>
        /// <param name="electricField">The electric field magnitude.</param>
        /// <param name="permittivity">The permittivity of the medium.</param>
        /// <returns>The electric flux density.</returns>
        public static double ElectricFluxDensity(double electricField, double permittivity)
        {
            return electricField * permittivity;
        }

        /// <summary>
        /// Calculates absolute permittivity from electric flux density and electric field.
        /// </summary>
        /// <param name="electricFluxDensity">The electric flux density.</param>
        /// <param name="electricField">The electric field magnitude.</param>
        /// <returns>The absolute permittivity.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="electricField"/> is zero.</exception>
        public static double AbsolutePermittivity(double electricFluxDensity, double electricField)
        {
            EnsureNonZero(electricField, nameof(electricField));
            return electricFluxDensity / electricField;
        }

        /// <summary>
        /// Calculates electric dipole moment.
        /// </summary>
        /// <param name="charge">The charge magnitude.</param>
        /// <param name="distance">The separation distance.</param>
        /// <returns>The electric dipole moment.</returns>
        public static double ElectricDipoleMoment(double charge, double distance)
        {
            return charge * distance;
        }

        /// <summary>
        /// Calculates electric polarization.
        /// </summary>
        /// <param name="electricDipoleMoment">The electric dipole moment.</param>
        /// <param name="volume">The material volume.</param>
        /// <returns>The electric polarization.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="volume"/> is zero.</exception>
        public static double ElectricPolarization(double electricDipoleMoment, double volume)
        {
            EnsureNonZero(volume, nameof(volume));
            return electricDipoleMoment / volume;
        }

        /// <summary>
        /// Calculates electric displacement field from polarization and permittivity.
        /// </summary>
        /// <param name="electricPolarization">The electric polarization.</param>
        /// <param name="permittivity">The permittivity of the medium.</param>
        /// <returns>The electric displacement field.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="permittivity"/> is zero.</exception>
        public static double ElectricDisplacementField(double electricPolarization, double permittivity)
        {
            EnsureNonZero(permittivity, nameof(permittivity));
            return electricPolarization / permittivity;
        }

        /// <summary>
        /// Calculates electric displacement flux.
        /// </summary>
        /// <param name="electricDisplacementField">The electric displacement field.</param>
        /// <param name="area">The cross-sectional area.</param>
        /// <returns>The electric displacement flux.</returns>
        public static double ElectricDisplacementFlux(double electricDisplacementField, double area)
        {
            return electricDisplacementField * area;
        }

        /// <summary>
        /// Calculates absolute electric potential from field and distance.
        /// </summary>
        /// <param name="electricField">The electric field magnitude.</param>
        /// <param name="distance">The displacement distance.</param>
        /// <returns>The electric potential difference.</returns>
        public static double AbsoluteElectricPotential(double electricField, double distance)
        {
            return electricField * distance;
        }

        private static void EnsureNonZero(double value, string parameterName)
        {
            if (value == 0d)
            {
                throw new ArgumentOutOfRangeException(parameterName, "The value must not be zero.");
            }
        }
    }

    /// <summary>
    /// Backward-compatible type alias for <see cref="Electromagnetism"/>.
    /// </summary>
    public class ElectroMagnetism : Electromagnetism
    {
    }
}
