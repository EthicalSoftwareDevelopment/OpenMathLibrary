pub struct AngularFrequencies;

impl AngularFrequencies {
    pub fn angular_frequency_from_frequency(frequency: f64) -> f64 {
        2.0 * std::f64::consts::PI * frequency
    }

    pub fn frequency_from_angular_frequency(angular_frequency: f64) -> f64 {
        angular_frequency / (2.0 * std::f64::consts::PI)
    }

    pub fn period_from_angular_frequency(angular_frequency: f64) -> f64 {
        2.0 * std::f64::consts::PI / angular_frequency.abs()
    }

    pub fn linear_undamped_unforced_oscillator(angular_frequency: f64) -> f64 {
        angular_frequency.abs()
    }

    pub fn simple_harmonic_oscillator(spring_constant: f64, mass: f64) -> f64 {
        (spring_constant / mass).sqrt()
    }

    pub fn linear_unforced_dho(natural_angular_frequency: f64, damping_ratio: f64) -> f64 {
        if damping_ratio.abs() >= 1.0 {
            0.0
        } else {
            natural_angular_frequency.abs() * (1.0 - damping_ratio.powi(2)).sqrt()
        }
    }

    pub fn low_amplitude_angular_sho(angular_frequency: f64, amplitude: f64) -> f64 {
        angular_frequency.abs() * amplitude.abs()
    }

    pub fn low_amplitude_simple_pendulum(angular_frequency: f64, amplitude: f64) -> f64 {
        angular_frequency.abs() * amplitude.abs()
    }

    pub fn simple_pendulum(length: f64, gravity: f64) -> f64 {
        (gravity / length).sqrt()
    }

    pub fn torsional_pendulum(torsion_constant: f64, moment_of_inertia: f64) -> f64 {
        (torsion_constant / moment_of_inertia).sqrt()
    }
}

#[cfg(test)]
mod tests {
    use super::AngularFrequencies;

    #[test]
    fn frequency_conversions_are_consistent() {
        let angular_frequency = AngularFrequencies::angular_frequency_from_frequency(1.0);
        let frequency = AngularFrequencies::frequency_from_angular_frequency(angular_frequency);

        assert!((angular_frequency - 2.0 * std::f64::consts::PI).abs() < 1e-12);
        assert!((frequency - 1.0).abs() < 1e-12);
    }

    #[test]
    fn oscillator_frequencies_match_standard_results() {
        let simple_harmonic = AngularFrequencies::simple_harmonic_oscillator(9.0, 1.0);
        let damped = AngularFrequencies::linear_unforced_dho(10.0, 0.6);
        let pendulum = AngularFrequencies::simple_pendulum(1.0, 9.81);

        assert!((simple_harmonic - 3.0).abs() < 1e-12);
        assert!((damped - 8.0).abs() < 1e-12);
        assert!((pendulum - 9.81_f64.sqrt()).abs() < 1e-12);
    }
}