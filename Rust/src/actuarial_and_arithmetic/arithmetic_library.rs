//! Arithmetic and elementary number-theory helpers.

/// Provides common arithmetic, combinatoric, and number-theoretic operations.
pub struct ArithmeticLibrary;

impl ArithmeticLibrary {
    /// Computes the greatest common divisor of two integers.
    pub fn gcd(mut value_a: i64, mut value_b: i64) -> i64 {
        value_a = value_a.abs();
        value_b = value_b.abs();

        while value_b != 0 {
            let remainder = value_a % value_b;
            value_a = value_b;
            value_b = remainder;
        }

        value_a
    }

    /// Computes the least common multiple of two integers.
    pub fn lcm(value_a: i64, value_b: i64) -> i64 {
        if value_a == 0 || value_b == 0 {
            return 0;
        }

        (value_a / Self::gcd(value_a, value_b) * value_b).abs()
    }

    /// Computes `n!` when the result fits in `u128`.
    pub fn factorial(number: usize) -> Result<u128, &'static str> {
        if number > 34 {
            return Err("Factorial result would overflow u128.");
        }

        let mut result = 1_u128;
        for value in 2..=number {
            result = result
                .checked_mul(value as u128)
                .ok_or("Factorial result would overflow u128.")?;
        }

        Ok(result)
    }

    /// Computes the binomial coefficient `n choose k`.
    pub fn binomial_coefficient(number_n: usize, number_k: usize) -> Result<u128, &'static str> {
        if number_k > number_n {
            return Err("k must be less than or equal to n.");
        }

        let mut result = 1_u128;
        let reduced_k = number_k.min(number_n - number_k);

        for index in 0..reduced_k {
            let numerator = (number_n - reduced_k + index + 1) as u128;
            let denominator = (index + 1) as u128;
            result = result
                .checked_mul(numerator)
                .ok_or("Binomial coefficient would overflow u128.")?
                / denominator;
        }

        Ok(result)
    }

    /// Computes the Möbius function of an integer.
    pub fn mobius(number: i32) -> i32 {
        if number == 0 {
            return 0;
        }

        let factors = Self::prime_factorization(number);
        if factors.iter().any(|(_, exponent)| *exponent > 1) {
            return 0;
        }

        if factors.len().is_multiple_of(2) {
            1
        } else {
            -1
        }
    }

    /// Computes the sum of all positive divisors of a positive integer.
    pub fn sigma(number: i32) -> Result<i32, &'static str> {
        Self::validate_positive_integer(number)?;

        let positive_number = number.abs();
        let mut sum = 0_i32;

        for divisor in 1..=((positive_number as f64).sqrt() as i32) {
            if positive_number % divisor == 0 {
                let paired_divisor = positive_number / divisor;
                sum += divisor;
                if paired_divisor != divisor {
                    sum += paired_divisor;
                }
            }
        }

        Ok(sum)
    }

    /// Counts the number of positive divisors of a positive integer.
    pub fn divisor_count(number: i32) -> Result<i32, &'static str> {
        Self::validate_positive_integer(number)?;

        let count = Self::prime_factorization(number)
            .into_iter()
            .map(|(_, exponent)| exponent as i32 + 1)
            .product();

        Ok(count)
    }

    /// Computes the aliquot sum of a positive integer.
    pub fn aliquot_sum(number: i32) -> Result<i32, &'static str> {
        Self::validate_positive_integer(number)?;

        if number == 1 {
            return Ok(0);
        }

        Ok(Self::sigma(number)? - number.abs())
    }

    /// Computes Euler's totient function.
    pub fn totient(number: i32) -> Result<i32, &'static str> {
        Self::validate_positive_integer(number)?;

        let mut result = number;
        for (prime, _) in Self::prime_factorization(number) {
            result -= result / prime as i32;
        }

        Ok(result)
    }

    /// Counts the prime numbers less than or equal to `number`.
    pub fn prime_counting(number: usize) -> usize {
        if number < 2 {
            return 0;
        }

        let mut is_prime = vec![true; number + 1];
        is_prime[0] = false;
        is_prime[1] = false;

        for candidate in 2..=((number as f64).sqrt() as usize) {
            if is_prime[candidate] {
                for composite in (candidate * candidate..=number).step_by(candidate) {
                    is_prime[composite] = false;
                }
            }
        }

        is_prime.into_iter().filter(|value| *value).count()
    }

    /// Computes the integer partition count of `number`.
    pub fn partition(number: usize) -> Result<usize, &'static str> {
        let mut partitions = vec![0_usize; number + 1];
        partitions[0] = 1;

        for part in 1..=number {
            for total in part..=number {
                partitions[total] = partitions[total]
                    .checked_add(partitions[total - part])
                    .ok_or("Partition result overflowed usize.")?;
            }
        }

        Ok(partitions[number])
    }

    /// Counts the distinct prime factors of an integer.
    pub fn omega(number: i32) -> i32 {
        Self::prime_factorization(number).len() as i32
    }

    /// Counts prime factors with multiplicity.
    pub fn big_omega(number: i32) -> i32 {
        Self::prime_factorization(number)
            .into_iter()
            .map(|(_, exponent)| exponent as i32)
            .sum()
    }

    /// Computes the first Chebyshev function.
    pub fn chebyshev_theta(number: i32) -> f64 {
        if number < 2 {
            return 0.0;
        }

        let mut sum = 0.0;
        for candidate in 2..=number {
            if Self::is_prime(candidate) {
                sum += (candidate as f64).ln();
            }
        }

        sum
    }

    /// Computes the second Chebyshev function.
    pub fn chebyshev_psi(number: i32) -> f64 {
        if number < 2 {
            return 0.0;
        }

        let mut sum = 0.0;
        for prime in 2..=number {
            if Self::is_prime(prime) {
                let mut prime_power = i64::from(prime);
                while prime_power <= i64::from(number) {
                    sum += (prime as f64).ln();
                    prime_power *= i64::from(prime);
                }
            }
        }

        sum
    }

    /// Computes the Liouville function of a positive integer.
    pub fn liouville(number: i32) -> Result<i32, &'static str> {
        Self::validate_positive_integer(number)?;

        if Self::big_omega(number) % 2 == 0 {
            Ok(1)
        } else {
            Ok(-1)
        }
    }

    /// Returns whether an integer is prime.
    pub fn is_prime(number: i32) -> bool {
        if number < 2 {
            return false;
        }

        if number == 2 {
            return true;
        }

        if number % 2 == 0 {
            return false;
        }

        let limit = (number as f64).sqrt() as i32;
        let mut divisor = 3;
        while divisor <= limit {
            if number % divisor == 0 {
                return false;
            }
            divisor += 2;
        }

        true
    }

    /// Returns the prime factorization of an integer as `(prime, exponent)` pairs.
    pub fn prime_factorization(number: i32) -> Vec<(i64, u32)> {
        let mut remaining = i64::from(number).abs();
        let mut factors = Vec::new();

        if remaining < 2 {
            return factors;
        }

        let mut divisor = 2_i64;
        while divisor * divisor <= remaining {
            let mut exponent = 0_u32;
            while remaining % divisor == 0 {
                remaining /= divisor;
                exponent += 1;
            }

            if exponent > 0 {
                factors.push((divisor, exponent));
            }

            divisor += if divisor == 2 { 1 } else { 2 };
        }

        if remaining > 1 {
            factors.push((remaining, 1));
        }

        factors
    }

    fn validate_positive_integer(number: i32) -> Result<(), &'static str> {
        if number < 1 {
            Err("Number must be a positive integer.")
        } else {
            Ok(())
        }
    }
}

