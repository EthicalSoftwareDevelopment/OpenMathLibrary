pub mod actuarial_and_arithmetic;
pub mod engineering;

pub use actuarial_and_arithmetic::{
	actuarial_library::ActuarialLibrary,
	arithmetic_library::ArithmeticLibrary,
	bisection_library::BisectionLibrary,
	piecewise_special_functions_library::PiecewiseSpecialFunctionsLibrary,
};

pub use engineering::{
	angular_frequencies::AngularFrequencies,
	civil_engineering_library::CivilEngineeringLibrary,
	classical_mechanics::ClassicalMechanics,
	derived_dynamic_quantities::DerivedDynamicQuantities,
	electro_magnetism::ElectroMagnetism,
	fluid_dynamics::FluidDynamics,
	general_energy_definitions::GeneralEnergyDefinitions,
};

