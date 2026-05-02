use std::f32::consts::PI;
use std::sync::Arc;

use vulkano::buffer::{BufferUsage, CpuAccessibleBuffer, CpuBufferPool, TypedBufferAccess};
use vulkano::command_buffer::{
    AutoCommandBufferBuilder, CommandBufferUsage, RenderPassBeginInfo, SubpassContents,
};
use vulkano::descriptor_set::{PersistentDescriptorSet, WriteDescriptorSet};
use vulkano::device::physical::PhysicalDeviceType;
use vulkano::device::{Device, DeviceCreateInfo, DeviceExtensions, Queue, QueueCreateInfo};
use vulkano::format::Format;
use vulkano::image::{view::ImageView, AttachmentImage, SwapchainImage};
use vulkano::impl_vertex;
use vulkano::instance::{Instance, InstanceCreateInfo};
use vulkano::pipeline::graphics::depth_stencil::DepthStencilState;
use vulkano::pipeline::graphics::input_assembly::InputAssemblyState;
use vulkano::pipeline::graphics::rasterization::{CullMode, FrontFace, RasterizationState};
use vulkano::pipeline::graphics::vertex_input::BuffersDefinition;
use vulkano::pipeline::graphics::viewport::{Viewport, ViewportState};
use vulkano::pipeline::{GraphicsPipeline, Pipeline, PipelineBindPoint};
use vulkano::render_pass::{Framebuffer, FramebufferCreateInfo, RenderPass, Subpass};
use vulkano::swapchain::{
    self, acquire_next_image, AcquireError, PresentMode, Surface, Swapchain, SwapchainCreateInfo,
    SwapchainCreationError, SwapchainPresentInfo,
};
use vulkano::sync::{self, FlushError, GpuFuture};
use vulkano::VulkanLibrary;
use vulkano_win::VkSurfaceBuild;
use winit::event::{Event, WindowEvent};
use winit::event_loop::{ControlFlow, EventLoop};
use winit::window::{Window, WindowBuilder};

#[derive(Default, Debug, Clone, Copy)]
#[repr(C)]
struct Vertex {
    position: [f32; 3],
    normal: [f32; 3],
    color: [f32; 3],
}

impl_vertex!(Vertex, position, normal, color);

type Matrix4 = [[f32; 4]; 4];

