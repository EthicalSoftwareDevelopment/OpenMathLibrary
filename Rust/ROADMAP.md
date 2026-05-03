# Roadmap

## Phase 1. Rust Math Libraries
- **Objective**: Develop a comprehensive suite of mathematical tools and utilities.
- **Tasks**:
  - Implement core modules for actuarial and engineering mathematics.
  - Add utility functions for common mathematical operations.
  - Ensure modularity and scalability for future extensions.

## Phase 2. Unit Tests
- **Objective**: Ensure the reliability and correctness of the library.
- **Tasks**:
  - Write unit tests for each module and function.
  - Achieve high code coverage.
  - Use Rust's built-in testing framework.

## Phase 3. Vulkan Rendering of a Toroid
- **Objective**: Visualize a toroid using Vulkan.
- **Tasks**:
  - Set up Vulkan for rendering.
    - Research Vulkan setup and initialization.
    - Configure Vulkan instance, device, and swapchain.
    - Use `vulkano` for the initial deliverable to reduce setup time and integration risk.
  - Create a 3D model of a toroid.
    - Design mathematical representation of a toroid.
    - Use vertex and index buffers for 3D modeling.
  - Implement rendering pipeline.
    - Develop shaders for rendering.
    - Integrate pipeline stages (vertex input, rasterization, etc.).
  - Deliver the first demo as a static render only.
    - Keep the toroid pose and lighting deterministic for validation.
    - Treat Phase 3 as a focused demo target rather than a full application.
  - Add UI scaffolding for future shader controls.
    - Reserve a far-right control panel in the demo window.
    - Add a draggable slider placeholder for future shader parameters.
    - Keep the slider decoupled from shader logic until Phase 4 begins.

### Phase 3 current deliverable
- Static toroid rendering demo implemented as `src/bin/toroid_vulkan_demo.rs`.
- Right-side control panel and placeholder slider added for future shader-flow controls.
- Intended run mode: explicit demo launch via Cargo or IDE run configuration.

## Phase 4. Shading Dynamics of the Toroid as if Self-Sustaining Flow
- **Objective**: Simulate and render dynamic shading effects.
- **Tasks**:
  - Develop algorithms for shading dynamics.
  - Integrate shading with the Vulkan rendering pipeline.
  - Connect the Phase 3 slider state to shader uniforms or push constants.
  - Optimize for real-time performance.
