pub struct GeneralEnergyDefinitions;

impl GeneralEnergyDefinitions {
    pub fn mechanical_work(force: f64, displacement: f64, angle: f64) -> f64 {
        force * displacement * angle.cos()
    }

    pub fn work_done_on_mechanical_system(force: f64, displacement: f64, angle: f64) -> f64 {
        force * displacement * angle.cos()
    }

    pub fn potential_energy(mass: f64, height: f64, gravity: f64) -> f64 {
        mass * height * gravity
    }

    pub fn kinetic_energy(mass: f64, velocity: f64) -> f64 {
        0.5 * mass * velocity * velocity
    }

    pub fn rotational_kinetic_energy(moment_of_inertia: f64, angular_velocity: f64) -> f64 {
        0.5 * moment_of_inertia * angular_velocity * angular_velocity
    }

    pub fn spring_potential_energy(spring_constant: f64, displacement: f64) -> f64 {
        0.5 * spring_constant * displacement * displacement
    }

    pub fn mechanical_energy(kinetic_energy: f64, potential_energy: f64) -> f64 {
        kinetic_energy + potential_energy
    }

    pub fn mechanical_power(force: f64, velocity: f64) -> f64 {
        force * velocity
    }

    pub fn power_from_work(work: f64, time: f64) -> f64 {
        work / time
    }
}

#[cfg(test)]
mod tests {
    use super::GeneralEnergyDefinitions;

    #[test]
    fn work_and_energy_formulas_match_standard_results() {
        let work = GeneralEnergyDefinitions::mechanical_work(10.0, 2.0, 0.0);
        let kinetic = GeneralEnergyDefinitions::kinetic_energy(4.0, 3.0);
        let potential = GeneralEnergyDefinitions::potential_energy(2.0, 5.0, 9.81);

        assert!((work - 20.0).abs() < 1e-12);
        assert!((kinetic - 18.0).abs() < 1e-12);
        assert!((potential - 98.1).abs() < 1e-12);
    }

    #[test]
    fn rotational_and_power_helpers_are_consistent() {
        let rotational = GeneralEnergyDefinitions::rotational_kinetic_energy(2.0, 3.0);
        let spring = GeneralEnergyDefinitions::spring_potential_energy(10.0, 0.5);
        let power = GeneralEnergyDefinitions::power_from_work(100.0, 4.0);

        assert!((rotational - 9.0).abs() < 1e-12);
        assert!((spring - 1.25).abs() < 1e-12);
        assert!((power - 25.0).abs() < 1e-12);
    }
}