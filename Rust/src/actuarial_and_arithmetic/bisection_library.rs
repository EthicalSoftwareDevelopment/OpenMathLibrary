//! Root-finding algorithms for scalar equations.

/// Provides bisection, Newton-Raphson, and secant root-finding methods.
pub struct BisectionLibrary;

impl BisectionLibrary {
    /// Approximates a root using the bisection method on an interval with opposite endpoint signs.
    pub fn bisection<F>(
        mut value_a: f64,
        mut value_b: f64,
        tolerance: f64,
        max_iterations: usize,
        math_function: F,
    ) -> Result<f64, &'static str>
    where
        F: Fn(f64) -> f64,
    {
        if !value_a.is_finite() || !value_b.is_finite() {
            return Err("Interval endpoints must be finite.");
        }
        if tolerance <= 0.0 {
            return Err("Tolerance must be positive.");
        }
        if max_iterations == 0 {
            return Err("max_iterations must be greater than zero.");
        }

        let mut function_a = math_function(value_a);
        let function_b = math_function(value_b);

        if function_a.abs() <= tolerance {
            return Ok(value_a);
        }
        if function_b.abs() <= tolerance {
            return Ok(value_b);
        }

        if function_a * function_b > 0.0 {
            return Err("function(a) and function(b) must have opposite signs");
        }

        for _ in 0..max_iterations {
            let value_c = (value_a + value_b) / 2.0;
            let function_c = math_function(value_c);

            if function_c.abs() <= tolerance || (value_b - value_a).abs() / 2.0 <= tolerance {
                return Ok(value_c);
            }

            if function_a * function_c < 0.0 {
                value_b = value_c;
            } else {
                value_a = value_c;
                function_a = function_c;
            }
        }

        Err("Bisection method did not converge within the maximum number of iterations.")
    }

    /// Approximates a root using the Newton-Raphson method.
    pub fn newton_raphson<F, G>(
        mut x: f64,
        tolerance: f64,
        max_iterations: usize,
        math_function: F,
        math_function_derivative: G,
    ) -> Result<f64, &'static str>
    where
        F: Fn(f64) -> f64,
        G: Fn(f64) -> f64,
    {
        if !x.is_finite() {
            return Err("Initial guess must be finite.");
        }
        if tolerance <= 0.0 {
            return Err("Tolerance must be positive.");
        }
        if max_iterations == 0 {
            return Err("max_iterations must be greater than zero.");
        }

        for _ in 0..max_iterations {
            let function_x = math_function(x);
            if function_x.abs() <= tolerance {
                return Ok(x);
            }

            let derivative_x = math_function_derivative(x);
            if derivative_x.abs() <= f64::EPSILON {
                return Err("Derivative evaluated to zero.");
            }

            let x1 = x - function_x / derivative_x;
            if !x1.is_finite() {
                return Err("Newton-Raphson produced a non-finite iterate.");
            }

            if (x1 - x).abs() < tolerance {
                return Ok(x1);
            }
            x = x1;
        }

        Err("Newton-Raphson method did not converge within the maximum number of iterations.")
    }

    /// Approximates a root using the secant method.
    pub fn secant<F>(
        mut x0: f64,
        mut x1: f64,
        tolerance: f64,
        max_iterations: usize,
        math_function: F,
    ) -> Result<f64, &'static str>
    where
        F: Fn(f64) -> f64,
    {
        if !x0.is_finite() || !x1.is_finite() {
            return Err("Initial guesses must be finite.");
        }
        if tolerance <= 0.0 {
            return Err("Tolerance must be positive.");
        }
        if max_iterations == 0 {
            return Err("max_iterations must be greater than zero.");
        }

        for _ in 0..max_iterations {
            let function_x0 = math_function(x0);
            let function_x1 = math_function(x1);

            if function_x1.abs() <= tolerance {
                return Ok(x1);
            }

            let denominator = function_x1 - function_x0;
            if denominator.abs() <= f64::EPSILON {
                return Err("Secant method encountered nearly identical function values.");
            }

            let x2 = x1 - function_x1 * (x1 - x0) / denominator;
            if !x2.is_finite() {
                return Err("Secant method produced a non-finite iterate.");
            }
            if (x2 - x1).abs() <= tolerance {
                return Ok(x2);
            }

            x0 = x1;
            x1 = x2;
        }

        Err("Secant method did not converge within the maximum number of iterations.")
    }

    /// Alias for [`Self::bisection`].
    pub fn binomial<F>(
        value_a: f64,
        value_b: f64,
        tolerance: f64,
        max_iterations: usize,
        math_function: F,
    ) -> Result<f64, &'static str>
    where
        F: Fn(f64) -> f64,
    {
        Self::bisection(value_a, value_b, tolerance, max_iterations, math_function)
    }
}

#[cfg(test)]
mod tests {
    use super::BisectionLibrary;

    #[test]
    fn bisection_finds_square_root_of_two() {
        let root = BisectionLibrary::bisection(1.0, 2.0, 1e-12, 100, |x| x * x - 2.0).unwrap();
        assert!((root - 2.0_f64.sqrt()).abs() < 1e-9);
    }

    #[test]
    fn newton_raphson_finds_square_root_of_two() {
        let root =
            BisectionLibrary::newton_raphson(1.5, 1e-12, 25, |x| x * x - 2.0, |x| 2.0 * x).unwrap();
        assert!((root - 2.0_f64.sqrt()).abs() < 1e-9);
    }

    #[test]
    fn secant_finds_square_root_of_two() {
        let root = BisectionLibrary::secant(1.0, 2.0, 1e-12, 25, |x| x * x - 2.0).unwrap();
        assert!((root - 2.0_f64.sqrt()).abs() < 1e-9);
    }

    #[test]
    fn bisection_accepts_endpoint_roots_and_alias_matches() {
        let endpoint_root = BisectionLibrary::bisection(2.0, 4.0, 1e-12, 10, |x| x - 2.0).unwrap();
        let alias_root = BisectionLibrary::binomial(1.0, 2.0, 1e-12, 100, |x| x * x - 2.0).unwrap();

        assert!((endpoint_root - 2.0).abs() < 1e-12);
        assert!((alias_root - 2.0_f64.sqrt()).abs() < 1e-9);
    }

    #[test]
    fn root_finders_reject_invalid_inputs() {
        assert!(BisectionLibrary::bisection(1.0, 2.0, 0.0, 10, |x| x).is_err());
        assert!(BisectionLibrary::bisection(1.0, 2.0, 1e-12, 10, |x| x * x + 1.0).is_err());
        assert!(BisectionLibrary::newton_raphson(0.0, 1e-12, 10, |x| x * x, |_| 0.0).is_err());
        assert!(BisectionLibrary::newton_raphson(f64::NAN, 1e-12, 10, |x| x, |_| 1.0).is_err());
        assert!(BisectionLibrary::secant(1.0, 2.0, 1e-12, 10, |_| 5.0).is_err());
        assert!(BisectionLibrary::secant(f64::INFINITY, 1.0, 1e-12, 10, |x| x).is_err());
    }
}
