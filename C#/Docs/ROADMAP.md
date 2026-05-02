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

**Objective:** Build a standalone Vulkan demo that renders a toroid using a Silk.NET-style integration path, while keeping rendering concerns separate from the core math libraries.

### Tasks
- Create a separate demo application such as `TheOpenMathLibrary.GraphicsDemo` instead of extending the existing math libraries
- Use a Silk.NET-style Vulkan wrapper and windowing path for instance, device, surface, swapchain, and presentation setup
- Establish early rendering milestones: open a window, clear the framebuffer, and render a simple triangle before toroid-specific work
- Generate the toroid mesh locally inside the demo project using configurable major radius, minor radius, and segment counts
- Compute renderer-local vertex positions, normals, and triangle indices rather than moving toroid generation into `TheOpenMathLibrary.GeneralMathematics`
- Upload toroid mesh data to GPU buffers and implement the graphics pipeline needed to draw it reliably
- Add minimal shading sufficient for shape readability, such as flat coloring or simple Lambert-style lighting
- Add basic camera orbit, projection, transform controls, and resize-aware scene setup for inspecting the toroid
- Document Vulkan prerequisites, runtime assumptions, and demo-only scope so the work stays experimental and intentionally non-reusable

## Phase 4. Shading Dynamics of the Toroid as if Self-Sustaining Flow

**Objective:** Extend the toroid demo with advanced animated shading effects after the basic Vulkan rendering path is working.

### Tasks
- Build on the Phase 3 toroid demo rather than redesigning the renderer architecture
- Develop animated shading patterns that suggest self-sustaining flow across the toroid surface
- Integrate time-based uniforms, material parameters, and tuning controls into the existing Vulkan demo pipeline
- Experiment with richer lighting and shading behavior beyond the minimal shading introduced in Phase 3
- Optimize enough for smooth interactive playback while keeping the work experimental in scope

## Supporting Work

In parallel with the roadmap above, the C# solution should continue improving its core foundation:

- expand the existing actuarial, engineering, and general mathematics libraries
- standardize namespaces and project structure
- improve XML documentation comments
- validate formulas against trusted references
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

