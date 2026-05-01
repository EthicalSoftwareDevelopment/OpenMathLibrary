
namespace TheOpenMathLibrary.ActuarialCalculators
{
    /// <summary>
    /// Provides arithmetic and number-theory helper functions.
    /// </summary>
    public class ArithmeticLibrary
    {
        /// <summary>
        /// Calculates the Möbius function μ(n).
        /// </summary>
        /// <param name="number">The positive integer to evaluate.</param>
        /// <returns>
        /// <c>1</c> when <paramref name="number"/> is square-free with an even number of prime factors,
        /// <c>-1</c> when it is square-free with an odd number of prime factors, and <c>0</c> otherwise.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="number"/> is less than 1.</exception>
        public static int Mobius(int number)
        {
            if (number < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(number), "Number must be a positive integer.");
            }

            if (number == 1)
            {
                return 1;
            }

            var remaining = number;
            var primeFactorCount = 0;

            for (var factor = 2; factor * factor <= remaining; factor++)
            {
                if (remaining % factor != 0)
                {
                    continue;
                }

                var multiplicity = 0;
                while (remaining % factor == 0)
                {
                    remaining /= factor;
                    multiplicity++;

                    if (multiplicity > 1)
                    {
                        return 0;
                    }
                }

                primeFactorCount++;
            }

            if (remaining > 1)
            {
                primeFactorCount++;
            }

            return primeFactorCount % 2 == 0 ? 1 : -1;
        }

        /// <summary>
        /// Calculates the sum of the positive divisors of a number.
        /// </summary>
        /// <param name="number">The positive integer to evaluate.</param>
        /// <returns>The sum of all positive divisors of <paramref name="number"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="number"/> is less than 1.</exception>
        public static int Sigma(int number)
        {
            if (number < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(number), "Number must be a positive integer.");
            }

            var sum = 0;
            var limit = (int)Math.Sqrt(number);

            for (var divisor = 1; divisor <= limit; divisor++)
            {
                if (number % divisor != 0)
                {
                    continue;
                }

                var quotient = number / divisor;
                sum += divisor;

                if (quotient != divisor)
                {
                    sum += quotient;
                }
            }

            return sum;
        }

        /// <summary>
        /// Calculates Euler's totient function φ(n).
        /// </summary>
        /// <param name="number">The positive integer to evaluate.</param>
        /// <returns>The count of integers from 1 through <paramref name="number"/> that are coprime to it.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="number"/> is less than 1.</exception>
        public static int Totient(int number)
        {
            if (number < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(number), "Number must be a positive integer.");
            }

            var remaining = number;
            var result = number;

            for (var factor = 2; factor * factor <= remaining; factor++)
            {
                if (remaining % factor != 0)
                {
                    continue;
                }

                while (remaining % factor == 0)
                {
                    remaining /= factor;
                }

                result -= result / factor;
            }

            if (remaining > 1)
            {
                result -= result / remaining;
            }

            return result;
        }

        /// <summary>
        /// Counts the number of prime numbers less than or equal to a value.
        /// </summary>
        /// <param name="number">The inclusive upper bound.</param>
        /// <returns>The number of primes less than or equal to <paramref name="number"/>.</returns>
        public static int PrimeCounting(int number)
        {
            if (number < 2)
            {
                return 0;
            }

            var isPrime = new bool[number + 1];
            Array.Fill(isPrime, true, 2, number - 1);

            for (var factor = 2; factor * factor <= number; factor++)
            {
                if (!isPrime[factor])
                {
                    continue;
                }

                for (var composite = factor * factor; composite <= number; composite += factor)
                {
                    isPrime[composite] = false;
                }
            }

            var primeCount = 0;
            for (var candidate = 2; candidate <= number; candidate++)
            {
                if (isPrime[candidate])
                {
                    primeCount++;
                }
            }

            return primeCount;
        }

        /// <summary>
        /// Calculates the partition number p(n), the number of ways to write a non-negative integer as a sum of positive integers.
        /// </summary>
        /// <param name="number">The non-negative integer to partition.</param>
        /// <returns>The number of additive partitions of <paramref name="number"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="number"/> is negative.</exception>
        /// <exception cref="OverflowException">Thrown when the result exceeds the range of <see cref="int"/>.</exception>
        public static int Partition(int number)
        {
            if (number < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(number), "Number must be a non-negative integer.");
            }

            var partitions = new int[number + 1];
            partitions[0] = 1;

            for (var part = 1; part <= number; part++)
            {
                for (var target = part; target <= number; target++)
                {
                    partitions[target] = checked(partitions[target] + partitions[target - part]);
                }
            }

            return partitions[number];
        }

        /// <summary>
        /// Calculates Ω(n), the total number of prime factors counted with multiplicity.
        /// </summary>
        /// <param name="number">The integer to factor.</param>
        /// <returns>The number of prime factors counted with multiplicity.</returns>
        public static int Omega(int number)
        {
            if (number < 2)
            {
                return 0;
            }

            var remaining = number;
            var totalPrimeFactors = 0;

            for (var factor = 2; factor * factor <= remaining; factor++)
            {
                while (remaining % factor == 0)
                {
                    totalPrimeFactors++;
                    remaining /= factor;
                }
            }

            if (remaining > 1)
            {
                totalPrimeFactors++;
            }

            return totalPrimeFactors;
        }

        /// <summary>
        /// Calculates Chebyshev's theta function θ(n), the sum of logarithms of the primes up to a bound.
        /// </summary>
        /// <param name="number">The inclusive upper bound.</param>
        /// <returns>The value of θ(<paramref name="number"/>).</returns>
        public static double ChebyshevTheta(int number)
        {
            if (number < 2)
            {
                return 0d;
            }

            var sum = 0d;
            for (var candidate = 2; candidate <= number; candidate++)
            {
                if (IsPrime(candidate))
                {
                    sum += Math.Log(candidate);
                }
            }

            return sum;
        }

        /// <summary>
        /// Calculates Chebyshev's psi function ψ(n), the sum of logarithms over prime powers up to a bound.
        /// </summary>
        /// <param name="number">The inclusive upper bound.</param>
        /// <returns>The value of ψ(<paramref name="number"/>).</returns>
        public static double ChebyshevPsi(int number)
        {
            if (number < 2)
            {
                return 0d;
            }

            var sum = 0d;
            for (var prime = 2; prime <= number; prime++)
            {
                if (!IsPrime(prime))
                {
                    continue;
                }

                long power = prime;
                while (power <= number)
                {
                    sum += Math.Log(prime);

                    if (power > number / prime)
                    {
                        break;
                    }

                    power *= prime;
                }
            }

            return sum;
        }

        /// <summary>
        /// Calculates Liouville's lambda function λ(n).
        /// </summary>
        /// <param name="number">The positive integer to evaluate.</param>
        /// <returns><c>1</c> when Ω(n) is even; otherwise <c>-1</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="number"/> is less than 1.</exception>
        public static int Liouville(int number)
        {
            if (number < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(number), "Number must be a positive integer.");
            }

            return Omega(number) % 2 == 0 ? 1 : -1;
        }

        private static bool IsPrime(int number)
        {
            if (number < 2)
            {
                return false;
            }

            if (number == 2)
            {
                return true;
            }

            if (number % 2 == 0)
            {
                return false;
            }

            for (var factor = 3; factor * factor <= number; factor += 2)
            {
                if (number % factor == 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
