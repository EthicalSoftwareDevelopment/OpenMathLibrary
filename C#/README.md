# TheOpenMathLibrary (C#)

TheOpenMathLibrary is a .NET 8 solution that organizes reusable mathematical code into focused class-library projects for actuarial calculators, engineering formulas, and general mathematics.

## Projects

The solution currently contains three C# class libraries:

- `TheOpenMathLibrary.ActuarialCalculators`
- `TheOpenMathLibrary.Engineering`
- `TheOpenMathLibrary.GeneralMathematics`

## Current Structure

### `TheOpenMathLibrary.ActuarialCalculators`
This project contains mathematical utilities related to arithmetic, numerical methods, and special functions.

Representative files:
- `ArithmeticLibrary.cs`
- `BisectionLibrary.cs`
- `PiecewiseSpecialFunctionsLibrary.cs`
- `TranscendentalLibrary.cs`

Examples of functionality:
- arithmetic and number theory helpers
- bisection and Newton-Raphson root finding
- piecewise and special functions
- transcendental functions

### `TheOpenMathLibrary.Engineering`
This project contains engineering-oriented formulas and helper functions.

Representative files:
- `AngularFrequencies.cs`
- `CivilEngineeringLibrary.cs`
- `ClassicalMechanics.cs`
- `FluidDynamics.cs`
- `MechanicalOscillations.cs`
- `OpticsAndPhotonics.cs`

Examples of functionality:
- angular frequency calculations
- civil engineering calculations
- mechanics and dynamics formulas
- optics and wave-related helpers

### `TheOpenMathLibrary.GeneralMathematics`
This project contains foundational mathematics utilities.

Representative files:
- `AlgebraicLibrary.cs`
- `GeometryFunctions.cs`
- `TrigonometryLibrary.cs`
- `WaveTheory.cs`

Examples of functionality:
- algebraic helpers
- geometry calculations
- trigonometric functions
- wave theory formulas

## Target Framework

All current projects target:

- `.NET 8.0`

## Build

From the `C#` solution root, you can restore and build with:

```powershell
dotnet restore .\TheOpenMathLibrary.sln
dotnet build .\TheOpenMathLibrary.sln
```

## Testing Status

There are currently no dedicated C# test projects in this workspace.

Adding unit tests is one of the highest-priority next steps for improving reliability and maintainability.

## Notes

The solution already has a useful foundation, but it is still in an early growth phase. Good next improvements include:

- adding unit tests
- improving XML documentation comments
- standardizing namespaces across projects
- validating formulas and edge cases
- expanding numerical and scientific functionality

## Roadmap

See `ROADMAP.md` for planned next steps, including:

- Rust math libraries
- unit tests
- Vulkan rendering of a toroid
- shading dynamics of the toroid as if self-sustaining flow

