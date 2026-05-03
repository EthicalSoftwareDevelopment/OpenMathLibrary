use std::f32::consts::PI;
use std::fmt;
use std::sync::Arc;

use bytemuck::{Pod, Zeroable};
use vulkano::buffer::{Buffer, BufferCreateInfo, BufferUsage};
use vulkano::command_buffer::allocator::StandardCommandBufferAllocator;
use vulkano::command_buffer::{
    AutoCommandBufferBuilder, CommandBufferUsage, RenderPassBeginInfo, SubpassContents,
};
use vulkano::descriptor_set::allocator::StandardDescriptorSetAllocator;
use vulkano::descriptor_set::layout::{
    DescriptorSetLayout, DescriptorSetLayoutBinding, DescriptorSetLayoutCreateInfo, DescriptorType,
};
use vulkano::descriptor_set::{PersistentDescriptorSet, WriteDescriptorSet};
use vulkano::device::physical::PhysicalDeviceType;
use vulkano::device::{Device, DeviceCreateInfo, DeviceExtensions, Queue, QueueCreateInfo};
use vulkano::format::Format;
use vulkano::image::{view::ImageView, AttachmentImage, ImageAccess, ImageUsage, SwapchainImage};
use vulkano::instance::{Instance, InstanceCreateInfo};
use vulkano::memory::allocator::{AllocationCreateInfo, MemoryUsage, StandardMemoryAllocator};
use vulkano::pipeline::graphics::depth_stencil::DepthStencilState;
use vulkano::pipeline::graphics::input_assembly::InputAssemblyState;
use vulkano::pipeline::graphics::rasterization::{CullMode, FrontFace, RasterizationState};
use vulkano::pipeline::graphics::vertex_input::Vertex;
use vulkano::pipeline::graphics::viewport::{Viewport, ViewportState};
use vulkano::pipeline::layout::{PipelineLayout, PipelineLayoutCreateInfo};
use vulkano::pipeline::{GraphicsPipeline, Pipeline, PipelineBindPoint};
use vulkano::render_pass::{Framebuffer, FramebufferCreateInfo, RenderPass, Subpass};
use vulkano::shader::ShaderModule;
use vulkano::shader::ShaderStages;
use vulkano::swapchain::{
    acquire_next_image, AcquireError, PresentMode, Surface, Swapchain, SwapchainCreateInfo,
    SwapchainCreationError, SwapchainPresentInfo,
};
use vulkano::sync::{self, FlushError, GpuFuture};
use vulkano::VulkanLibrary;
use vulkano_win::VkSurfaceBuild;
use winit::event::{ElementState, Event, MouseButton, WindowEvent};
use winit::event_loop::{ControlFlow, EventLoop};
use winit::window::{Window, WindowBuilder};

#[derive(Default, Debug, Clone, Copy, Zeroable, Pod, Vertex)]
#[repr(C)]
struct SceneVertex {
    #[format(R32G32B32_SFLOAT)]
    position: [f32; 3],
    #[format(R32G32B32_SFLOAT)]
    normal: [f32; 3],
    #[format(R32G32B32_SFLOAT)]
    color: [f32; 3],
}

#[derive(Default, Debug, Clone, Copy, Zeroable, Pod, Vertex)]
#[repr(C)]
struct UiVertex {
    #[format(R32G32_SFLOAT)]
    position: [f32; 2],
    #[format(R32G32B32_SFLOAT)]
    color: [f32; 3],
}

type Matrix4 = [[f32; 4]; 4];
type DemoResult<T> = Result<T, Box<dyn std::error::Error>>;
type SwapchainBundle = (Arc<Swapchain>, Vec<Arc<SwapchainImage>>);
type FramebufferBundle = (Vec<Arc<Framebuffer>>, Viewport);

const SCENE_VERTEX_SHADER_WGSL: &str = r#"
struct SceneUniforms {
    mvp: mat4x4<f32>,
    shading: vec4<f32>,
};

struct VertexInput {
    @location(0) position: vec3<f32>,
    @location(1) normal: vec3<f32>,
    @location(2) color: vec3<f32>,
};

struct VertexOutput {
    @builtin(position) position: vec4<f32>,
    @location(0) vertex_color: vec3<f32>,
};

@group(0) @binding(0)
var<uniform> uniforms: SceneUniforms;

@vertex
fn main(input: VertexInput) -> VertexOutput {
    var output: VertexOutput;
    let light_direction = normalize(vec3<f32>(0.4, 1.0, 0.7));
    let flow_strength = uniforms.shading.x;
    let ambient_floor = uniforms.shading.y;
    let contrast = uniforms.shading.z;
    let phase = uniforms.shading.w;
    let flow_wave = 0.5 + 0.5 * sin(dot(input.position, vec3<f32>(2.1, 3.7, 1.9)) + phase * 6.2831853);
    let dynamic_light = normalize(vec3<f32>(0.35 + 0.45 * contrast, 1.0, 0.75 - 0.3 * contrast));
    let diffuse = max(dot(normalize(input.normal), dynamic_light), 0.0);
    let lit = ambient_floor + (1.0 - ambient_floor) * diffuse;
    let flowing_lit = lit * (0.82 + 0.18 * flow_wave);
    let final_lit = lit + flow_strength * (flowing_lit - lit);

    output.position = uniforms.mvp * vec4<f32>(input.position, 1.0);
    output.vertex_color = input.color * final_lit;
    return output;
}
"#;

const SCENE_FRAGMENT_SHADER_WGSL: &str = r#"
@fragment
fn main(@location(0) vertex_color: vec3<f32>) -> @location(0) vec4<f32> {
    return vec4<f32>(vertex_color, 1.0);
}
"#;

const UI_VERTEX_SHADER_WGSL: &str = r#"
struct UiVertexInput {
    @location(0) position: vec2<f32>,
    @location(1) color: vec3<f32>,
};

struct UiVertexOutput {
    @builtin(position) position: vec4<f32>,
    @location(0) vertex_color: vec3<f32>,
};

@vertex
fn main(input: UiVertexInput) -> UiVertexOutput {
    var output: UiVertexOutput;
    output.position = vec4<f32>(input.position, 0.0, 1.0);
    output.vertex_color = input.color;
    return output;
}
"#;

