using System;

namespace TheOpenMathLibrary.Engineering
{
    /// <summary>
    /// Provides helper methods for common angular-frequency relationships in oscillatory systems.
    /// </summary>
    public class AngularFrequencies
    {
        /// <summary>
        /// Returns the angular frequency of a linear undamped, unforced oscillator.
        /// </summary>
        /// <param name="angularFrequency">The angular frequency in radians per second.</param>
        /// <returns>The same angular frequency that was provided.</returns>
        public static double LinearUndampedUnforcedOscillator(double angularFrequency)
        {
            return angularFrequency;
        }

        /// <summary>
        /// Calculates the damped angular frequency for a linear unforced damped harmonic oscillator.
        /// </summary>
        /// <param name="angularFrequency">The undamped natural angular frequency.</param>
        /// <param name="dampingCoefficient">The normalized damping coefficient.</param>
        /// <returns>The damped angular frequency.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the damping coefficient produces a negative radicand.</exception>
        public static double LinearUnforcedDHO(double angularFrequency, double dampingCoefficient)
        {
            var radicand = 1d - dampingCoefficient * dampingCoefficient;
            if (radicand < 0d)
            {
                throw new ArgumentOutOfRangeException(nameof(dampingCoefficient), "The damping coefficient must satisfy |dampingCoefficient| <= 1.");
            }

            return angularFrequency * Math.Sqrt(radicand);
        }

        /// <summary>
        /// Calculates a low-amplitude angular simple harmonic oscillation quantity from angular frequency and amplitude.
        /// </summary>
        /// <param name="angularFrequency">The angular frequency.</param>
        /// <param name="amplitude">The oscillation amplitude.</param>
        /// <returns>The product of angular frequency and amplitude.</returns>
        public static double LowAmplitudeAngularSHO(double angularFrequency, double amplitude)
        {
            return angularFrequency * amplitude;
        }

        /// <summary>
        /// Calculates a low-amplitude simple pendulum quantity from angular frequency and amplitude.
        /// </summary>
        /// <param name="angularFrequency">The angular frequency.</param>
        /// <param name="amplitude">The oscillation amplitude.</param>
        /// <returns>The product of angular frequency and amplitude.</returns>
        public static double LowAmplitudeSimplePendulum(double angularFrequency, double amplitude)
        {
            return angularFrequency * amplitude;
        }
    }
}
