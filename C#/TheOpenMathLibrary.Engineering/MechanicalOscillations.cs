namespace TheOpenMathLibrary.Engineering
{
    /// <summary>
    /// Provides energy formulas for simple and damped harmonic oscillations.
    /// </summary>
    public class MechanicalOscillations
    {
        /// <summary>
        /// Calculates the total mechanical energy of a simple harmonic oscillator.
        /// </summary>
        /// <param name="mass">The oscillating mass.</param>
        /// <param name="amplitude">The oscillation amplitude.</param>
        /// <param name="angularFrequency">The angular frequency.</param>
        /// <returns>The total oscillation energy.</returns>
        public static double ShmEnergy(double mass, double amplitude, double angularFrequency)
        {
            return 0.5d * mass * amplitude * amplitude * angularFrequency * angularFrequency;
        }

        /// <summary>
        /// Calculates the total mechanical energy of a simple harmonic oscillator.
        /// </summary>
        /// <param name="mass">The oscillating mass.</param>
        /// <param name="amplitude">The oscillation amplitude.</param>
        /// <param name="angularFrequency">The angular frequency.</param>
        /// <returns>The total oscillation energy.</returns>
        public static double SHMEnergy(double mass, double amplitude, double angularFrequency)
        {
            return ShmEnergy(mass, amplitude, angularFrequency);
        }

        /// <summary>
        /// Calculates the total energy of a damped harmonic oscillator using the same instantaneous form.
        /// </summary>
        /// <param name="mass">The oscillating mass.</param>
        /// <param name="amplitude">The oscillation amplitude.</param>
        /// <param name="angularFrequency">The angular frequency.</param>
        /// <returns>The oscillation energy.</returns>
        public static double DhmEnergy(double mass, double amplitude, double angularFrequency)
        {
            return 0.5d * mass * amplitude * amplitude * angularFrequency * angularFrequency;
        }

        /// <summary>
        /// Calculates the total energy of a damped harmonic oscillator using the same instantaneous form.
        /// </summary>
        /// <param name="mass">The oscillating mass.</param>
        /// <param name="amplitude">The oscillation amplitude.</param>
        /// <param name="angularFrequency">The angular frequency.</param>
        /// <returns>The oscillation energy.</returns>
        public static double DHMEnergy(double mass, double amplitude, double angularFrequency)
        {
            return DhmEnergy(mass, amplitude, angularFrequency);
        }
    }
}