const UI_FRAGMENT_SHADER_WGSL: &str = r#"
@fragment
fn main(@location(0) vertex_color: vec3<f32>) -> @location(0) vec4<f32> {
    return vec4<f32>(vertex_color, 1.0);
}
"#;

const RIGHT_PANEL_WIDTH_PX: f32 = 220.0;
const MIN_SCENE_WIDTH_PX: f32 = 320.0;
const PANEL_MARGIN_PX: f32 = 28.0;
const SLIDER_TRACK_WIDTH_PX: f32 = 10.0;
const SLIDER_KNOB_HALF_WIDTH_PX: f32 = 22.0;
const SLIDER_KNOB_HALF_HEIGHT_PX: f32 = 14.0;

#[derive(Debug)]
struct ToroidMesh {
    vertices: Vec<SceneVertex>,
    indices: Vec<u32>,
}

#[derive(Default, Debug, Clone, Copy, Zeroable, Pod)]
#[repr(C)]
struct SceneUniformData {
    mvp: Matrix4,
    shading: [f32; 4],
}

#[derive(Debug, Clone, Copy, PartialEq)]
struct SliderValue(f32);

impl SliderValue {
    const MIDPOINT: Self = Self(0.5);

    fn new_clamped(value: f32) -> Self {
        Self(value.clamp(0.0, 1.0))
    }

    fn get(self) -> f32 {
        self.0
    }
}

#[derive(Debug, Clone, Copy)]
struct PositiveFiniteF32(f32);

impl PositiveFiniteF32 {
    fn new(value: f32) -> Result<Self, DemoConfigError> {
        if !value.is_finite() || value <= 0.0 {
            return Err(DemoConfigError::NonPositiveFiniteValue);
        }

        Ok(Self(value))
    }

    const fn get(self) -> f32 {
        self.0
    }
}

#[derive(Debug, Clone, Copy)]
struct ToroidSpec<const MAJOR_SEGMENTS: u32, const MINOR_SEGMENTS: u32> {
    major_radius: PositiveFiniteF32,
    minor_radius: PositiveFiniteF32,
}

impl<const MAJOR_SEGMENTS: u32, const MINOR_SEGMENTS: u32>
    ToroidSpec<MAJOR_SEGMENTS, MINOR_SEGMENTS>
{
    const ASSERT_MAJOR_SEGMENTS: () = assert!(MAJOR_SEGMENTS >= 3);
    const ASSERT_MINOR_SEGMENTS: () = assert!(MINOR_SEGMENTS >= 3);

    fn new(major_radius: f32, minor_radius: f32) -> Result<Self, DemoConfigError> {
        let _segment_constraints = (Self::ASSERT_MAJOR_SEGMENTS, Self::ASSERT_MINOR_SEGMENTS);

        let major_radius = PositiveFiniteF32::new(major_radius)?;
        let minor_radius = PositiveFiniteF32::new(minor_radius)?;

        if major_radius.get() <= minor_radius.get() {
            return Err(DemoConfigError::MajorRadiusMustExceedMinorRadius);
        }

        Ok(Self {
            major_radius,
            minor_radius,
        })
    }

    const fn major_radius(self) -> f32 {
        self.major_radius.get()
    }

    const fn minor_radius(self) -> f32 {
        self.minor_radius.get()
    }
}

#[derive(Debug, Clone, Copy, PartialEq, Eq)]
enum DemoConfigError {
    NonPositiveFiniteValue,
    MajorRadiusMustExceedMinorRadius,
}

impl fmt::Display for DemoConfigError {
    fn fmt(&self, formatter: &mut fmt::Formatter<'_>) -> fmt::Result {
        let message = match self {
            Self::NonPositiveFiniteValue => "toroid radii must be finite and strictly positive",
            Self::MajorRadiusMustExceedMinorRadius => {
                "major radius must exceed minor radius for this demo toroid"
            }
        };

        formatter.write_str(message)
    }
}

impl std::error::Error for DemoConfigError {}

#[derive(Debug, Clone, Copy)]
struct Rect {
    left: f32,
    top: f32,
    right: f32,
    bottom: f32,
}

impl Rect {
    fn width(self) -> f32 {
        (self.right - self.left).max(0.0)
    }

    fn height(self) -> f32 {
        (self.bottom - self.top).max(0.0)
    }

    fn contains(self, x: f32, y: f32) -> bool {
        x >= self.left && x <= self.right && y >= self.top && y <= self.bottom
    }
}

#[derive(Debug, Clone)]
struct DemoLayout {
    scene_viewport: Viewport,
    ui_viewport: Viewport,
    panel_rect: Rect,
    divider_rect: Rect,
    slider_track_rect: Rect,
    slider_hit_rect: Rect,
    slider_knob_rect: Rect,
}

#[derive(Debug, Clone, Copy)]
struct CameraConfig {
    distance: f32,
    x_rotation: f32,
    y_rotation: f32,
    vertical_fov_radians: f32,
    near_plane: f32,
    far_plane: f32,
}

impl Default for CameraConfig {
    fn default() -> Self {
        Self {
            distance: 3.8,
            x_rotation: 0.6,
            y_rotation: 0.9,
            vertical_fov_radians: PI / 3.0,
            near_plane: 0.1,
            far_plane: 100.0,
        }
    }
}

fn main() {
    if let Err(error) = run() {
        eprintln!("Failed to launch toroid Vulkan demo: {error}");
    }
}

