namespace TheOpenMathLibrary.ActuarialCalculators
{
    /// <summary>
    /// Provides transcendental and exponential helper functions.
    /// </summary>
    public class TranscendentalLibrary
    {
        /// <summary>
        /// Raises a value to a specified power.
        /// </summary>
        /// <param name="x">The base value.</param>
        /// <param name="power">The exponent to apply.</param>
        /// <returns><paramref name="x"/> raised to <paramref name="power"/>.</returns>
        public static double Exponential(double x, double power)
        {
            return Math.Pow(x, power);
        }

        /// <summary>
        /// Calculates the hyperbolic sine of a value.
        /// </summary>
        /// <param name="x">The input value.</param>
        /// <returns>The hyperbolic sine of <paramref name="x"/>.</returns>
        public static double HyperbolicSine(double x)
        {
            return Math.Sinh(x);
        }

        /// <summary>
        /// Calculates the natural logarithm of a positive value.
        /// </summary>
        /// <param name="x">The positive input value.</param>
        /// <returns>The natural logarithm of <paramref name="x"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="x"/> is less than or equal to zero.</exception>
        public static double Logarithm(double x)
        {
            ValidatePositive(x, nameof(x));
            return Math.Log(x);
        }

        /// <summary>
        /// Calculates the base-10 logarithm of a positive value.
        /// </summary>
        /// <param name="x">The positive input value.</param>
        /// <returns>The base-10 logarithm of <paramref name="x"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="x"/> is less than or equal to zero.</exception>
        public static double CommonLogarithm10(double x)
        {
            ValidatePositive(x, nameof(x));
            return Math.Log10(x);
        }

        /// <summary>
        /// Calculates the base-2 logarithm of a positive value.
        /// </summary>
        /// <param name="x">The positive input value.</param>
        /// <returns>The base-2 logarithm of <paramref name="x"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="x"/> is less than or equal to zero.</exception>
        public static double BinaryLogarithm2(double x)
        {
            ValidatePositive(x, nameof(x));
            return Math.Log2(x);
        }

        private static void ValidatePositive(double value, string parameterName)
        {
            if (value <= 0d)
            {
                throw new ArgumentOutOfRangeException(parameterName, "The value must be greater than zero.");
            }
        }
    }
}
