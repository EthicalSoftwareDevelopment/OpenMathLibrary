//! Root-finding algorithms for scalar equations.

use std::fmt;
use std::num::NonZeroUsize;

/// Errors returned by the root-finding helpers.
#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum RootSolveError {
    /// One or more numeric inputs were not finite.
    NonFiniteInput,
    /// The tolerance must be finite and strictly positive.
    InvalidTolerance,
    /// The iteration budget must be non-zero.
    ZeroMaxIterations,
    /// The interval endpoints do not bracket a sign change.
    IntervalDoesNotBracketRoot,
    /// The Newton-Raphson derivative evaluated to zero.
    ZeroDerivative,
    /// The numerical method produced a non-finite iterate.
    NonFiniteIterate,
    /// The secant method encountered an unstable denominator.
    DegenerateSecantDenominator,
    /// The method did not converge before exhausting the iteration budget.
    DidNotConverge,
}

impl fmt::Display for RootSolveError {
    fn fmt(&self, formatter: &mut fmt::Formatter<'_>) -> fmt::Result {
        let message = match self {
            Self::NonFiniteInput => "numeric inputs must be finite",
            Self::InvalidTolerance => "tolerance must be finite and positive",
            Self::ZeroMaxIterations => "max_iterations must be greater than zero",
            Self::IntervalDoesNotBracketRoot => {
                "function(a) and function(b) must have opposite signs"
            }
            Self::ZeroDerivative => "derivative evaluated to zero",
            Self::NonFiniteIterate => "the method produced a non-finite iterate",
            Self::DegenerateSecantDenominator => {
                "secant method encountered nearly identical function values"
            }
            Self::DidNotConverge => {
                "the method did not converge within the maximum number of iterations"
            }
        };

        formatter.write_str(message)
    }
}

impl std::error::Error for RootSolveError {}

/// Shared convergence controls for scalar root-finding methods.
#[derive(Debug, Clone, Copy, PartialEq)]
pub struct RootSolveConfig {
    tolerance: f64,
    max_iterations: NonZeroUsize,
}

impl RootSolveConfig {
    /// Creates a validated root-solving configuration.
    pub fn new(tolerance: f64, max_iterations: NonZeroUsize) -> Result<Self, RootSolveError> {
        if !tolerance.is_finite() || tolerance <= 0.0 {
            return Err(RootSolveError::InvalidTolerance);
        }

        Ok(Self {
            tolerance,
            max_iterations,
        })
    }

    /// Returns the positive convergence tolerance.
    pub const fn tolerance(self) -> f64 {
        self.tolerance
    }

    /// Returns the non-zero iteration budget.
    pub const fn max_iterations(self) -> NonZeroUsize {
        self.max_iterations
    }
}

/// Provides bisection, Newton-Raphson, and secant root-finding methods.
pub struct BisectionLibrary;

impl BisectionLibrary {
    /// Approximates a root using the bisection method on an interval with opposite endpoint signs.
    pub fn bisection<F>(
        value_a: f64,
        value_b: f64,
        tolerance: f64,
        max_iterations: usize,
        math_function: F,
    ) -> Result<f64, RootSolveError>
    where
        F: Fn(f64) -> f64,
    {
        let max_iterations =
            NonZeroUsize::new(max_iterations).ok_or(RootSolveError::ZeroMaxIterations)?;
        let config = RootSolveConfig::new(tolerance, max_iterations)?;

        Self::bisection_with_config(value_a, value_b, config, math_function)
    }

    /// Approximates a root using the bisection method with a validated configuration.
    pub fn bisection_with_config<F>(
        mut value_a: f64,
        mut value_b: f64,
        config: RootSolveConfig,
        math_function: F,
    ) -> Result<f64, RootSolveError>
    where
        F: Fn(f64) -> f64,
    {
        if !value_a.is_finite() || !value_b.is_finite() {
            return Err(RootSolveError::NonFiniteInput);
        }

        let mut function_a = math_function(value_a);
        let function_b = math_function(value_b);

        if function_a.abs() <= config.tolerance() {
            return Ok(value_a);
        }
        if function_b.abs() <= config.tolerance() {
            return Ok(value_b);
        }

        if function_a * function_b > 0.0 {
            return Err(RootSolveError::IntervalDoesNotBracketRoot);
        }

        for _ in 0..config.max_iterations().get() {
            let value_c = (value_a + value_b) / 2.0;
            let function_c = math_function(value_c);

            if function_c.abs() <= config.tolerance()
                || (value_b - value_a).abs() / 2.0 <= config.tolerance()
            {
                return Ok(value_c);
            }

            if function_a * function_c < 0.0 {
                value_b = value_c;
            } else {
                value_a = value_c;
                function_a = function_c;
            }
        }

        Err(RootSolveError::DidNotConverge)
    }