fn run() -> DemoResult<()> {
    let library = VulkanLibrary::new()?;
    let required_extensions = vulkano_win::required_extensions(&library);
    let instance = Instance::new(
        library,
        InstanceCreateInfo {
            enabled_extensions: required_extensions,
            ..Default::default()
        },
    )?;

    let event_loop = EventLoop::new();
    let surface = WindowBuilder::new()
        .with_title("OpenMathLibrary - Toroid Vulkan Demo")
        .build_vk_surface(&event_loop, instance.clone())?;

    let (device, queue) = create_device(surface.clone())?;
    let (mut swapchain, images) = create_swapchain(device.clone(), surface.clone(), queue.clone())?;
    let memory_allocator = Arc::new(StandardMemoryAllocator::new_default(device.clone()));

    let depth_format = Format::D16_UNORM;
    let render_pass = create_render_pass(device.clone(), swapchain.image_format(), depth_format)?;
    let (mut framebuffers, mut viewport) =
        window_size_dependent_setup(memory_allocator.as_ref(), &images, render_pass.clone(), depth_format)?;
    let (scene_descriptor_set_layout, scene_pipeline_layout) =
        create_scene_pipeline_layout(device.clone())?;

    let descriptor_set_allocator = StandardDescriptorSetAllocator::new(device.clone());
    let command_buffer_allocator =
        StandardCommandBufferAllocator::new(device.clone(), Default::default());

    let mesh = generate_toroid_mesh(ToroidSpec::<96, 48>::new(1.4, 0.45)?);
    let vertex_buffer = Buffer::from_iter(
        memory_allocator.as_ref(),
        BufferCreateInfo {
            usage: BufferUsage::VERTEX_BUFFER,
            ..Default::default()
        },
        AllocationCreateInfo {
            usage: MemoryUsage::Upload,
            ..Default::default()
        },
        mesh.vertices.iter().copied(),
    )?;
    let index_buffer = Buffer::from_iter(
        memory_allocator.as_ref(),
        BufferCreateInfo {
            usage: BufferUsage::INDEX_BUFFER,
            ..Default::default()
        },
        AllocationCreateInfo {
            usage: MemoryUsage::Upload,
            ..Default::default()
        },
        mesh.indices.iter().copied(),
    )?;

    let vs = load_wgsl_shader(device.clone(), SCENE_VERTEX_SHADER_WGSL, naga::ShaderStage::Vertex)?;
    let fs =
        load_wgsl_shader(device.clone(), SCENE_FRAGMENT_SHADER_WGSL, naga::ShaderStage::Fragment)?;
    let pipeline = create_pipeline(
        device.clone(),
        render_pass.clone(),
        scene_pipeline_layout.clone(),
        vs,
        fs,
    )?;
    let ui_vs = load_wgsl_shader(device.clone(), UI_VERTEX_SHADER_WGSL, naga::ShaderStage::Vertex)?;
    let ui_fs =
        load_wgsl_shader(device.clone(), UI_FRAGMENT_SHADER_WGSL, naga::ShaderStage::Fragment)?;
    let ui_pipeline = create_ui_pipeline(device.clone(), render_pass.clone(), ui_vs, ui_fs)?;
    let camera = CameraConfig::default();

    let mut recreate_swapchain = false;
    let mut previous_frame_end = Some(sync::now(device.clone()).boxed());
    let mut shader_control_value = SliderValue::MIDPOINT;
    let mut slider_drag_active = false;
    let mut cursor_position = [0.0_f32, 0.0_f32];
    let mut needs_redraw = true;

    request_window_redraw(&surface);

    event_loop.run(move |event, _, control_flow| {
        *control_flow = ControlFlow::Wait;

        match event {
            Event::WindowEvent {
                event: WindowEvent::CloseRequested,
                ..
            } => {
                *control_flow = ControlFlow::Exit;
            }
            Event::WindowEvent {
                event: WindowEvent::Resized(_),
                ..
            } => {
                recreate_swapchain = true;
                needs_redraw = true;
            }
            Event::WindowEvent {
                event: WindowEvent::ScaleFactorChanged { .. },
                ..
            } => {
                recreate_swapchain = true;
                needs_redraw = true;
            }
            Event::WindowEvent {
                event: WindowEvent::CursorMoved { position, .. },
                ..
            } => {
                cursor_position = [position.x as f32, position.y as f32];

                if slider_drag_active {
                    if let Some(window_extent) = current_window_extent(&surface) {
                        let full_viewport = viewport_from_extent(window_extent);
                        let layout = build_demo_layout(&full_viewport, shader_control_value);
                        shader_control_value =
                            slider_value_from_cursor(&layout.slider_track_rect, cursor_position[1]);
                        needs_redraw = true;
                    }
                }
            }
            Event::WindowEvent {
                event:
                    WindowEvent::MouseInput {
                        state,
                        button: MouseButton::Left,
                        ..
                    },
                ..
            } => {
                if let Some(window_extent) = current_window_extent(&surface) {
                    let full_viewport = viewport_from_extent(window_extent);
                    let layout = build_demo_layout(&full_viewport, shader_control_value);

                    match state {
                        ElementState::Pressed => {
                            if layout
                                .slider_hit_rect
                                .contains(cursor_position[0], cursor_position[1])
                            {
                                slider_drag_active = true;
                                shader_control_value = slider_value_from_cursor(
                                    &layout.slider_track_rect,
                                    cursor_position[1],
                                );
                                needs_redraw = true;
                            }
                        }
                        ElementState::Released => {
                            slider_drag_active = false;
                            needs_redraw = true;
                        }
                    }
                }
            }
            Event::MainEventsCleared => {
                if needs_redraw {
                    request_window_redraw(&surface);
                }
            }
            Event::RedrawRequested(_) => {
                let Some(future) = previous_frame_end.as_mut() else {
                    return;
                };
                future.cleanup_finished();

                if recreate_swapchain {
                    let Some(image_extent) = current_window_extent(&surface) else {
                        needs_redraw = true;
                        return;
                    };

                    match swapchain.recreate(SwapchainCreateInfo {
                        image_extent,
                        ..swapchain.create_info()
                    }) {
                        Ok((new_swapchain, new_images)) => {
                            swapchain = new_swapchain;
                            match window_size_dependent_setup(
                                memory_allocator.as_ref(),
                                &new_images,
                                render_pass.clone(),
                                depth_format,
                            ) {
                                Ok((new_framebuffers, new_viewport)) => {
                                    framebuffers = new_framebuffers;
                                    viewport = new_viewport;
                                }
                                Err(error) => {
                                    eprintln!(
                                        "Failed to rebuild swapchain-dependent resources: {error}"
                                    );
                                    *control_flow = ControlFlow::Exit;
                                    return;
                                }
                            }
                            recreate_swapchain = false;
                            needs_redraw = true;
                        }
                        Err(SwapchainCreationError::ImageExtentNotSupported { .. }) => return,
                        Err(error) => {
                            eprintln!("Failed to recreate swapchain: {error}");
                            *control_flow = ControlFlow::Exit;
                            return;
                        }
                    }
                }

                let (image_index, suboptimal, acquire_future) =
                    match acquire_next_image(swapchain.clone(), None) {
                        Ok(result) => result,
                        Err(AcquireError::OutOfDate) => {
                            recreate_swapchain = true;
                            needs_redraw = true;
                            return;
                        }
                        Err(AcquireError::Timeout) => {
                            needs_redraw = true;
                            return;
                        }
                        Err(error) => {
                            eprintln!("Failed to acquire swapchain image: {error}");
                            needs_redraw = true;
                            return;
                        }
                    };

                if suboptimal {
                    recreate_swapchain = true;
                }

                let layout = build_demo_layout(&viewport, shader_control_value);
                let mvp = static_demo_mvp(&camera, &layout.scene_viewport);
                let shading = shader_flow_parameters(shader_control_value);
                let uniform_subbuffer = match Buffer::from_data(
                    memory_allocator.as_ref(),
                    BufferCreateInfo {
                        usage: BufferUsage::UNIFORM_BUFFER,
                        ..Default::default()
                    },
                    AllocationCreateInfo {
                        usage: MemoryUsage::Upload,
                        ..Default::default()
                    },
                    SceneUniformData { mvp, shading },
                ) {
                    Ok(buffer) => buffer,
                    Err(error) => {
                        eprintln!("Failed to allocate uniform buffer: {error}");
                        *control_flow = ControlFlow::Exit;
                        return;
                    }
                };

                let ui_vertices = build_ui_vertices(&layout, shader_control_value);
                let ui_vertex_buffer = match Buffer::from_iter(
                    memory_allocator.as_ref(),
                    BufferCreateInfo {
                        usage: BufferUsage::VERTEX_BUFFER,
                        ..Default::default()
                    },
                    AllocationCreateInfo {
                        usage: MemoryUsage::Upload,
                        ..Default::default()
                    },
                    ui_vertices,
                ) {
                    Ok(buffer) => buffer,
                    Err(error) => {
                        eprintln!("Failed to allocate UI vertex buffer: {error}");
                        *control_flow = ControlFlow::Exit;
                        return;
                    }
                };

                let descriptor_set = match PersistentDescriptorSet::new(
                    &descriptor_set_allocator,
                    scene_descriptor_set_layout.clone(),
                    [WriteDescriptorSet::buffer(0, uniform_subbuffer)],
                ) {
                    Ok(set) => set,
                    Err(error) => {
                        eprintln!("Failed to create descriptor set: {error}");
                        *control_flow = ControlFlow::Exit;
                        return;
                    }
                };

                let mut builder = match AutoCommandBufferBuilder::primary(
                    &command_buffer_allocator,
                    queue.queue_family_index(),
                    CommandBufferUsage::OneTimeSubmit,
                ) {
                    Ok(builder) => builder,
                    Err(error) => {
                        eprintln!("Failed to start command buffer: {error}");
                        *control_flow = ControlFlow::Exit;
                        return;
                    }
                };

                if let Err(error) = builder.begin_render_pass(
                    RenderPassBeginInfo {
                        clear_values: vec![
                            Some([0.02, 0.02, 0.05, 1.0].into()),
                            Some(1_f32.into()),
                        ],
                        ..RenderPassBeginInfo::framebuffer(framebuffers[image_index as usize].clone())
                    },
                    SubpassContents::Inline,
                ) {
                    eprintln!("Failed to record command buffer: {error}");
                    *control_flow = ControlFlow::Exit;
                    return;
                }

                builder.set_viewport(0, [layout.scene_viewport.clone()]);
                builder.bind_pipeline_graphics(pipeline.clone());
                builder.bind_descriptor_sets(
                    PipelineBindPoint::Graphics,
                    pipeline.layout().clone(),
                    0,
                    descriptor_set,
                );
                builder.bind_vertex_buffers(0, vertex_buffer.clone());
                builder.bind_index_buffer(index_buffer.clone());

                if let Err(error) =
                    builder.draw_indexed(index_buffer.len() as u32, 1, 0, 0, 0)
                {
                    eprintln!("Failed to draw toroid mesh: {error}");
                    *control_flow = ControlFlow::Exit;
                    return;
                }

                builder.set_viewport(0, [layout.ui_viewport.clone()]);
                builder.bind_pipeline_graphics(ui_pipeline.clone());
                builder.bind_vertex_buffers(0, ui_vertex_buffer.clone());

                if let Err(error) = builder.draw(ui_vertex_buffer.len() as u32, 1, 0, 0) {
                    eprintln!("Failed to draw UI overlay: {error}");
                    *control_flow = ControlFlow::Exit;
                    return;
                }

                if let Err(error) = builder.end_render_pass() {
                    eprintln!("Failed to end render pass: {error}");
                    *control_flow = ControlFlow::Exit;
                    return;
                }

                let command_buffer = match builder.build() {
                    Ok(command_buffer) => command_buffer,
                    Err(error) => {
                        eprintln!("Failed to build command buffer: {error}");
                        *control_flow = ControlFlow::Exit;
                        return;
                    }
                };

                let future = previous_frame_end
                    .take()
                    .expect("previous frame future should exist")
                    .join(acquire_future)
                    .then_execute(queue.clone(), command_buffer);

                let future = match future {
                    Ok(future) => future
                        .then_swapchain_present(
                            queue.clone(),
                            SwapchainPresentInfo::swapchain_image_index(
                                swapchain.clone(),
                                image_index,
                            ),
                        )
                        .then_signal_fence_and_flush(),
                    Err(error) => {
                        eprintln!("Failed to submit draw commands: {error}");
                        previous_frame_end = Some(sync::now(device.clone()).boxed());
                        needs_redraw = true;
                        return;
                    }
                };

                match future {
                    Ok(future) => {
                        previous_frame_end = Some(future.boxed());
                        needs_redraw = false;
                    }
                    Err(FlushError::OutOfDate) => {
                        recreate_swapchain = true;
                        previous_frame_end = Some(sync::now(device.clone()).boxed());
                        needs_redraw = true;
                    }
                    Err(FlushError::Timeout)
                    | Err(FlushError::AccessError(_))
                    | Err(FlushError::ResourceAccessError { .. })
                    | Err(FlushError::ExclusiveAlreadyInUse)
                    | Err(FlushError::OneTimeSubmitAlreadySubmitted) => {
                        previous_frame_end = Some(sync::now(device.clone()).boxed());
                        needs_redraw = true;
                    }
                    Err(error) => {
                        eprintln!("Failed to flush GPU future: {error}");
                        previous_frame_end = Some(sync::now(device.clone()).boxed());
                        needs_redraw = true;
                    }
                }
            }
            _ => {}
        }
    });
}

