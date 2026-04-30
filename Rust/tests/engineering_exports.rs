use open_math_library::{
	AngularFrequencies,
	CivilEngineeringLibrary,
	ClassicalMechanics,
	DerivedDynamicQuantities,
	ElectroMagnetism,
	FluidDynamics,
	GeneralEnergyDefinitions,
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

