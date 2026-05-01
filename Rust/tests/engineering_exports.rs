use open_math_library::{
    AngularFrequencies, CivilEngineeringLibrary, ClassicalMechanics, DerivedDynamicQuantities,
    ElectroMagnetism, FluidDynamics, GeneralEnergyDefinitions,
};

#[test]
fn engineering_exports_are_available_from_the_crate_root() {
    let angular_frequency = AngularFrequencies::angular_frequency_from_frequency(2.0);
    let slope = CivilEngineeringLibrary::slope_as_ratio(3.0, 4.0);
    let kinetic = ClassicalMechanics::translational_kinetic_energy(2.0, 5.0);
    let momentum = DerivedDynamicQuantities::momentum(3.0, 4.0);
    let flux_density = ElectroMagnetism::electric_flux_density(2.0, 3.0);
    let mass_flow = FluidDynamics::mass_flow_rate(1000.0, 0.01);
    let energy = GeneralEnergyDefinitions::mechanical_energy(10.0, 5.0);

    assert!(angular_frequency > 12.0);
    assert!((slope - 0.75).abs() < 1e-12);
    assert!((kinetic - 25.0).abs() < 1e-12);
    assert!((momentum - 12.0).abs() < 1e-12);
    assert!((flux_density - 6.0).abs() < 1e-12);
    assert!((mass_flow - 10.0).abs() < 1e-12);
    assert!((energy - 15.0).abs() < 1e-12);
}

#[test]
fn engineering_exports_preserve_core_relationships() {
    let angular_frequency = AngularFrequencies::angular_frequency_from_frequency(3.5);
    let recovered_frequency =
        AngularFrequencies::frequency_from_angular_frequency(angular_frequency);
    let period = AngularFrequencies::period_from_angular_frequency(-angular_frequency);

    let moment = ClassicalMechanics::moment_of_inertia(3.0, 2.0);
    let radius = ClassicalMechanics::radius_of_gyration(moment, 3.0);

    let velocity = FluidDynamics::flow_velocity(0.5, 0.4);
    let volume_flux = FluidDynamics::volume_flux(velocity, 0.2);

    let displacement = ElectroMagnetism::electric_displacement_field(2.0, 3.0);
    let flux_density = ElectroMagnetism::electric_flux_density(2.0, 3.0);

    let work = GeneralEnergyDefinitions::mechanical_work(5.0, 4.0, 0.0);
    let power = GeneralEnergyDefinitions::power_from_work(work, 2.0);

    assert!((recovered_frequency - 3.5).abs() < 1e-12);
    assert!((period - (1.0 / 3.5)).abs() < 1e-12);
    assert!((radius - 2.0).abs() < 1e-12);
    assert!((volume_flux - 0.5).abs() < 1e-12);
    assert!((displacement - flux_density).abs() < 1e-12);
    assert!((power - 10.0).abs() < 1e-12);
}

#[test]
fn engineering_exports_preserve_flux_density_and_flow_identities() {
    let radius = 0.4;
    let density = 998.0;
    let velocity = 3.0;
    let area = std::f64::consts::PI * radius * radius;

    let volume_flux = FluidDynamics::volume_flux(velocity, radius);
    let mass_flow = FluidDynamics::mass_flow_rate(density, volume_flux);
    let current_density = FluidDynamics::mass_current_density(density, velocity);

    let pressure = 1_500.0;
    let gravity = 9.81;
    let height = 2.0;
    let bernoulli =
        FluidDynamics::bernoullis_equation(pressure, density, velocity, height, gravity);
    let euler = FluidDynamics::eulers_equations(pressure, density, velocity, height, gravity);

    let field = 4.0;
    let permittivity = 3.5;
    let displacement = ElectroMagnetism::electric_displacement_field(field, permittivity);
    let recovered_permittivity = ElectroMagnetism::absolute_permittivity(displacement, field);
    let displacement_flux = ElectroMagnetism::electric_displacement_flux(displacement, area);

    assert!((mass_flow - current_density * area).abs() < 1e-12);
    assert!((bernoulli - density * euler).abs() < 1e-9);
    assert!((recovered_permittivity - permittivity).abs() < 1e-12);
    assert!((displacement_flux - displacement * area).abs() < 1e-12);
}

#[test]
fn engineering_exports_link_force_work_and_material_splits() {
    let mass = 3.0;
    let acceleration = 2.0;
    let time = 4.0;
    let displacement = 10.0;

    let force = DerivedDynamicQuantities::force(mass, acceleration);
    let impulse = DerivedDynamicQuantities::impulse(force, time);
    let momentum = DerivedDynamicQuantities::momentum(mass, acceleration * time);
    let work = DerivedDynamicQuantities::work(force, displacement, 0.0);
    let average_velocity = displacement / time;
    let power_from_work = GeneralEnergyDefinitions::power_from_work(work, time);
    let mechanical_power = GeneralEnergyDefinitions::mechanical_power(force, average_velocity);

    let slope_ratio = CivilEngineeringLibrary::slope_as_ratio(3.0, 4.0);
    let slope_percentage = CivilEngineeringLibrary::slope_as_percentage(3.0, 4.0);
    let total_parts = CivilEngineeringLibrary::concrete_mix_total_parts(1.0, 2.0, 4.0);
    let dry_volume = 1.54;
    let cement = CivilEngineeringLibrary::concrete_component_quantity(dry_volume, 1.0, total_parts);
    let sand = CivilEngineeringLibrary::concrete_component_quantity(dry_volume, 2.0, total_parts);
    let aggregate =
        CivilEngineeringLibrary::concrete_component_quantity(dry_volume, 4.0, total_parts);

    assert!((impulse - momentum).abs() < 1e-12);
    assert!((power_from_work - mechanical_power).abs() < 1e-12);
    assert!((slope_percentage - slope_ratio * 100.0).abs() < 1e-12);
    assert!((cement + sand + aggregate - dry_volume).abs() < 1e-12);
}