fn create_device(
    surface: Arc<Surface>,
) -> Result<(Arc<Device>, Arc<Queue>), Box<dyn std::error::Error>> {
    let device_extensions = DeviceExtensions {
        khr_swapchain: true,
        ..DeviceExtensions::empty()
    };

    let (physical_device, queue_family_index) = surface
        .instance()
        .enumerate_physical_devices()?
        .filter(|device| device.supported_extensions().contains(&device_extensions))
        .filter_map(|device| {
            device
                .queue_family_properties()
                .iter()
                .enumerate()
                .find(|(queue_family_index, queue_family_properties)| {
                    queue_family_properties
                        .queue_flags
                        .intersects(vulkano::device::QueueFlags::GRAPHICS)
                        && device
                            .surface_support(*queue_family_index as u32, &surface)
                            .unwrap_or(false)
                })
                .map(|(queue_family_index, _)| (device.clone(), queue_family_index as u32))
        })
        .min_by_key(|(device, _)| match device.properties().device_type {
            PhysicalDeviceType::DiscreteGpu => 0,
            PhysicalDeviceType::IntegratedGpu => 1,
            PhysicalDeviceType::VirtualGpu => 2,
            PhysicalDeviceType::Cpu => 3,
            _ => 4,
        })
        .ok_or("No suitable Vulkan device found.")?;

    let (device, mut queues) = Device::new(
        physical_device,
        DeviceCreateInfo {
            enabled_extensions: device_extensions,
            queue_create_infos: vec![QueueCreateInfo {
                queue_family_index,
                ..Default::default()
            }],
            ..Default::default()
        },
    )?;

    let queue = queues
        .next()
        .ok_or("Logical device did not expose a graphics queue.")?;

    Ok((device, queue))
}

