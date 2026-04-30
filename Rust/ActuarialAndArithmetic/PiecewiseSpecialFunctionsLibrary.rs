pub struct PiecewiseSpecialFunctionsLibrary;

impl PiecewiseSpecialFunctionsLibrary {
    const EPSILON: f64 = 1e-12;

    fn is_close(value_a: f64, value_b: f64) -> bool {
        (value_a - value_b).abs() <= Self::EPSILON
    }

    fn phase(x: f64) -> f64 {
        x - x.floor()
    }

    pub fn indicator_function(x: f64) -> f64 {
        if Self::is_close(x, 0.0) {
            1.0
        } else {
            0.0
        }
    }

    pub fn sign_function(x: f64) -> f64 {
        if x > Self::EPSILON {
            1.0
        } else if x < -Self::EPSILON {
            -1.0
        } else {
            0.0
        }
    }

    pub fn step_function(x: f64) -> f64 {
        if x < -Self::EPSILON {
            0.0
        } else if Self::is_close(x, 0.0) {
            0.5
        } else {
            1.0
        }
    }

    pub fn heaviside_step_function(x: f64) -> f64 {
        if x < 0.0 {
            0.0
        } else {
            1.0
        }
    }

    pub fn rectangular_function(x: f64) -> f64 {
        if x < -0.5 - Self::EPSILON || x > 0.5 + Self::EPSILON {
            0.0
        } else if Self::is_close(x, -0.5) || Self::is_close(x, 0.5) {
            0.5
        } else {
            1.0
        }
    }

    pub fn sawtooth_function(x: f64) -> f64 {
        Self::phase(x)
    }

    pub fn triangle_wave_function(x: f64) -> f64 {
        1.0 - 4.0 * (Self::phase(x) - 0.5).abs()
    }

    pub fn square_wave_function(x: f64) -> f64 {
        let phase = Self::phase(x);

        if Self::is_close(phase, 0.0) || Self::is_close(phase, 0.5) {
            0.0
        } else if phase < 0.5 {
            1.0
        } else {
            -1.0
        }
    }

    pub fn sinc_function(x: f64) -> f64 {
        if Self::is_close(x, 0.0) {
            1.0
        } else {
            x.sin() / x
        }
    }

    pub fn dirichlet_kernel(x: f64, n: u32) -> f64 {
        let half_x = 0.5 * x;

        if half_x.sin().abs() <= Self::EPSILON {
            (2 * n + 1) as f64
        } else {
            (((n as f64) + 0.5) * x).sin() / half_x.sin()
        }
    }
}

#[cfg(test)]
mod tests {
    use super::PiecewiseSpecialFunctionsLibrary;

    #[test]
    fn elementary_piecewise_functions_match_expected_values() {
        assert_eq!(PiecewiseSpecialFunctionsLibrary::indicator_function(0.0), 1.0);
        assert_eq!(PiecewiseSpecialFunctionsLibrary::sign_function(-3.0), -1.0);
        assert_eq!(PiecewiseSpecialFunctionsLibrary::step_function(0.0), 0.5);
        assert_eq!(PiecewiseSpecialFunctionsLibrary::heaviside_step_function(-1.0), 0.0);
        assert_eq!(PiecewiseSpecialFunctionsLibrary::rectangular_function(0.25), 1.0);
    }

    #[test]
    fn periodic_waves_are_shaped_correctly() {
        assert!((PiecewiseSpecialFunctionsLibrary::sawtooth_function(1.75) - 0.75).abs() < 1e-12);
        assert!((PiecewiseSpecialFunctionsLibrary::triangle_wave_function(0.25) - 0.0).abs() < 1e-12);
        assert_eq!(PiecewiseSpecialFunctionsLibrary::square_wave_function(0.25), 1.0);
        assert_eq!(PiecewiseSpecialFunctionsLibrary::square_wave_function(0.75), -1.0);
    }

    #[test]
    fn special_functions_handle_removable_singularities() {
        assert!((PiecewiseSpecialFunctionsLibrary::sinc_function(0.0) - 1.0).abs() < 1e-12);
        assert!((PiecewiseSpecialFunctionsLibrary::dirichlet_kernel(0.0, 3) - 7.0).abs() < 1e-12);
    }
}