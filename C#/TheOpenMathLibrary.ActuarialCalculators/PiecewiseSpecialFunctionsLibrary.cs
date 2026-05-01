namespace TheOpenMathLibrary.ActuarialCalculators
{
    /// <summary>
    /// Provides piecewise-defined and special mathematical functions.
    /// </summary>
    public class PiecewiseSpecialFunctionsLibrary
    {
        private const double ComparisonTolerance = 1e-12;

        /// <summary>
        /// Evaluates the indicator function for zero.
        /// </summary>
        /// <param name="x">The value to test.</param>
        /// <returns><c>1</c> when <paramref name="x"/> is zero; otherwise <c>0</c>.</returns>
        public static double IndicatorFunction(double x)
        {
            return x == 0d ? 1d : 0d;
        }

        /// <summary>
        /// Evaluates the unit step function with a midpoint value of 0.5 at zero.
        /// </summary>
        /// <param name="x">The value at which to evaluate the function.</param>
        /// <returns>The step-function value at <paramref name="x"/>.</returns>
        public static double StepFunction(double x)
        {
            if (x < 0d)
            {
                return 0d;
            }

            return x == 0d ? 0.5d : 1d;
        }

        /// <summary>
        /// Evaluates the Heaviside step function.
        /// </summary>
        /// <param name="x">The value at which to evaluate the function.</param>
        /// <returns><c>0</c> for negative values; otherwise <c>1</c>.</returns>
        public static double HeavisideStepFunction(double x)
        {
            return x < 0d ? 0d : 1d;
        }

        /// <summary>
        /// Evaluates the rectangular function centered at zero.
        /// </summary>
        /// <param name="x">The value at which to evaluate the function.</param>
        /// <returns>The rectangular-function value at <paramref name="x"/>.</returns>
        public static double RectangularFunction(double x)
        {
            if (x < -0.5d || x > 0.5d)
            {
                return 0d;
            }

            return Math.Abs(x - 0.5d) <= ComparisonTolerance || Math.Abs(x + 0.5d) <= ComparisonTolerance ? 0.5d : 1d;
        }

        /// <summary>
        /// Evaluates a sawtooth wave with period 1.
        /// </summary>
        /// <param name="x">The input value.</param>
        /// <returns>The sawtooth-wave value at <paramref name="x"/>.</returns>
        public static double SawtoothFunction(double x)
        {
            return x - Math.Floor(x);
        }

        /// <summary>
        /// Evaluates a triangle wave with period 1 and range [0, 1].
        /// </summary>
        /// <param name="x">The input value.</param>
        /// <returns>The triangle-wave value at <paramref name="x"/>.</returns>
        public static double TriangleWaveFunction(double x)
        {
            return 2d * Math.Abs(SawtoothFunction(x) - 0.5d);
        }

        /// <summary>
        /// Evaluates a square wave derived from the sawtooth function.
        /// </summary>
        /// <param name="x">The input value.</param>
        /// <returns><c>0</c> on the first half-period and <c>1</c> on the second half-period.</returns>
        public static double SquareWaveFunction(double x)
        {
            return HeavisideStepFunction(SawtoothFunction(x) - 0.5d);
        }

        /// <summary>
        /// Evaluates the normalized sinc function sin(x) / x.
        /// </summary>
        /// <param name="x">The input value.</param>
        /// <returns>The sinc value at <paramref name="x"/>.</returns>
        public static double SincFunction(double x)
        {
            if (Math.Abs(x) <= ComparisonTolerance)
            {
                return 1d;
            }

            return Math.Sin(x) / x;
        }

        /// <summary>
        /// Evaluates the Dirichlet kernel of order <paramref name="order"/>.
        /// </summary>
        /// <param name="x">The input angle in radians.</param>
        /// <param name="order">The non-negative kernel order.</param>
        /// <returns>The value of the Dirichlet kernel.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="order"/> is negative.</exception>
        public static double DirichletKernel(double x, int order)
        {
            if (order < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(order), "The order must be non-negative.");
            }

            if (Math.Abs(x) <= ComparisonTolerance)
            {
                return 2d * order + 1d;
            }

            return Math.Sin((order + 0.5d) * x) / Math.Sin(0.5d * x);
        }
    }
}
