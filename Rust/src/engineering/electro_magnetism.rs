//! Electromagnetism formulas for fields, flux, and capacitance.

/// Provides helper formulas for introductory electromagnetism calculations.
pub struct ElectroMagnetism;

impl ElectroMagnetism {
    /// The permittivity of free space in SI units.
    pub const VACUUM_PERMITTIVITY: f64 = 8.854_187_812_8e-12;
    /// Coulomb's constant in SI units.
    pub const COULOMB_CONSTANT: f64 = 8.987_551_792_3e9;

    /// Computes electric field from a potential gradient.
    pub fn electric_field_potential_gradient(potential_difference: f64, distance: f64) -> f64 {
        -potential_difference / distance
    }

    /// Computes electric flux density.
    pub fn electric_flux_density(electric_field: f64, permittivity: f64) -> f64 {
        permittivity * electric_field
    }

    /// Computes absolute permittivity from flux density and electric field.
    pub fn absolute_permittivity(electric_flux_density: f64, electric_field: f64) -> f64 {
        electric_flux_density / electric_field
    }

    /// Computes electric dipole moment.
    pub fn electric_dipole_moment(charge: f64, distance: f64) -> f64 {
        charge * distance
    }

    /// Computes electric polarization.
    pub fn electric_polarization(electric_dipole_moment: f64, volume: f64) -> f64 {
        electric_dipole_moment / volume
    }

    /// Computes electric displacement field.
    pub fn electric_displacement_field(electric_field: f64, permittivity: f64) -> f64 {
        permittivity * electric_field
    }

    /// Computes electric displacement flux.
    pub fn electric_displacement_flux(electric_displacement_field: f64, area: f64) -> f64 {
        electric_displacement_field * area
    }

    /// Computes electric potential from field and distance.
    pub fn absolute_electric_potential(electric_field: f64, distance: f64) -> f64 {
        electric_field * distance
    }

    /// Computes electrostatic force using Coulomb's law.
    pub fn coulombs_law_force(charge_1: f64, charge_2: f64, separation_distance: f64) -> f64 {
        Self::COULOMB_CONSTANT * charge_1 * charge_2 / separation_distance.powi(2)
    }

    /// Computes capacitance from charge and potential difference.
    pub fn capacitance(charge: f64, potential_difference: f64) -> f64 {
        charge / potential_difference
    }
}

#[cfg(test)]
mod tests {
    use super::ElectroMagnetism;

    #[test]
    fn field_and_flux_density_relationships_are_consistent() {
        let field = ElectroMagnetism::electric_field_potential_gradient(12.0, 3.0);
        let flux_density = ElectroMagnetism::electric_flux_density(2.0, 5.0);
        let permittivity = ElectroMagnetism::absolute_permittivity(10.0, 2.0);

        assert!((field + 4.0).abs() < 1e-12);
        assert!((flux_density - 10.0).abs() < 1e-12);
        assert!((permittivity - 5.0).abs() < 1e-12);
    }

    #[test]
    fn dipole_and_coulomb_helpers_produce_expected_results() {
        assert!((ElectroMagnetism::electric_dipole_moment(2.0, 0.5) - 1.0).abs() < 1e-12);
        assert!((ElectroMagnetism::electric_polarization(1.0, 0.25) - 4.0).abs() < 1e-12);
        assert!(ElectroMagnetism::coulombs_law_force(1e-6, 2e-6, 0.1) > 1.0);
    }
}