    /// Approximates a root using the Newton-Raphson method.
    pub fn newton_raphson<F, G>(
        x: f64,
        tolerance: f64,
        max_iterations: usize,
        math_function: F,
        math_function_derivative: G,
    ) -> Result<f64, RootSolveError>
    where
        F: Fn(f64) -> f64,
        G: Fn(f64) -> f64,
    {
        let max_iterations =
            NonZeroUsize::new(max_iterations).ok_or(RootSolveError::ZeroMaxIterations)?;
        let config = RootSolveConfig::new(tolerance, max_iterations)?;

        Self::newton_raphson_with_config(x, config, math_function, math_function_derivative)
    }

    /// Approximates a root using the Newton-Raphson method with a validated configuration.
    pub fn newton_raphson_with_config<F, G>(
        mut x: f64,
        config: RootSolveConfig,
        math_function: F,
        math_function_derivative: G,
    ) -> Result<f64, RootSolveError>
    where
        F: Fn(f64) -> f64,
        G: Fn(f64) -> f64,
    {
        if !x.is_finite() {
            return Err(RootSolveError::NonFiniteInput);
        }

        for _ in 0..config.max_iterations().get() {
            let function_x = math_function(x);
            let derivative_x = math_function_derivative(x);
            if derivative_x.abs() <= f64::EPSILON {
                return Err(RootSolveError::ZeroDerivative);
            }

            if function_x.abs() <= config.tolerance() {
                return Ok(x);
            }

            let x1 = x - function_x / derivative_x;
            if !x1.is_finite() {
                return Err(RootSolveError::NonFiniteIterate);
            }

            if (x1 - x).abs() < config.tolerance() {
                return Ok(x1);
            }
            x = x1;
        }

        Err(RootSolveError::DidNotConverge)
    }

    /// Approximates a root using the secant method.
    pub fn secant<F>(
        x0: f64,
        x1: f64,
        tolerance: f64,
        max_iterations: usize,
        math_function: F,
    ) -> Result<f64, RootSolveError>
    where
        F: Fn(f64) -> f64,
    {
        let max_iterations =
            NonZeroUsize::new(max_iterations).ok_or(RootSolveError::ZeroMaxIterations)?;
        let config = RootSolveConfig::new(tolerance, max_iterations)?;

        Self::secant_with_config(x0, x1, config, math_function)
    }

    /// Approximates a root using the secant method with a validated configuration.
    pub fn secant_with_config<F>(
        mut x0: f64,
        mut x1: f64,
        config: RootSolveConfig,
        math_function: F,
    ) -> Result<f64, RootSolveError>
    where
        F: Fn(f64) -> f64,
    {
        if !x0.is_finite() || !x1.is_finite() {
            return Err(RootSolveError::NonFiniteInput);
        }

        for _ in 0..config.max_iterations().get() {
            let function_x0 = math_function(x0);
            let function_x1 = math_function(x1);

            if function_x1.abs() <= config.tolerance() {
                return Ok(x1);
            }

            let denominator = function_x1 - function_x0;
            if denominator.abs() <= f64::EPSILON {
                return Err(RootSolveError::DegenerateSecantDenominator);
            }

            let x2 = x1 - function_x1 * (x1 - x0) / denominator;
            if !x2.is_finite() {
                return Err(RootSolveError::NonFiniteIterate);
            }
            if (x2 - x1).abs() <= config.tolerance() {
                return Ok(x2);
            }

            x0 = x1;
            x1 = x2;
        }

        Err(RootSolveError::DidNotConverge)
    }

    /// Alias for [`Self::bisection`].
    pub fn binomial<F>(
        value_a: f64,
        value_b: f64,
        tolerance: f64,
        max_iterations: usize,
        math_function: F,
    ) -> Result<f64, RootSolveError>
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
