# OpenMathLibrary

`open_math_library` is a Rust crate that collects reusable formulas and numerical helpers for
actuarial mathematics, arithmetic, and engineering calculations.

## Modules

- `actuarial_and_arithmetic`
  - `ActuarialLibrary`
  - `ArithmeticLibrary`
  - `BisectionLibrary`
  - `PiecewiseSpecialFunctionsLibrary`
- `engineering`
  - `AngularFrequencies`
  - `CivilEngineeringLibrary`
  - `ClassicalMechanics`
  - `DerivedDynamicQuantities`
  - `ElectroMagnetism`
  - `FluidDynamics`
  - `GeneralEnergyDefinitions`

## Project Structure

```text
Rust/
├── Cargo.toml
├── CODING_STANDARDS.md
├── README.md
├── src/
│   ├── actuarial_and_arithmetic/
│   │   ├── actuarial_library.rs
│   │   ├── arithmetic_library.rs
│   │   ├── bisection_library.rs
│   │   ├── mod.rs
│   │   └── piecewise_special_functions_library.rs
│   ├── engineering/
│   │   ├── angular_frequencies.rs
│   │   ├── civil_engineering_library.rs
│   │   ├── classical_mechanics.rs
│   │   ├── derived_dynamic_quantities.rs
│   │   ├── electro_magnetism.rs
│   │   ├── fluid_dynamics.rs
│   │   ├── general_energy_definitions.rs
│   │   └── mod.rs
│   ├── bin/
│   │   └── toroid_vulkan_demo.rs
│   └── lib.rs
└── tests/
```

## Usage

Add the crate to your project and import the exported types from the crate root:

```rust
use open_math_library::{ActuarialLibrary, ClassicalMechanics};

fn main() {
    let future_value = ActuarialLibrary::future_value(1_000.0, 0.05, 2).unwrap();
    let kinetic_energy = ClassicalMechanics::translational_kinetic_energy(4.0, 3.0);

    assert!(future_value > 1_100.0);
    assert!((kinetic_energy - 18.0).abs() < 1e-12);
}
```

## Development Checks

Run the standard quality gates before committing changes:

```powershell
cargo fmt --check
cargo clippy --all-targets -- -D warnings
cargo test
cargo doc --no-deps
```

## Vulkan Toroid Demo

Phase 3 adds an optional demo binary that renders a static toroid with `vulkano`.

On Windows, make sure Rust can find the MSVC toolchain and Vulkan runtime before building the
demo.

```powershell
cargo run --features vulkan-demo --bin toroid_vulkan_demo
```

## Coding Standards

Project-specific Rust conventions are documented in `CODING_STANDARDS.md`.

## License

This project is licensed under the MIT License.
