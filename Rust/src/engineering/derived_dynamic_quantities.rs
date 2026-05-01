//! Derived dynamic quantities such as force, work, and torque.

/// Provides formulas for linear and angular dynamic quantities.
pub struct DerivedDynamicQuantities;

impl DerivedDynamicQuantities {
    /// Computes linear momentum.
    pub fn momentum(mass: f64, velocity: f64) -> f64 {
        mass * velocity
    }

    /// Computes force from mass and acceleration.
    pub fn force(mass: f64, acceleration: f64) -> f64 {
        mass * acceleration
    }

    /// Computes impulse from force and time.
    pub fn impulse(force: f64, time: f64) -> f64 {
        force * time
    }

    /// Computes angular momentum magnitude for perpendicular radius and velocity.
    pub fn angular_momentum(mass: f64, velocity: f64, radius: f64) -> f64 {
        mass * velocity * radius
    }

    /// Computes torque magnitude.
    pub fn torque(force: f64, radius: f64) -> f64 {
        force * radius
    }

    /// Computes angular impulse.
    pub fn angular_impulse(torque: f64, time: f64) -> f64 {
        torque * time
    }

    /// Computes mechanical work using the angle between force and displacement.
    pub fn work(force: f64, displacement: f64, angle_radians: f64) -> f64 {
        force * displacement * angle_radians.cos()
    }

    /// Computes mechanical power using the angle between force and velocity.
    pub fn power(force: f64, velocity: f64, angle_radians: f64) -> f64 {
        force * velocity * angle_radians.cos()
    }

    /// Computes centripetal force.
    pub fn centripetal_force(mass: f64, velocity: f64, radius: f64) -> f64 {
        mass * velocity * velocity / radius
    }
}

#[cfg(test)]
mod tests {
    use super::DerivedDynamicQuantities;

    #[test]
    fn linear_and_angular_quantities_match_standard_formulas() {
        assert!((DerivedDynamicQuantities::momentum(3.0, 4.0) - 12.0).abs() < 1e-12);
        assert!((DerivedDynamicQuantities::torque(5.0, 2.0) - 10.0).abs() < 1e-12);
        assert!((DerivedDynamicQuantities::angular_impulse(10.0, 0.5) - 5.0).abs() < 1e-12);
    }

    #[test]
    fn work_power_and_centripetal_force_are_computed_correctly() {
        assert!((DerivedDynamicQuantities::work(10.0, 2.0, 0.0) - 20.0).abs() < 1e-12);
        assert!((DerivedDynamicQuantities::power(8.0, 3.0, 0.0) - 24.0).abs() < 1e-12);
        assert!((DerivedDynamicQuantities::centripetal_force(2.0, 3.0, 1.5) - 12.0).abs() < 1e-12);
    }
}
