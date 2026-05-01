//! Fluid-dynamics formulas for flow, pressure, and dimensionless quantities.

/// Provides helper formulas for introductory fluid-dynamics calculations.
pub struct FluidDynamics;

impl FluidDynamics {
    /// Computes mean flow velocity in a circular pipe from volumetric flow rate and diameter.
    pub fn flow_velocity(flow_rate: f64, pipe_diameter: f64) -> f64 {
        (4.0 * flow_rate) / (std::f64::consts::PI * pipe_diameter * pipe_diameter)
    }

    /// Computes tangential speed from angular velocity and radius.
    pub fn velocity_pseudovector(angular_velocity: f64, radius: f64) -> f64 {
        angular_velocity * radius
    }

    /// Computes volumetric flux through a circular cross-section.
    pub fn volume_flux(velocity: f64, radius: f64) -> f64 {
        velocity * std::f64::consts::PI * radius * radius
    }

    /// Computes mass current per unit volume.
    pub fn mass_current_per_volume(mass_current: f64, volume: f64) -> f64 {
        mass_current / volume
    }

    /// Computes mass flow rate.
    pub fn mass_flow_rate(density: f64, volume_flux: f64) -> f64 {
        density * volume_flux
    }

    /// Computes mass current density.
    pub fn mass_current_density(density: f64, velocity: f64) -> f64 {
        density * velocity
    }

    /// Computes momentum current density.
    pub fn momentum_current_density(density: f64, velocity: f64) -> f64 {
        density * velocity * velocity
    }

    /// Computes a pressure gradient.
    pub fn pressure_gradient(pressure_difference: f64, distance: f64) -> f64 {
        pressure_difference / distance
    }

    /// Computes buoyancy force.
    pub fn buoyancy_force(density: f64, volume: f64, gravity: f64) -> f64 {
        density * volume * gravity
    }

    /// Evaluates Bernoulli's equation in pressure form.
    pub fn bernoullis_equation(
        pressure: f64,
        density: f64,
        velocity: f64,
        height: f64,
        gravity: f64,
    ) -> f64 {
        pressure + 0.5 * density * velocity * velocity + density * gravity * height
    }

    /// Evaluates Euler's equation in specific-energy form.
    pub fn eulers_equations(
        pressure: f64,
        density: f64,
        velocity: f64,
        height: f64,
        gravity: f64,
    ) -> f64 {
        pressure / density + 0.5 * velocity * velocity + gravity * height
    }

    /// Computes convective acceleration.
    pub fn convective_acceleration(velocity: f64, velocity_gradient: f64) -> f64 {
        velocity * velocity_gradient
    }

    /// Evaluates a simplified Navier-Stokes acceleration balance.
    pub fn navier_stokes_equations(
        pressure_gradient: f64,
        density: f64,
        body_force: f64,
        velocity_laplacian: f64,
        dynamic_viscosity: f64,
    ) -> f64 {
        -pressure_gradient / density + body_force + dynamic_viscosity * velocity_laplacian / density
    }

    /// Computes the Reynolds number.
    pub fn reynolds_number(
        density: f64,
        velocity: f64,
        characteristic_length: f64,
        dynamic_viscosity: f64,
    ) -> f64 {
        density * velocity * characteristic_length / dynamic_viscosity
    }
}

#[cfg(test)]
mod tests {
    use super::FluidDynamics;

    #[test]
    fn volumetric_flow_relations_are_consistent() {
        let flow_rate = 2.0;
        let radius = 0.5;
        let diameter = 2.0 * radius;
        let velocity = FluidDynamics::flow_velocity(flow_rate, diameter);
        let reconstructed_flow_rate = FluidDynamics::volume_flux(velocity, radius);

        assert!((reconstructed_flow_rate - flow_rate).abs() < 1e-12);
    }

    #[test]
    fn bernoulli_and_reynolds_helpers_match_known_values() {
        let bernoulli = FluidDynamics::bernoullis_equation(1000.0, 2.0, 3.0, 5.0, 9.81);
        let reynolds = FluidDynamics::reynolds_number(1000.0, 2.0, 0.5, 0.001);

        assert!((bernoulli - 1107.1).abs() < 1e-9);
        assert!((reynolds - 1_000_000.0).abs() < 1e-6);
    }
}
