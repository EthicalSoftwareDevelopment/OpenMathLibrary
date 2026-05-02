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

On Windows, make sure Rust can find the MSVC toolchain and Vulkan runtime before building or
running the demo.

```powershell
cargo run --features vulkan-demo --bin toroid_vulkan_demo
```

## Build, Test, and Run

This repository is primarily a **library crate**. There is no default application entry point for
plain `cargo run`. The main ways to work with it are:

- build the library,
- run the test suite,
- generate docs, or
- run the optional Vulkan demo binary.

### 1. Install prerequisites

1. Install Rust from [rust-lang.org](https://www.rust-lang.org/).
2. On Windows, install **Visual Studio Build Tools** with the **Desktop development with C++**
   workload so `link.exe` is available.
3. If you want to run the Vulkan demo, install a Vulkan-capable driver/runtime.

### 2. Open a terminal in the repository root

Use the folder that contains `Cargo.toml`:

```powershell
cd C:\Dev\OpenMathLibrary\Rust
```

### 3. Build the library

This command was verified successfully in this workspace:

```powershell
cargo build
```

### 4. Run the test suite

Use the following command to run all tests:

```powershell
cargo test
```

> Note: In the current Windows environment used for verification, `cargo test` fails because the
> MSVC linker `link.exe` is not installed or not on `PATH`. After installing the Visual Studio
> Build Tools, rerun the command.

### 5. Generate documentation

This command was also verified successfully in this workspace:

```powershell
cargo doc --no-deps
```

### 6. Run the Vulkan toroid demo

The demo is behind the `vulkan-demo` feature and must be launched explicitly:

```powershell
cargo run --features vulkan-demo --bin toroid_vulkan_demo
```

> Notes:
> - Plain `cargo run` is not the correct command for this repository because the only binary target
>   is `toroid_vulkan_demo`, and it requires the `vulkan-demo` feature.
> - In the current Windows environment used for verification, building the demo is blocked by the
>   missing MSVC linker `link.exe`.

## Running from the IDE Play Button

This repository can be run from a JetBrains IDE play button, but only after you choose a specific
Cargo target to run.

There is no default app entry point for the crate root, so the play button needs a run
configuration such as:

- a test target,
- a documentation build target, or
- the `toroid_vulkan_demo` binary with the `vulkan-demo` feature enabled.

### JetBrains Cargo run configuration for the demo

If you are using a JetBrains IDE with Rust/Cargo support, create a Cargo run configuration with:

- **Command**: `run`
- **Bin**: `toroid_vulkan_demo`
- **Features**: `vulkan-demo`
- **Working directory**: the folder containing `Cargo.toml`

Equivalent command:

```powershell
cargo run --features vulkan-demo --bin toroid_vulkan_demo
```

### JetBrains run configurations for common development tasks

You can also wire the play button to these Cargo commands:

```powershell
cargo build
cargo test
cargo doc --no-deps
```

### Current limitation in this workspace

In the current Windows environment used for verification, the IDE play button will still fail for
targets that need linking until `link.exe` is available through the MSVC Build Tools.

## Windows Troubleshooting

### `link.exe` not found

If Cargo reports `link.exe` not found, install **Visual Studio Build Tools** and include the
**Desktop development with C++** workload.

After installation, reopen the terminal or IDE and verify the linker is available:

```powershell
where.exe link
rustc -vV
cargo test
```

If `where.exe link` returns no result, launch the IDE from a Developer Command Prompt for Visual
Studio or restart Windows after the Build Tools installation so environment changes are picked up.

### `kernel32.lib` not found

If Cargo finds `link.exe` but the linker fails with `LNK1181: cannot open input file 'kernel32.lib'`,
the MSVC toolchain is present but the Windows SDK libraries are not available to the build.

Typical fixes:

- install or repair the **Windows 10/11 SDK** from the Visual Studio Build Tools installer,
- ensure the C++ workload includes the Windows SDK component,
- restart the IDE or terminal after installation so updated environment variables are loaded.

Quick checks:

```powershell
where.exe link
Get-ChildItem 'C:\Program Files (x86)\Windows Kits' -ErrorAction SilentlyContinue
cargo test
```

If the IDE play button still fails after installation, close and reopen the JetBrains IDE so its
run configurations inherit the refreshed MSVC and SDK environment.

### Vulkan demo does not start

For `toroid_vulkan_demo`, verify all of the following:

- the `vulkan-demo` feature is enabled,
- the system has a Vulkan-capable graphics driver/runtime,
- and the MSVC linker issue is resolved first.

Retry with:

```powershell
cargo build --features vulkan-demo --bin toroid_vulkan_demo
cargo run --features vulkan-demo --bin toroid_vulkan_demo
```

