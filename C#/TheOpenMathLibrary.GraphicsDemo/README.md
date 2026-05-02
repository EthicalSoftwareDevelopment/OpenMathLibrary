# TheOpenMathLibrary.GraphicsDemo

`TheOpenMathLibrary.GraphicsDemo` is the experimental Phase 3 Vulkan toroid demo for the C# workspace.

## Scope

This project is intentionally:

- demo-only
- renderer-local for toroid geometry generation
- separate from the reusable math libraries
- focused on minimal readable shading rather than advanced flow effects

## Current Features

- GLFW window creation through Silk.NET
- Vulkan instance, surface, physical device, logical device, and swapchain setup
- renderer-local toroid mesh generation with positions, normals, and indices
- minimal Lambert-style shading compiled at runtime with Shaderc
- keyboard and mouse orbit camera controls
- controls mirrored in the native window title bar while the demo is running
- in-scene HUD instructions rendered directly in the Vulkan output
- automatic toroid rotation with runtime toggle
- optional wireframe rendering when supported by the selected Vulkan device
- improved minimize/resize handling around framebuffer availability and swapchain recreation
- CPU snapshot export to timestamped PPM images
- enhanced lighting and model tilt so the toroid reads more clearly as a 3D shape
- right-side vertical slider placeholder for future shader tuning
- render loop capped to roughly 60 FPS

## Controls

- `Left` / `Right`: orbit horizontally
- `Up` / `Down`: orbit vertically
- `W` or `=`: zoom in
- `S` or `-`: zoom out
- hold `Left Mouse Button` and drag: orbit the camera
- hold `Right Mouse Button` and drag vertically: zoom the camera
- scroll `Mouse Wheel`: zoom the camera
- drag the right-side vertical slider: adjust the placeholder shader control value
- `R`: toggle automatic toroid rotation
- `F1`: toggle wireframe mode when supported by the current GPU
- `P`: export a snapshot to `Screenshots\`
- `Esc`: close the demo

## Run

From the `C#` solution root:

```powershell
dotnet run --project .\TheOpenMathLibrary.GraphicsDemo\TheOpenMathLibrary.GraphicsDemo.csproj
```

## Test

The non-GPU logic is covered by `TheOpenMathLibrary.GraphicsDemo.Tests`:

```powershell
dotnet test .\TheOpenMathLibrary.GraphicsDemo.Tests\TheOpenMathLibrary.GraphicsDemo.Tests.csproj
```

## Requirements

- .NET 8 SDK
- Vulkan-capable graphics driver/runtime
- a machine where GLFW window creation is available

## Notes

- snapshot export currently uses a CPU-generated `.ppm` image path so it remains portable and independent of Vulkan readback support
- wireframe mode is enabled only when the selected Vulkan device advertises non-solid fill support
- minimizing the window will pause rendering until the framebuffer becomes usable again
- the current controls are also shown in the GLFW title bar for quick in-window reference
- the right-side slider is currently visual/input-ready and reserved for future shader parameter control

A successful build does not guarantee runtime rendering on systems that do not expose Vulkan presentation support.

