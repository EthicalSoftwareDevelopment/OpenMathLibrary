//! Classical mechanics formulas for mass, energy, and gravitation.

/// Provides classical mechanics helper formulas.
pub struct ClassicalMechanics;

impl ClassicalMechanics {
    /// The universal gravitational constant in SI units.
    pub const GRAVITATIONAL_CONSTANT: f64 = 6.67430e-11;

    /// Computes linear mass density.
    pub fn linear_mass_density(mass: f64, length: f64) -> f64 {
        mass / length
    }

    /// Computes surface mass density.
    pub fn surface_mass_density(mass: f64, area: f64) -> f64 {
        mass / area
    }

    /// Computes volumetric mass density.
    pub fn volumetric_mass_density(mass: f64, volume: f64) -> f64 {
        mass / volume
    }

    /// Computes the mass moment about a radius.
    pub fn moment_of_mass(mass: f64, radius: f64) -> f64 {
        mass * radius * radius
    }

    /// Computes the center of mass of a two-body system along one axis.
    pub fn center_of_mass(mass1: f64, mass2: f64, radius1: f64, radius2: f64) -> f64 {
        (mass1 * radius1 + mass2 * radius2) / (mass1 + mass2)
    }

    /// Computes the reduced mass of a two-body system.
    pub fn reduced_mass(mass1: f64, mass2: f64) -> f64 {
        (mass1 * mass2) / (mass1 + mass2)
    }

    /// Computes the moment of inertia of a point mass.
    pub fn moment_of_inertia(mass: f64, radius: f64) -> f64 {
        mass * radius * radius
    }

    /// Computes the radius of gyration.
    pub fn radius_of_gyration(moment_of_inertia: f64, mass: f64) -> f64 {
        (moment_of_inertia / mass).sqrt()
    }

    /// Computes translational kinetic energy.
    pub fn translational_kinetic_energy(mass: f64, velocity: f64) -> f64 {
        0.5 * mass * velocity * velocity
    }

    /// Computes the gravitational force between two point masses.
    pub fn gravitational_force(mass1: f64, mass2: f64, separation_distance: f64) -> f64 {
        Self::GRAVITATIONAL_CONSTANT * mass1 * mass2 / separation_distance.powi(2)
    }
}

#[cfg(test)]
mod tests {
    use super::ClassicalMechanics;

    #[test]
    fn mass_density_and_mass_moments_are_consistent() {
        assert!((ClassicalMechanics::linear_mass_density(10.0, 2.0) - 5.0).abs() < 1e-12);
        assert!((ClassicalMechanics::center_of_mass(2.0, 1.0, 0.0, 3.0) - 1.0).abs() < 1e-12);
        assert!((ClassicalMechanics::moment_of_inertia(2.0, 3.0) - 18.0).abs() < 1e-12);
    }

    #[test]
    fn energy_and_gravity_helpers_produce_expected_values() {
        assert!((ClassicalMechanics::translational_kinetic_energy(4.0, 3.0) - 18.0).abs() < 1e-12);
        assert!((ClassicalMechanics::radius_of_gyration(18.0, 2.0) - 3.0).abs() < 1e-12);
        assert!(ClassicalMechanics::gravitational_force(5.0, 10.0, 2.0) > 0.0);
    }
}