fn create_swapchain(
    device: Arc<Device>,
    surface: Arc<Surface>,
    queue: Arc<Queue>,
) -> DemoResult<SwapchainBundle> {
    let physical_device = device.physical_device();
    let surface_capabilities =
        physical_device.surface_capabilities(&surface, Default::default())?;
    let image_format = Some(
        physical_device
            .surface_formats(&surface, Default::default())?
            .first()
            .ok_or("Surface does not expose any supported image formats.")?
            .0,
    );

    let window = surface
        .object()
        .and_then(|object| object.downcast_ref::<Window>())
        .ok_or("Surface should expose a winit window.")?;
    let image_extent = window.inner_size();

    let min_image_count = (surface_capabilities.min_image_count + 1)
        .min(surface_capabilities.max_image_count.unwrap_or(u32::MAX));

    let (swapchain, images) = Swapchain::new(
        device,
        surface,
        SwapchainCreateInfo {
            min_image_count,
            image_format,
            image_extent: image_extent.into(),
            image_usage: ImageUsage::COLOR_ATTACHMENT,
            composite_alpha: surface_capabilities
                .supported_composite_alpha
                .into_iter()
                .next()
                .ok_or("Surface does not support any composite alpha modes.")?,
            present_mode: PresentMode::Fifo,
            ..Default::default()
        },
    )?;

    let _ = queue;
    Ok((swapchain, images))
}

fn create_render_pass(
    device: Arc<Device>,
    image_format: Format,
    depth_format: Format,
) -> DemoResult<Arc<RenderPass>> {
    Ok(vulkano::single_pass_renderpass!(
        device,
        attachments: {
            color: {
                load: Clear,
                store: Store,
                format: image_format,
                samples: 1,
            },
            depth: {
                load: Clear,
                store: DontCare,
                format: depth_format,
                samples: 1,
            }
        },
        pass: {
            color: [color],
            depth_stencil: {depth}
        }
    )?)
}

fn create_scene_pipeline_layout(
    device: Arc<Device>,
) -> DemoResult<(Arc<DescriptorSetLayout>, Arc<PipelineLayout>)> {
    let descriptor_set_layout = DescriptorSetLayout::new(
        device.clone(),
        DescriptorSetLayoutCreateInfo {
            bindings: [(
                0,
                DescriptorSetLayoutBinding {
                    stages: ShaderStages::all_graphics(),
                    ..DescriptorSetLayoutBinding::descriptor_type(DescriptorType::UniformBuffer)
                },
            )]
            .into(),
            ..Default::default()
        },
    )?;
    let pipeline_layout = PipelineLayout::new(
        device,
        PipelineLayoutCreateInfo {
            set_layouts: vec![descriptor_set_layout.clone()],
            ..Default::default()
        },
    )?;

    Ok((descriptor_set_layout, pipeline_layout))
}

fn create_pipeline(
    device: Arc<Device>,
    render_pass: Arc<RenderPass>,
    pipeline_layout: Arc<PipelineLayout>,
    vs: Arc<vulkano::shader::ShaderModule>,
    fs: Arc<vulkano::shader::ShaderModule>,
) -> DemoResult<Arc<GraphicsPipeline>> {
    let subpass = Subpass::from(render_pass, 0)
        .ok_or("Render pass does not contain graphics subpass 0.")?;
    let vs_entry_point = vs
        .entry_point("main")
        .ok_or("Scene vertex shader entry point 'main' was not found.")?;
    let fs_entry_point = fs
        .entry_point("main")
        .ok_or("Scene fragment shader entry point 'main' was not found.")?;

    Ok(GraphicsPipeline::start()
        .vertex_input_state(SceneVertex::per_vertex())
        .vertex_shader(vs_entry_point, ())
        .input_assembly_state(InputAssemblyState::new())
        .viewport_state(ViewportState::viewport_dynamic_scissor_irrelevant())
        .depth_stencil_state(DepthStencilState::simple_depth_test())
        .rasterization_state(
            RasterizationState::new()
                .cull_mode(CullMode::Back)
                .front_face(FrontFace::CounterClockwise),
        )
        .fragment_shader(fs_entry_point, ())
        .render_pass(subpass)
        .with_pipeline_layout(device, pipeline_layout)?)
}

