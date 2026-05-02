# TheOpenMathLibrary (C#)

TheOpenMathLibrary is a .NET 8 solution that organizes reusable mathematical code into focused class-library projects for actuarial calculators, engineering formulas, general mathematics, and an experimental Vulkan toroid demo.

## Projects

The solution currently contains four primary C# projects:

- `TheOpenMathLibrary.ActuarialCalculators`
- `TheOpenMathLibrary.Engineering`
- `TheOpenMathLibrary.GeneralMathematics`
- `TheOpenMathLibrary.GraphicsDemo`

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

### `TheOpenMathLibrary.GraphicsDemo`
This project is a standalone, demo-only Vulkan renderer that draws a toroid using Silk.NET, GLFW, and renderer-local mesh generation.

Representative files:
- `GraphicsDemoApp.cs`
- `OrbitCamera.cs`
- `Geometry/ToroidMeshGenerator.cs`
- `Rendering/VulkanRenderer.cs`
- `Rendering/ShaderSources.cs`

Examples of functionality:
- Vulkan instance, surface, device, and swapchain setup
- renderer-local toroid vertex, normal, and index generation
- minimal Lambert-style shading for shape readability
- basic orbit camera controls for inspecting the toroid

## Target Framework

All current projects target:

- `.NET 8.0`

## Build

From the `C#` solution root, you can restore and build with:

```powershell
dotnet restore .\TheOpenMathLibrary.sln
dotnet build .\TheOpenMathLibrary.sln
```

## Test Projects

The solution now includes dedicated MSTest projects that mirror the three production libraries and the graphics demo:

- `TheOpenMathLibrary.ActuarialCalculators.Tests`
- `TheOpenMathLibrary.Engineering.Tests`
- `TheOpenMathLibrary.GeneralMathematics.Tests`
- `TheOpenMathLibrary.GraphicsDemo.Tests`

These tests cover formula correctness, edge cases, regression-sensitive aliases, invalid-input guards, and renderer-local toroid mesh generation.

## Test

From the `C#` solution root, run the full automated test suite with:

```powershell
dotnet test .\TheOpenMathLibrary.sln
```

To run a single test project during development, use its project file directly. For example:

```powershell
dotnet test .\TheOpenMathLibrary.Engineering.Tests\TheOpenMathLibrary.Engineering.Tests.csproj
```

To run the Vulkan toroid demo during development, use:

```powershell
dotnet run --project .\TheOpenMathLibrary.GraphicsDemo\TheOpenMathLibrary.GraphicsDemo.csproj
```

The demo requires a Vulkan-capable runtime and remains intentionally separate from the reusable math libraries.

## Notes

The solution already has a useful foundation, but it is still in an early growth phase. Good next improvements include:

- expanding test coverage
- improving XML documentation comments
- standardizing namespaces across projects
- validating formulas and edge cases
- expanding numerical and scientific functionality

## Roadmap

See `ROADMAP.md` for planned next steps, including:

- C# math libraries
- unit tests
- Vulkan rendering of a toroid
- shading dynamics of the toroid as if self-sustaining flow