#[derive(Debug)]
struct ToroidMesh {
    vertices: Vec<Vertex>,
    indices: Vec<u32>,
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

fn run() -> Result<(), Box<dyn std::error::Error>> {
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

    let depth_format = Format::D16_UNORM;
    let render_pass = create_render_pass(device.clone(), swapchain.image_format(), depth_format)?;
    let (mut framebuffers, mut viewport) =
        window_size_dependent_setup(device.clone(), &images, render_pass.clone(), depth_format)?;

    let mesh = generate_toroid_mesh(1.4, 0.45, 96, 48)?;
    let vertex_buffer = CpuAccessibleBuffer::from_iter(
        device.clone(),
        BufferUsage::vertex_buffer(),
        false,
        mesh.vertices.iter().copied(),
    )?;
    let index_buffer = CpuAccessibleBuffer::from_iter(
        device.clone(),
        BufferUsage::index_buffer(),
        false,
        mesh.indices.iter().copied(),
    )?;

    let vs = vs::load(device.clone())?;
    let fs = fs::load(device.clone())?;
    let pipeline = create_pipeline(device.clone(), render_pass.clone(), vs, fs)?;
    let uniform_buffer =
        CpuBufferPool::<vs::ty::Data>::new(device.clone(), BufferUsage::uniform_buffer());
    let camera = CameraConfig::default();

    let mut recreate_swapchain = false;
    let mut previous_frame_end = Some(sync::now(device.clone()).boxed());

    event_loop.run(move |event, _, control_flow| {
        *control_flow = ControlFlow::Poll;

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
            }
            Event::WindowEvent {
                event: WindowEvent::ScaleFactorChanged { .. },
                ..
            } => {
                recreate_swapchain = true;
            }
            Event::RedrawEventsCleared => {
                let Some(future) = previous_frame_end.as_mut() else {
                    return;
                };
                future.cleanup_finished();

                if recreate_swapchain {
                    let Some(image_extent) = current_window_extent(&surface) else {
                        return;
                    };

                    match swapchain.recreate(SwapchainCreateInfo {
                        image_extent,
                        ..swapchain.create_info()
                    }) {
                        Ok((new_swapchain, new_images)) => {
                            swapchain = new_swapchain;
                            match window_size_dependent_setup(
                                device.clone(),
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
                            return;
                        }
                        Err(error) => {
                            eprintln!("Failed to acquire swapchain image: {error}");
                            *control_flow = ControlFlow::Exit;
                            return;
                        }
                    };

                if suboptimal {
                    recreate_swapchain = true;
                }

                let mvp = static_demo_mvp(&camera, &viewport);
                let uniform_data = vs::ty::Data { mvp };
                let uniform_subbuffer = match uniform_buffer.from_data(uniform_data) {
                    Ok(buffer) => buffer,
                    Err(error) => {
                        eprintln!("Failed to allocate uniform buffer: {error}");
                        *control_flow = ControlFlow::Exit;
                        return;
                    }
                };

                let layout = pipeline
                    .layout()
                    .set_layouts()
                    .get(0)
                    .expect("pipeline should expose one descriptor-set layout")
                    .clone();
                let descriptor_set = match PersistentDescriptorSet::new(
                    layout,
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
                    device.clone(),
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

                if let Err(error) = builder
                    .begin_render_pass(
                        RenderPassBeginInfo {
                            clear_values: vec![
                                Some([0.02, 0.02, 0.05, 1.0].into()),
                                Some(1_f32.into()),
                            ],
                            ..RenderPassBeginInfo::framebuffer(
                                framebuffers[image_index as usize].clone(),
                            )
                        },
                        SubpassContents::Inline,
                    )
                    .and_then(|builder| builder.set_viewport(0, [viewport.clone()]))
                    .and_then(|builder| builder.bind_pipeline_graphics(pipeline.clone()))
                    .and_then(|builder| {
                        builder.bind_descriptor_sets(
                            PipelineBindPoint::Graphics,
                            pipeline.layout().clone(),
                            0,
                            descriptor_set,
                        )
                    })
                    .and_then(|builder| builder.bind_vertex_buffers(0, vertex_buffer.clone()))
                    .and_then(|builder| builder.bind_index_buffer(index_buffer.clone()))
                    .and_then(|builder| builder.draw_indexed(index_buffer.len() as u32, 1, 0, 0, 0))
                    .and_then(|builder| builder.end_render_pass())
                {
                    eprintln!("Failed to record command buffer: {error}");
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
                        *control_flow = ControlFlow::Exit;
                        return;
                    }
                };

                match future {
                    Ok(future) => {
                        previous_frame_end = Some(future.boxed());
                    }
                    Err(FlushError::OutOfDate) => {
                        recreate_swapchain = true;
                        previous_frame_end = Some(sync::now(device.clone()).boxed());
                    }
                    Err(error) => {
                        eprintln!("Failed to flush GPU future: {error}");
                        previous_frame_end = Some(sync::now(device.clone()).boxed());
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
                    queue_family_properties.queue_flags.graphics
                        && device
                            .surface_support(*queue_family_index as u32, &surface)
                            .unwrap_or(false)
                })
                .map(|(queue_family_index, _)| (device, queue_family_index as u32))
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
) -> Result<(Arc<Swapchain>, Vec<Arc<SwapchainImage>>), Box<dyn std::error::Error>> {
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
            image_usage: vulkano::image::ImageUsage::color_attachment(),
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
) -> Result<Arc<RenderPass>, Box<dyn std::error::Error>> {
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

fn create_pipeline(
    device: Arc<Device>,
    render_pass: Arc<RenderPass>,
    vs: Arc<vulkano::shader::ShaderModule>,
    fs: Arc<vulkano::shader::ShaderModule>,
) -> Result<Arc<GraphicsPipeline>, Box<dyn std::error::Error>> {
    let subpass = Subpass::from(render_pass, 0)?;

    Ok(GraphicsPipeline::start()
        .vertex_input_state(BuffersDefinition::new().vertex::<Vertex>())
        .vertex_shader(vs.entry_point("main")?, ())
        .input_assembly_state(InputAssemblyState::new())
        .viewport_state(ViewportState::viewport_dynamic_scissor_irrelevant())
        .depth_stencil_state(DepthStencilState::simple_depth_test())
        .rasterization_state(
            RasterizationState::new()
                .cull_mode(CullMode::Back)
                .front_face(FrontFace::CounterClockwise),
        )
        .fragment_shader(fs.entry_point("main")?, ())
        .render_pass(subpass)
        .build(device)?)
}

fn window_size_dependent_setup(
    device: Arc<Device>,
    images: &[Arc<SwapchainImage>],
    render_pass: Arc<RenderPass>,
    depth_format: Format,
) -> Result<(Vec<Arc<Framebuffer>>, Viewport), Box<dyn std::error::Error>> {
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
                AttachmentImage::transient(device.clone(), dimensions, depth_format)?;
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

fn generate_toroid_mesh(
    major_radius: f32,
    minor_radius: f32,
    major_segments: u32,
    minor_segments: u32,
) -> Result<ToroidMesh, &'static str> {
    if !major_radius.is_finite() || !minor_radius.is_finite() {
        return Err("Toroid radii must be finite.");
    }
    if major_radius <= 0.0 || minor_radius <= 0.0 {
        return Err("Toroid radii must be positive.");
    }
    if major_radius <= minor_radius {
        return Err("Major radius must exceed minor radius for this demo toroid.");
    }
    if major_segments < 3 || minor_segments < 3 {
        return Err("Toroid segment counts must both be at least 3.");
    }

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

            vertices.push(Vertex {
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

    Ok(ToroidMesh { vertices, indices })
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

mod vs {
    vulkano_shaders::shader! {
        ty: "vertex",
        src: r#"
            #version 450

            layout(location = 0) in vec3 position;
            layout(location = 1) in vec3 normal;
            layout(location = 2) in vec3 color;

            layout(location = 0) out vec3 vertex_color;

            layout(set = 0, binding = 0) uniform Data {
                mat4 mvp;
            } uniforms;

            void main() {
                gl_Position = uniforms.mvp * vec4(position, 1.0);

                vec3 light_direction = normalize(vec3(0.4, 1.0, 0.7));
                float diffuse = max(dot(normalize(normal), light_direction), 0.2);
                vertex_color = color * diffuse;
            }
        "#
    }
}

mod fs {
    vulkano_shaders::shader! {
        ty: "fragment",
        src: r#"
            #version 450

            layout(location = 0) in vec3 vertex_color;
            layout(location = 0) out vec4 fragment_color;

            void main() {
                fragment_color = vec4(vertex_color, 1.0);
            }
        "#
    }
}

#[cfg(test)]
mod tests {
    use super::{
        current_window_extent, generate_toroid_mesh, multiply_matrices, rotation_x,
        static_demo_mvp, translation, CameraConfig, Viewport,
    };

    #[test]
    fn toroid_mesh_has_expected_vertex_and_index_counts() {
        let mesh = generate_toroid_mesh(1.2, 0.3, 12, 8).expect("mesh generation should succeed");

        assert_eq!(mesh.vertices.len(), 96);
        assert_eq!(mesh.indices.len(), 576);
    }

    #[test]
    fn toroid_mesh_rejects_invalid_parameters() {
        assert!(generate_toroid_mesh(0.0, 0.2, 12, 8).is_err());
        assert!(generate_toroid_mesh(1.0, 1.0, 12, 8).is_err());
        assert!(generate_toroid_mesh(1.0, 0.2, 2, 8).is_err());
        assert!(generate_toroid_mesh(f32::NAN, 0.2, 12, 8).is_err());
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
}
