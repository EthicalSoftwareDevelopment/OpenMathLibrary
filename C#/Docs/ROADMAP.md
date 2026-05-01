# Roadmap

This roadmap outlines the near-term and longer-term direction for the C# OpenMathLibrary workspace.

## Phase 1. C# Math Libraries

**Objective:** Develop a comprehensive suite of mathematical tools and utilities.

### Tasks
- Implement core modules for actuarial and engineering mathematics
- Add utility functions for common mathematical operations
- Ensure modularity and scalability for future extensions
- Explore whether C# should act as a companion library, a performance-focused backend, or a parallel implementation

## Phase 2. Unit Tests

**Objective:** Ensure the reliability and correctness of the library.

### Tasks
- Write unit tests for each module and function
- Achieve high code coverage
- Use C#'s built-in testing framework and `dotnet test`
- Add regression tests for bugs and edge cases
- Introduce automated test execution as the codebase grows

## Phase 3. Vulkan Rendering of a Toroid

**Objective:** Visualize a toroid using Vulkan.

### Tasks
- Set up Vulkan for rendering
- Create a 3D model of a toroid
- Implement rendering pipeline
- Add camera controls and scene setup
- Keep rendering work separate from the core math libraries

## Phase 4. Shading Dynamics of the Toroid as if Self-Sustaining Flow

**Objective:** Simulate and render dynamic shading effects.

### Tasks
- Develop algorithms for shading dynamics
- Integrate shading with the Vulkan rendering pipeline
- Optimize for real-time performance
- Add time-based animation and tuning controls

## Supporting Work

In parallel with the roadmap above, the C# solution should continue improving its core foundation:

- expand the existing actuarial, engineering, and general mathematics libraries
- standardize namespaces and project structure
- improve XML documentation comments
- validate formulas against tC#ed references
- improve developer documentation and examples

## Priority Suggestion

### Near-Term
1. Unit Tests
2. Core library cleanup and expansion
3. Documentation improvements

### Mid-Term
4. C# Math Libraries

### Experimental / Long-Term
5. Vulkan Rendering of a Toroid
6. Shading Dynamics of the Toroid as if Self-Sustaining Flow

