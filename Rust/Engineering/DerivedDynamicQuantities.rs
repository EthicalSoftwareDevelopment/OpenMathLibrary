pub struct DerivedDynamicQuantities;

impl DerivedDynamicQuantities {
    pub fn momentum(mass: f64, velocity: f64) -> f64 {
        mass * velocity
    }

    pub fn force(mass: f64, acceleration: f64) -> f64 {
        mass * acceleration
    }

    pub fn impulse(force: f64, time: f64) -> f64 {
        force * time
    }

    pub fn angular_momentum(mass: f64, velocity: f64, radius: f64) -> f64 {
        mass * velocity * radius
    }

    pub fn torque(force: f64, radius: f64) -> f64 {
        force * radius
    }

    pub fn angular_impulse(torque: f64, time: f64) -> f64 {
        torque * time
    }

    pub fn work(force: f64, displacement: f64, angle_radians: f64) -> f64 {
        force * displacement * angle_radians.cos()
    }

    pub fn power(force: f64, velocity: f64, angle_radians: f64) -> f64 {
        force * velocity * angle_radians.cos()
    }

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