#[cfg(test)]
mod tests {
    use super::ArithmeticLibrary;

    #[test]
    fn arithmetic_utilities_cover_common_cases() {
        assert_eq!(ArithmeticLibrary::gcd(84, 30), 6);
        assert_eq!(ArithmeticLibrary::lcm(21, 6), 42);
        assert_eq!(ArithmeticLibrary::factorial(10).unwrap(), 3_628_800);
        assert_eq!(ArithmeticLibrary::binomial_coefficient(10, 3).unwrap(), 120);
    }

    #[test]
    fn divisor_and_partition_functions_are_consistent() {
        assert_eq!(ArithmeticLibrary::sigma(12).unwrap(), 28);
        assert_eq!(ArithmeticLibrary::divisor_count(12).unwrap(), 6);
        assert_eq!(ArithmeticLibrary::aliquot_sum(12).unwrap(), 16);
        assert_eq!(ArithmeticLibrary::partition(5).unwrap(), 7);
    }

    #[test]
    fn number_theory_functions_match_known_values() {
        assert_eq!(ArithmeticLibrary::totient(9).unwrap(), 6);
        assert_eq!(ArithmeticLibrary::prime_counting(10), 4);
        assert_eq!(ArithmeticLibrary::omega(12), 2);
        assert_eq!(ArithmeticLibrary::big_omega(12), 3);
        assert_eq!(ArithmeticLibrary::mobius(30), -1);
        assert_eq!(ArithmeticLibrary::mobius(12), 0);
        assert_eq!(ArithmeticLibrary::liouville(12).unwrap(), -1);
    }

    #[test]
    fn chebyshev_functions_are_ordered_correctly() {
        let theta = ArithmeticLibrary::chebyshev_theta(10);
        let psi = ArithmeticLibrary::chebyshev_psi(10);
        assert!(theta > 0.0);
        assert!(psi >= theta);
    }
}