fn create_ui_pipeline(
    device: Arc<Device>,
    render_pass: Arc<RenderPass>,
    vs: Arc<vulkano::shader::ShaderModule>,
    fs: Arc<vulkano::shader::ShaderModule>,
) -> DemoResult<Arc<GraphicsPipeline>> {
    let subpass = Subpass::from(render_pass, 0)
        .ok_or("Render pass does not contain UI subpass 0.")?;
    let vs_entry_point = vs
        .entry_point("main")
        .ok_or("UI vertex shader entry point 'main' was not found.")?;
    let fs_entry_point = fs
        .entry_point("main")
        .ok_or("UI fragment shader entry point 'main' was not found.")?;

    Ok(GraphicsPipeline::start()
        .vertex_input_state(UiVertex::per_vertex())
        .vertex_shader(vs_entry_point, ())
        .input_assembly_state(InputAssemblyState::new())
        .viewport_state(ViewportState::viewport_dynamic_scissor_irrelevant())
        .rasterization_state(RasterizationState::new().cull_mode(CullMode::None))
        .fragment_shader(fs_entry_point, ())
        .render_pass(subpass)
        .build(device)?)
}

fn window_size_dependent_setup(
    memory_allocator: &StandardMemoryAllocator,
    images: &[Arc<SwapchainImage>],
    render_pass: Arc<RenderPass>,
    depth_format: Format,
) -> DemoResult<FramebufferBundle> {
    let dimensions = images[0].dimensions().width_height();
    let viewport = Viewport {
        origin: [0.0, 0.0],
        dimensions: [dimensions[0] as f32, dimensions[1] as f32],
        depth_range: 0.0..1.0,
    };

    let framebuffers = images
        .iter()
        .map(|image| {
            let color_view = ImageView::new_default(image.clone())?;
            let depth_buffer =
                AttachmentImage::transient(memory_allocator, dimensions, depth_format)?;
            let depth_view = ImageView::new_default(depth_buffer)?;

            Ok(Framebuffer::new(
                render_pass.clone(),
                FramebufferCreateInfo {
                    attachments: vec![color_view, depth_view],
                    ..Default::default()
                },
            )?)
        })
        .collect::<Result<Vec<_>, Box<dyn std::error::Error>>>()?;

    Ok((framebuffers, viewport))
}

fn current_window_extent(surface: &Arc<Surface>) -> Option<[u32; 2]> {
    let window = surface
        .object()
        .and_then(|object| object.downcast_ref::<Window>())?;
    let dimensions = window.inner_size();

    if dimensions.width == 0 || dimensions.height == 0 {
        None
    } else {
        Some([dimensions.width, dimensions.height])
    }
}

fn request_window_redraw(surface: &Arc<Surface>) {
    if let Some(window) = surface
        .object()
        .and_then(|object| object.downcast_ref::<Window>())
    {
        window.request_redraw();
    }
}

fn viewport_from_extent(extent: [u32; 2]) -> Viewport {
    Viewport {
        origin: [0.0, 0.0],
        dimensions: [extent[0] as f32, extent[1] as f32],
        depth_range: 0.0..1.0,
    }
}

fn build_demo_layout(full_viewport: &Viewport, slider_value: SliderValue) -> DemoLayout {
    let full_width = full_viewport.dimensions[0].max(1.0);
    let full_height = full_viewport.dimensions[1].max(1.0);
    let max_panel_width = (full_width - MIN_SCENE_WIDTH_PX).max(0.0);
    let panel_width = if max_panel_width <= 0.0 {
        0.0
    } else {
        RIGHT_PANEL_WIDTH_PX.min(max_panel_width)
    };
    let scene_width = (full_width - panel_width).max(1.0);
    let scene_viewport = Viewport {
        origin: [0.0, 0.0],
        dimensions: [scene_width, full_height],
        depth_range: 0.0..1.0,
    };

    let panel_rect = Rect {
        left: scene_width,
        top: 0.0,
        right: full_width,
        bottom: full_height,
    };
    let divider_rect = Rect {
        left: scene_width,
        top: 0.0,
        right: (scene_width + 2.0).min(full_width),
        bottom: full_height,
    };

    let slider_top = PANEL_MARGIN_PX.min(full_height * 0.25);
    let slider_bottom = (full_height - PANEL_MARGIN_PX).max(slider_top + 72.0);
    let slider_center_x = if panel_width > 0.0 {
        full_width - PANEL_MARGIN_PX - SLIDER_KNOB_HALF_WIDTH_PX
    } else {
        full_width - PANEL_MARGIN_PX
    };
    let slider_track_rect = if panel_width > 0.0 {
        Rect {
            left: slider_center_x - (SLIDER_TRACK_WIDTH_PX * 0.5),
            top: slider_top,
            right: slider_center_x + (SLIDER_TRACK_WIDTH_PX * 0.5),
            bottom: slider_bottom,
        }
    } else {
        Rect {
            left: 0.0,
            top: 0.0,
            right: 0.0,
            bottom: 0.0,
        }
    };

    let slider_center_y = slider_center_y(&slider_track_rect, slider_value);
    let slider_knob_rect = if panel_width > 0.0 {
        Rect {
            left: slider_center_x - SLIDER_KNOB_HALF_WIDTH_PX,
            top: slider_center_y - SLIDER_KNOB_HALF_HEIGHT_PX,
            right: slider_center_x + SLIDER_KNOB_HALF_WIDTH_PX,
            bottom: slider_center_y + SLIDER_KNOB_HALF_HEIGHT_PX,
        }
    } else {
        Rect {
            left: 0.0,
            top: 0.0,
            right: 0.0,
            bottom: 0.0,
        }
    };
    let slider_hit_rect = Rect {
        left: slider_knob_rect.left.min(slider_track_rect.left) - 12.0,
        top: slider_track_rect.top - 12.0,
        right: slider_knob_rect.right.max(slider_track_rect.right) + 12.0,
        bottom: slider_track_rect.bottom + 12.0,
    };

    DemoLayout {
        scene_viewport,
        ui_viewport: full_viewport.clone(),
        panel_rect,
        divider_rect,
        slider_track_rect,
        slider_hit_rect,
        slider_knob_rect,
    }
}

fn slider_center_y(track_rect: &Rect, slider_value: SliderValue) -> f32 {
    track_rect.top + (1.0 - slider_value.get()) * track_rect.height()
}

