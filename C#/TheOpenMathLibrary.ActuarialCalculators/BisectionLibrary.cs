namespace TheOpenMathLibrary.ActuarialCalculators
{
    /// <summary>
    /// Provides root-finding methods for one-dimensional functions.
    /// </summary>
    public class BisectionLibrary
    {
        /// <summary>
        /// Finds a root of a continuous function on an interval using the bisection method.
        /// </summary>
        /// <param name="valueA">The lower bound of the interval.</param>
        /// <param name="valueB">The upper bound of the interval.</param>
        /// <param name="tolerance">The absolute error tolerance used as a stopping condition.</param>
        /// <param name="maxIterations">The maximum number of iterations to perform.</param>
        /// <param name="mathFunction">The function for which a root is sought.</param>
        /// <returns>An approximation of a root in the interval.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="mathFunction"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="tolerance"/> or <paramref name="maxIterations"/> is invalid.</exception>
        /// <exception cref="ArgumentException">Thrown when the interval does not bracket a root.</exception>
        public static double Bisection(double valueA, double valueB, double tolerance, int maxIterations, Func<double, double> mathFunction)
        {
            return SolveByIntervalHalving(valueA, valueB, tolerance, maxIterations, mathFunction);
        }

        /// <summary>
        /// Finds a root of a function using the Newton-Raphson method.
        /// </summary>
        /// <param name="x0">The initial estimate.</param>
        /// <param name="tolerance">The absolute error tolerance used as a stopping condition.</param>
        /// <param name="maxIterations">The maximum number of iterations to perform.</param>
        /// <param name="mathFunction">The function for which a root is sought.</param>
        /// <param name="mathFunctionDerivative">The derivative of <paramref name="mathFunction"/>.</param>
        /// <returns>An approximation of a root near <paramref name="x0"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when a delegate argument is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="tolerance"/> or <paramref name="maxIterations"/> is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the derivative becomes zero during iteration.</exception>
        public static double NewtonRaphson(double x0, double tolerance, int maxIterations, Func<double, double> mathFunction, Func<double, double> mathFunctionDerivative)
        {
            ValidateCommonInputs(tolerance, maxIterations);

            ArgumentNullException.ThrowIfNull(mathFunction);
            ArgumentNullException.ThrowIfNull(mathFunctionDerivative);

            var current = x0;
            for (var iteration = 0; iteration < maxIterations; iteration++)
            {
                var functionValue = mathFunction(current);
                if (Math.Abs(functionValue) <= tolerance)
                {
                    return current;
                }

                var derivativeValue = mathFunctionDerivative(current);
                if (Math.Abs(derivativeValue) <= double.Epsilon)
                {
                    throw new InvalidOperationException("The derivative evaluated to zero during the Newton-Raphson iteration.");
                }

                var next = current - functionValue / derivativeValue;
                if (Math.Abs(next - current) <= tolerance)
                {
                    return next;
                }

                current = next;
            }

            return current;
        }

        /// <summary>
        /// Finds a root of a function using the same interval-halving approach as the bisection method.
        /// </summary>
        /// <param name="valueA">The lower bound of the interval.</param>
        /// <param name="valueB">The upper bound of the interval.</param>
        /// <param name="tolerance">The absolute error tolerance used as a stopping condition.</param>
        /// <param name="maxIterations">The maximum number of iterations to perform.</param>
        /// <param name="mathFunction">The function for which a root is sought.</param>
        /// <returns>An approximation of a root in the interval.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="mathFunction"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="tolerance"/> or <paramref name="maxIterations"/> is invalid.</exception>
        /// <exception cref="ArgumentException">Thrown when the interval does not bracket a root.</exception>
        public static double Binomial(double valueA, double valueB, double tolerance, int maxIterations, Func<double, double> mathFunction)
        {
            return SolveByIntervalHalving(valueA, valueB, tolerance, maxIterations, mathFunction);
        }

        private static double SolveByIntervalHalving(double valueA, double valueB, double tolerance, int maxIterations, Func<double, double> mathFunction)
        {
            ValidateCommonInputs(tolerance, maxIterations);
            ArgumentNullException.ThrowIfNull(mathFunction);

            var functionA = mathFunction(valueA);
            if (Math.Abs(functionA) <= tolerance)
            {
                return valueA;
            }

            var functionB = mathFunction(valueB);
            if (Math.Abs(functionB) <= tolerance)
            {
                return valueB;
            }

            if (functionA * functionB > 0)
            {
                throw new ArgumentException("function(a) and function(b) must have opposite signs.");
            }

            var midpoint = (valueA + valueB) / 2d;
            for (var iteration = 0; iteration < maxIterations; iteration++)
            {
                midpoint = (valueA + valueB) / 2d;
                var functionMidpoint = mathFunction(midpoint);

                if (Math.Abs(functionMidpoint) <= tolerance || Math.Abs(valueB - valueA) / 2d <= tolerance)
                {
                    return midpoint;
                }

                if (functionA * functionMidpoint < 0)
                {
                    valueB = midpoint;
                }
                else
                {
                    valueA = midpoint;
                    functionA = functionMidpoint;
                }
            }

            return midpoint;
        }

        private static void ValidateCommonInputs(double tolerance, int maxIterations)
        {
            if (tolerance <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tolerance), "Tolerance must be greater than zero.");
            }

            if (maxIterations <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxIterations), "The maximum number of iterations must be greater than zero.");
            }
        }
    }
}
