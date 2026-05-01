namespace TheOpenMathLibrary.GeneralMathematics
{
    /// <summary>
    /// Provides basic algebraic helper functions.
    /// </summary>
    public class AlgebraicLibrary
    {
        /// <summary>
        /// Returns the linear identity function.
        /// </summary>
        public static double Linear(double x)
        {
            return x;
        }

        /// <summary>
        /// Calculates the cube of a value.
        /// </summary>
        public static double Cubic(double x)
        {
            return x * x * x;
        }

        /// <summary>
        /// Calculates the fourth power of a value.
        /// </summary>
        public static double Quartic(double x)
        {
            return x * x * x * x;
        }

        /// <summary>
        /// Calculates the fifth power of a value.
        /// </summary>
        public static double Quintic(double x)
        {
            return x * x * x * x * x;
        }

        /// <summary>
        /// Calculates the principal square root of a non-negative value.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="x"/> is negative.</exception>
        public static double SquareRoot(double x)
        {
            if (x < 0d)
            {
                throw new ArgumentOutOfRangeException(nameof(x), "The value must be non-negative.");
            }

            return Math.Sqrt(x);
        }

        /// <summary>
        /// Calculates the real cube root of a value.
        /// </summary>
        public static double CubeRoot(double x)
        {
            return Math.Cbrt(x);
        }
    }
}