fn slider_value_from_cursor(track_rect: &Rect, cursor_y: f32) -> SliderValue {
    if track_rect.height() <= 0.0 {
        return SliderValue::MIDPOINT;
    }

    let normalized = ((cursor_y - track_rect.top) / track_rect.height()).clamp(0.0, 1.0);
    SliderValue::new_clamped(1.0 - normalized)
}

fn build_ui_vertices(layout: &DemoLayout, slider_value: SliderValue) -> Vec<UiVertex> {
    let mut vertices = Vec::with_capacity(30);

    push_rect(
        &mut vertices,
        &layout.panel_rect,
        &layout.ui_viewport,
        [0.05, 0.06, 0.10],
    );
    push_rect(
        &mut vertices,
        &layout.divider_rect,
        &layout.ui_viewport,
        [0.18, 0.22, 0.32],
    );
    push_rect(
        &mut vertices,
        &layout.slider_track_rect,
        &layout.ui_viewport,
        [0.18, 0.21, 0.30],
    );

    let slider_fill_rect = Rect {
        left: layout.slider_track_rect.left,
        top: slider_center_y(&layout.slider_track_rect, slider_value),
        right: layout.slider_track_rect.right,
        bottom: layout.slider_track_rect.bottom,
    };
    push_rect(
        &mut vertices,
        &slider_fill_rect,
        &layout.ui_viewport,
        [0.22, 0.58, 0.92],
    );
    push_rect(
        &mut vertices,
        &layout.slider_knob_rect,
        &layout.ui_viewport,
        [0.88, 0.92, 0.98],
    );

    vertices
}

fn push_rect(
    vertices: &mut Vec<UiVertex>,
    rect: &Rect,
    viewport: &Viewport,
    color: [f32; 3],
) {
    if rect.width() <= 0.0 || rect.height() <= 0.0 {
        return;
    }

    let top_left = pixel_to_ndc(rect.left, rect.top, viewport);
    let top_right = pixel_to_ndc(rect.right, rect.top, viewport);
    let bottom_right = pixel_to_ndc(rect.right, rect.bottom, viewport);
    let bottom_left = pixel_to_ndc(rect.left, rect.bottom, viewport);

    vertices.extend_from_slice(&[
        UiVertex {
            position: top_left,
            color,
        },
        UiVertex {
            position: top_right,
            color,
        },
        UiVertex {
            position: bottom_right,
            color,
        },
        UiVertex {
            position: top_left,
            color,
        },
        UiVertex {
            position: bottom_right,
            color,
        },
        UiVertex {
            position: bottom_left,
            color,
        },
    ]);
}

fn pixel_to_ndc(x: f32, y: f32, viewport: &Viewport) -> [f32; 2] {
    let width = viewport.dimensions[0].max(1.0);
    let height = viewport.dimensions[1].max(1.0);

    [(x / width) * 2.0 - 1.0, 1.0 - (y / height) * 2.0]
}

fn generate_toroid_mesh<const MAJOR_SEGMENTS: u32, const MINOR_SEGMENTS: u32>(
    spec: ToroidSpec<MAJOR_SEGMENTS, MINOR_SEGMENTS>,
) -> ToroidMesh {
    let major_radius = spec.major_radius();
    let minor_radius = spec.minor_radius();
    let major_segments = MAJOR_SEGMENTS;
    let minor_segments = MINOR_SEGMENTS;
    let vertex_count = (major_segments * minor_segments) as usize;
    let index_count = (major_segments * minor_segments * 6) as usize;
    let mut vertices = Vec::with_capacity(vertex_count);
    let mut indices = Vec::with_capacity(index_count);

    for major_index in 0..major_segments {
        let major_theta = 2.0 * PI * (major_index as f32) / (major_segments as f32);
        let (major_sin, major_cos) = major_theta.sin_cos();

        for minor_index in 0..minor_segments {
            let minor_theta = 2.0 * PI * (minor_index as f32) / (minor_segments as f32);
            let (minor_sin, minor_cos) = minor_theta.sin_cos();

            let ring_radius = major_radius + minor_radius * minor_cos;
            let position = [
                ring_radius * major_cos,
                minor_radius * minor_sin,
                ring_radius * major_sin,
            ];
            let normal = [major_cos * minor_cos, minor_sin, major_sin * minor_cos];
            let color = [
                0.35 + 0.65 * (0.5 * (normal[0] + 1.0)),
                0.25 + 0.75 * (0.5 * (normal[1] + 1.0)),
                0.40 + 0.60 * (0.5 * (normal[2] + 1.0)),
            ];

            vertices.push(SceneVertex {
                position,
                normal,
                color,
            });
        }
    }

    for major_index in 0..major_segments {
        let next_major = (major_index + 1) % major_segments;

        for minor_index in 0..minor_segments {
            let next_minor = (minor_index + 1) % minor_segments;
            let current = major_index * minor_segments + minor_index;
            let current_next_minor = major_index * minor_segments + next_minor;
            let next_major_current = next_major * minor_segments + minor_index;
            let next_major_next_minor = next_major * minor_segments + next_minor;

            indices.extend_from_slice(&[
                current,
                next_major_current,
                current_next_minor,
                current_next_minor,
                next_major_current,
                next_major_next_minor,
            ]);
        }
    }

    ToroidMesh { vertices, indices }
}

fn multiply_matrices(left: Matrix4, right: Matrix4) -> Matrix4 {
    let mut result = [[0.0; 4]; 4];

    for column in 0..4 {
        for row in 0..4 {
            result[column][row] = left[0][row] * right[column][0]
                + left[1][row] * right[column][1]
                + left[2][row] * right[column][2]
                + left[3][row] * right[column][3];
        }
    }

    result
}

fn rotation_x(angle: f32) -> Matrix4 {
    let (sine, cosine) = angle.sin_cos();
    [
        [1.0, 0.0, 0.0, 0.0],
        [0.0, cosine, sine, 0.0],
        [0.0, -sine, cosine, 0.0],
        [0.0, 0.0, 0.0, 1.0],
    ]
}

fn rotation_y(angle: f32) -> Matrix4 {
    let (sine, cosine) = angle.sin_cos();
    [
        [cosine, 0.0, -sine, 0.0],
        [0.0, 1.0, 0.0, 0.0],
        [sine, 0.0, cosine, 0.0],
        [0.0, 0.0, 0.0, 1.0],
    ]
}

