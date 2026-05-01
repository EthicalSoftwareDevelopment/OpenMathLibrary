namespace TheOpenMathLibrary.GeneralMathematics
{
    /// <summary>
    /// Provides helper formulas for basic wave theory relationships.
    /// </summary>
    public class WaveTheory
    {
        /// <summary>
        /// Calculates wavelength from wave speed and frequency.
        /// </summary>
        public static double Wavelength(double waveSpeed, double frequency)
        {
            EnsureNonZero(frequency, nameof(frequency));
            return waveSpeed / frequency;
        }

        /// <summary>
        /// Calculates wave vector magnitude from wave speed and frequency.
        /// </summary>
        public static double WaveVector(double waveSpeed, double frequency)
        {
            EnsureNonZero(waveSpeed, nameof(waveSpeed));
            return 2d * Math.PI * frequency / waveSpeed;
        }

        /// <summary>
        /// Calculates frequency from wave speed and wavelength.
        /// </summary>
        public static double Frequency(double waveSpeed, double wavelength)
        {
            EnsureNonZero(wavelength, nameof(wavelength));
            return waveSpeed / wavelength;
        }

        /// <summary>
        /// Calculates angular frequency from frequency.
        /// </summary>
        public static double AngularFrequency(double frequency)
        {
            return 2d * Math.PI * frequency;
        }

        /// <summary>
        /// Calculates oscillatory velocity from amplitude, angular frequency, time, and phase.
        /// </summary>
        public static double OscillatoryVelocity(double amplitude, double angularFrequency, double time, double phase)
        {
            return amplitude * Math.Sin(angularFrequency * time + phase);
        }

        /// <summary>
        /// Calculates oscillatory acceleration from amplitude, angular frequency, time, and phase.
        /// </summary>
        public static double OscillatoryAcceleration(double amplitude, double angularFrequency, double time, double phase)
        {
            return amplitude * angularFrequency * Math.Cos(angularFrequency * time + phase);
        }

        /// <summary>
        /// Calculates phase velocity from angular frequency and wave vector magnitude.
        /// </summary>
        public static double PhaseVelocity(double waveSpeed, double waveVector)
        {
            EnsureNonZero(waveVector, nameof(waveVector));
            return waveSpeed / waveVector;
        }

        /// <summary>
        /// Calculates group velocity from wave speed and wave vector magnitude.
        /// </summary>
        public static double GroupVelocity(double waveSpeed, double waveVector)
        {
            EnsureNonZero(waveVector, nameof(waveVector));
            return waveSpeed / waveVector;
        }

        /// <summary>
        /// Calculates time delay from time and phase offset.
        /// </summary>
        public static double TimeDelay(double time, double phase)
        {
            return time - phase;
        }

        /// <summary>
        /// Calculates phase difference between two phases.
        /// </summary>
        public static double PhaseDifference(double phase1, double phase2)
        {
            return phase1 - phase2;
        }

        /// <summary>
        /// Calculates the phase of a traveling wave.
        /// </summary>
        public static double Phase(double waveVector, double position, double time, double phase)
        {
            return waveVector * position - 2d * Math.PI * time + phase;
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