fn translation(x: f32, y: f32, z: f32) -> Matrix4 {
    [
        [1.0, 0.0, 0.0, 0.0],
        [0.0, 1.0, 0.0, 0.0],
        [0.0, 0.0, 1.0, 0.0],
        [x, y, z, 1.0],
    ]
}

fn perspective_projection(aspect_ratio: f32, fov_y_radians: f32, near: f32, far: f32) -> Matrix4 {
    let focal_length = 1.0 / (0.5 * fov_y_radians).tan();

    [
        [focal_length / aspect_ratio.max(1e-6), 0.0, 0.0, 0.0],
        [0.0, -focal_length, 0.0, 0.0],
        [0.0, 0.0, far / (near - far), -1.0],
        [0.0, 0.0, (near * far) / (near - far), 0.0],
    ]
}

fn static_demo_mvp(camera: &CameraConfig, viewport: &Viewport) -> Matrix4 {
    let aspect_ratio = viewport.dimensions[0] / viewport.dimensions[1].max(1.0);
    let model = multiply_matrices(rotation_y(camera.y_rotation), rotation_x(camera.x_rotation));
    let view = translation(0.0, 0.0, -camera.distance);
    let projection = perspective_projection(
        aspect_ratio,
        camera.vertical_fov_radians,
        camera.near_plane,
        camera.far_plane,
    );

    multiply_matrices(projection, multiply_matrices(view, model))
}

fn shader_flow_parameters(slider_value: SliderValue) -> [f32; 4] {
    let control = slider_value.get().clamp(0.0, 1.0);
    let flow_strength = control;
    let ambient_floor = 0.18 + 0.12 * (1.0 - control);
    let contrast = 0.55 + 0.45 * control;
    let phase = control;

    [flow_strength, ambient_floor, contrast, phase]
}

fn load_wgsl_shader(
    device: Arc<Device>,
    source: &str,
    stage: naga::ShaderStage,
) -> Result<Arc<ShaderModule>, Box<dyn std::error::Error>> {
    let module = naga::front::wgsl::parse_str(source)?;
    let module_info = naga::valid::Validator::new(
        naga::valid::ValidationFlags::all(),
        naga::valid::Capabilities::all(),
    )
    .validate(&module)?;
    let words = naga::back::spv::write_vec(
        &module,
        &module_info,
        &naga::back::spv::Options::default(),
        Some(&naga::back::spv::PipelineOptions {
            shader_stage: stage,
            entry_point: "main".into(),
        }),
    )?;

    Ok(unsafe { ShaderModule::from_words(device, &words)? })
}

#[cfg(test)]
mod tests {
    use super::{
        build_demo_layout, build_ui_vertices, generate_toroid_mesh, multiply_matrices,
        rotation_x, shader_flow_parameters, slider_value_from_cursor, static_demo_mvp,
        translation, viewport_from_extent, CameraConfig, SliderValue, ToroidSpec, Viewport,
    };

    #[test]
    fn toroid_mesh_has_expected_vertex_and_index_counts() {
        let mesh = generate_toroid_mesh(
            ToroidSpec::<12, 8>::new(1.2, 0.3).expect("spec should be valid"),
        );

        assert_eq!(mesh.vertices.len(), 96);
        assert_eq!(mesh.indices.len(), 576);
    }

    #[test]
    fn toroid_mesh_rejects_invalid_parameters() {
        assert!(ToroidSpec::<12, 8>::new(0.0, 0.2).is_err());
        assert!(ToroidSpec::<12, 8>::new(1.0, 1.0).is_err());
        assert!(ToroidSpec::<12, 8>::new(f32::NAN, 0.2).is_err());
    }

    #[test]
    fn matrix_multiplication_preserves_translation_when_left_multiplying_identity_rotation() {
        let transform = multiply_matrices(rotation_x(0.0), translation(1.0, 2.0, 3.0));

        assert_eq!(transform[3], [1.0, 2.0, 3.0, 1.0]);
    }

    #[test]
    fn static_demo_mvp_stays_finite_for_valid_viewports() {
        let viewport = Viewport {
            origin: [0.0, 0.0],
            dimensions: [1280.0, 720.0],
            depth_range: 0.0..1.0,
        };
        let matrix = static_demo_mvp(&CameraConfig::default(), &viewport);

        assert!(matrix.iter().flatten().all(|value| value.is_finite()));
    }

    #[test]
    fn demo_layout_reserves_a_right_side_panel_when_space_allows() {
        let full_viewport = viewport_from_extent([1280, 720]);
        let layout = build_demo_layout(&full_viewport, SliderValue::MIDPOINT);

        assert!(layout.scene_viewport.dimensions[0] < full_viewport.dimensions[0]);
        assert!(layout.panel_rect.left >= layout.scene_viewport.dimensions[0]);
        assert!(layout.slider_track_rect.left >= layout.panel_rect.left);
    }

    #[test]
    fn slider_value_from_cursor_clamps_between_zero_and_one() {
        let full_viewport = viewport_from_extent([1280, 720]);
        let layout = build_demo_layout(&full_viewport, SliderValue::MIDPOINT);

        assert_eq!(slider_value_from_cursor(&layout.slider_track_rect, -100.0).get(), 1.0);
        assert_eq!(slider_value_from_cursor(&layout.slider_track_rect, 5_000.0).get(), 0.0);
    }

    #[test]
    fn ui_overlay_builds_expected_rectangles() {
        let full_viewport = viewport_from_extent([1280, 720]);
        let slider_value = SliderValue::new_clamped(0.65);
        let layout = build_demo_layout(&full_viewport, slider_value);
        let vertices = build_ui_vertices(&layout, slider_value);

        assert_eq!(vertices.len(), 30);
    }

    #[test]
    fn shader_flow_parameters_are_clamped_and_finite() {
        let low = shader_flow_parameters(SliderValue::new_clamped(-10.0));
        let high = shader_flow_parameters(SliderValue::new_clamped(10.0));

        assert_eq!(low[0], 0.0);
        assert!(low.iter().all(|value| value.is_finite()));
        assert_eq!(high[0], 1.0);
        assert!(high.iter().all(|value| value.is_finite()));
        assert!(high[1] < low[1]);
        assert!(high[2] > low[2]);
    }
}
