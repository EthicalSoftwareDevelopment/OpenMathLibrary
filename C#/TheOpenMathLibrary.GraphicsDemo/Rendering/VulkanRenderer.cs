using System.Numerics;
using System.Runtime.InteropServices;
using Silk.NET.Core.Native;
using Silk.NET.Shaderc;
using Silk.NET.Vulkan;
using TheOpenMathLibrary.GraphicsDemo.Geometry;
using Buffer = Silk.NET.Vulkan.Buffer;
using Image = Silk.NET.Vulkan.Image;
using Semaphore = Silk.NET.Vulkan.Semaphore;

namespace TheOpenMathLibrary.GraphicsDemo.Rendering;

/// <summary>
/// Owns the Vulkan resources required to render the toroid demo.
/// </summary>
public unsafe sealed class VulkanRenderer : IDisposable
{
    private const string KhrSwapchainExtensionName = "VK_KHR_swapchain";

    private readonly string _applicationName;
    private readonly GlfwWindowHost _windowHost;
    private readonly Vk _vk;
    private readonly Shaderc _shaderc;

    private Buffer _indexBuffer;
    private CommandBuffer[] _commandBuffers = [];
    private CommandPool _commandPool;
    private Device _device;
    private DeviceMemory _depthImageMemory;
    private Pipeline _hudGraphicsPipeline;
    private PipelineLayout _hudPipelineLayout;
    private Buffer _hudVertexBuffer;
    private DeviceMemory _hudVertexBufferMemory;
    private uint _hudVertexCount;
    private DeviceMemory _indexBufferMemory;
    private DeviceMemory _vertexBufferMemory;
    private Extent2D _swapchainExtent;
    private Fence _inFlightFence;
    private Framebuffer[] _framebuffers = [];
    private Queue _graphicsQueue;
    private uint _graphicsQueueFamilyIndex;
    private Pipeline _solidGraphicsPipeline;
    private Pipeline _wireframeGraphicsPipeline;
    private Image _depthImage;
    private ImageView _depthImageView;
    private Format _depthFormat;
    private Semaphore _imageAvailableSemaphore;
    private Image[] _swapchainImages = [];
    private ImageView[] _swapchainImageViews = [];
    private uint _indexCount;
    private Instance _instance;
    private bool _isInitialized;
    private PhysicalDevice _physicalDevice;
    private PipelineLayout _pipelineLayout;
    private RenderPass _renderPass;
    private Semaphore _renderFinishedSemaphore;
    private SurfaceKHR _surface;
    private SwapchainKHR _swapchain;
    private Format _swapchainImageFormat;
    private bool _supportsWireframe;
    private Buffer _vertexBuffer;
    private VkGetPhysicalDeviceSurfaceSupportKhr? _getPhysicalDeviceSurfaceSupportKhr;
    private VkGetPhysicalDeviceSurfaceCapabilitiesKhr? _getPhysicalDeviceSurfaceCapabilitiesKhr;
    private VkGetPhysicalDeviceSurfaceFormatsKhr? _getPhysicalDeviceSurfaceFormatsKhr;
    private VkGetPhysicalDeviceSurfacePresentModesKhr? _getPhysicalDeviceSurfacePresentModesKhr;
    private VkDestroySurfaceKhr? _destroySurfaceKhr;
    private VkCreateSwapchainKhr? _createSwapchainKhr;
    private VkDestroySwapchainKhr? _destroySwapchainKhr;
    private VkGetSwapchainImagesKhr? _getSwapchainImagesKhr;
    private VkAcquireNextImageKhr? _acquireNextImageKhr;
    private VkQueuePresentKhr? _queuePresentKhr;

    /// <summary>
    /// Initializes a new instance of the <see cref="VulkanRenderer"/> class.
    /// </summary>
    public VulkanRenderer(GlfwWindowHost windowHost, string applicationName)
    {
        _windowHost = windowHost ?? throw new ArgumentNullException(nameof(windowHost));
        _applicationName = applicationName;
        _vk = Vk.GetApi();
        _shaderc = Shaderc.GetApi();
    }

    /// <summary>
    /// Gets a value indicating whether the selected Vulkan device supports wireframe rasterization.
    /// </summary>
    public bool SupportsWireframe => _supportsWireframe;

    /// <summary>
    /// Initializes the Vulkan device and renderer resources.
    /// </summary>
    public void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }

        CreateInstance();
        CreateSurface();
        LoadInstanceExtensions();
        PickPhysicalDevice();
        CreateLogicalDevice();
        CreateCommandPool();
        CreateGeometryBuffers();
        CreateSyncObjects();
        RecreateSwapchain(_windowHost.FramebufferWidth, _windowHost.FramebufferHeight);
        _isInitialized = true;
    }

    /// <summary>
    /// Renders a single frame using the supplied camera matrix.
    /// </summary>
    public void RenderFrame(DemoRenderOptions renderOptions, int framebufferWidth, int framebufferHeight)
    {
        EnsureInitialized();

        if (framebufferWidth <= 0 || framebufferHeight <= 0)
        {
            return;
        }

        if (_swapchainExtent.Width != framebufferWidth || _swapchainExtent.Height != framebufferHeight)
        {
            RecreateSwapchain(framebufferWidth, framebufferHeight);
        }

        if (_framebuffers.Length == 0 || _commandBuffers.Length == 0)
        {
            RecreateSwapchain(framebufferWidth, framebufferHeight);
            if (_framebuffers.Length == 0 || _commandBuffers.Length == 0)
            {
                return;
            }
        }

        UpdateHudResources(renderOptions.ShaderSliderValue);

        Check(_vk.WaitForFences(_device, 1, in _inFlightFence, true, ulong.MaxValue), "wait for the in-flight fence");
        Check(_vk.ResetFences(_device, 1, in _inFlightFence), "reset the in-flight fence");

        Result acquireResult = _acquireNextImageKhr!(_device, _swapchain, ulong.MaxValue, _imageAvailableSemaphore, default, out uint imageIndex);
        if (acquireResult == Result.ErrorOutOfDateKhr)
        {
            RecreateSwapchain(framebufferWidth, framebufferHeight);
            return;
        }

        Check(acquireResult, "acquire the next swapchain image");

        Check(_vk.ResetCommandBuffer(_commandBuffers[imageIndex], 0), "reset the command buffer");
        RecordCommandBuffer(_commandBuffers[imageIndex], _framebuffers[imageIndex], renderOptions);

        Semaphore waitSemaphore = _imageAvailableSemaphore;
        Semaphore signalSemaphore = _renderFinishedSemaphore;
        PipelineStageFlags waitStage = PipelineStageFlags.ColorAttachmentOutputBit;

        fixed (CommandBuffer* commandBufferPtr = &_commandBuffers[imageIndex])
        {
            Semaphore* waitSemaphorePtr = &waitSemaphore;
            Semaphore* signalSemaphorePtr = &signalSemaphore;
            PipelineStageFlags* waitStagePtr = &waitStage;

            SubmitInfo submitInfo = new()
            {
                SType = StructureType.SubmitInfo,
                WaitSemaphoreCount = 1,
                PWaitSemaphores = waitSemaphorePtr,
                PWaitDstStageMask = waitStagePtr,
                CommandBufferCount = 1,
                PCommandBuffers = commandBufferPtr,
                SignalSemaphoreCount = 1,
                PSignalSemaphores = signalSemaphorePtr,
            };

            Check(_vk.QueueSubmit(_graphicsQueue, 1, in submitInfo, _inFlightFence), "submit draw commands");
        }

        {
            SwapchainKHR* swapchainPtr = stackalloc SwapchainKHR[1];
            swapchainPtr[0] = _swapchain;

            uint* imageIndexPtr = stackalloc uint[1];
            imageIndexPtr[0] = imageIndex;

            Semaphore* signalSemaphorePtr = &signalSemaphore;

            PresentInfoKHR presentInfo = new()
            {
                SType = StructureType.PresentInfoKhr,
                WaitSemaphoreCount = 1,
                PWaitSemaphores = signalSemaphorePtr,
                SwapchainCount = 1,
                PSwapchains = swapchainPtr,
                PImageIndices = imageIndexPtr,
            };

            Result presentResult = _queuePresentKhr!(_graphicsQueue, in presentInfo);
            if (presentResult == Result.ErrorOutOfDateKhr || presentResult == Result.SuboptimalKhr)
            {
                RecreateSwapchain(framebufferWidth, framebufferHeight);
                return;
            }

            Check(presentResult, "present the rendered frame");
        }
    }

    /// <summary>
    /// Waits until the device is idle.
    /// </summary>
    public void WaitIdle()
    {
        if (_device.Handle != 0)
        {
            _vk.DeviceWaitIdle(_device);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_isInitialized)
        {
            return;
        }

        WaitIdle();
        DestroySwapchainResources();

        if (_vertexBuffer.Handle != 0)
        {
            _vk.DestroyBuffer(_device, _vertexBuffer, null);
        }

        if (_vertexBufferMemory.Handle != 0)
        {
            _vk.FreeMemory(_device, _vertexBufferMemory, null);
        }

        if (_indexBuffer.Handle != 0)
        {
            _vk.DestroyBuffer(_device, _indexBuffer, null);
        }

        if (_indexBufferMemory.Handle != 0)
        {
            _vk.FreeMemory(_device, _indexBufferMemory, null);
        }

        if (_commandPool.Handle != 0)
        {
            _vk.DestroyCommandPool(_device, _commandPool, null);
        }

        if (_imageAvailableSemaphore.Handle != 0)
        {
            _vk.DestroySemaphore(_device, _imageAvailableSemaphore, null);
        }

        if (_renderFinishedSemaphore.Handle != 0)
        {
            _vk.DestroySemaphore(_device, _renderFinishedSemaphore, null);
        }

        if (_inFlightFence.Handle != 0)
        {
            _vk.DestroyFence(_device, _inFlightFence, null);
        }

        if (_device.Handle != 0)
        {
            _vk.DestroyDevice(_device, null);
        }

        if (_surface.Handle != 0)
        {
            _destroySurfaceKhr!(_instance, _surface, null);
        }

        if (_instance.Handle != 0)
        {
            _vk.DestroyInstance(_instance, null);
        }

        _isInitialized = false;
    }

    private void CreateInstance()
    {
        if (!_windowHost.Glfw.VulkanSupported())
        {
            throw new InvalidOperationException("GLFW reports that Vulkan is not available on this machine.");
        }

        byte* applicationNamePtr = (byte*)SilkMarshal.StringToPtr(_applicationName);
        byte* engineNamePtr = (byte*)SilkMarshal.StringToPtr("TheOpenMathLibrary");

        try
        {
            byte** requiredExtensions = _windowHost.GetRequiredInstanceExtensions(out uint extensionCount);

            ApplicationInfo applicationInfo = new()
            {
                SType = StructureType.ApplicationInfo,
                PApplicationName = applicationNamePtr,
                ApplicationVersion = Vk.MakeVersion(1, 0),
                PEngineName = engineNamePtr,
                EngineVersion = Vk.MakeVersion(1, 0),
                ApiVersion = Vk.Version11,
            };

            InstanceCreateInfo createInfo = new()
            {
                SType = StructureType.InstanceCreateInfo,
                PApplicationInfo = &applicationInfo,
                EnabledExtensionCount = extensionCount,
                PpEnabledExtensionNames = requiredExtensions,
            };

            Check(_vk.CreateInstance(in createInfo, null, out _instance), "create the Vulkan instance");
        }
        finally
        {
            SilkMarshal.Free((nint)applicationNamePtr);
            SilkMarshal.Free((nint)engineNamePtr);
        }
    }

    private void LoadInstanceExtensions()
    {
        _getPhysicalDeviceSurfaceSupportKhr = LoadInstanceProc<VkGetPhysicalDeviceSurfaceSupportKhr>("vkGetPhysicalDeviceSurfaceSupportKHR");
        _getPhysicalDeviceSurfaceCapabilitiesKhr = LoadInstanceProc<VkGetPhysicalDeviceSurfaceCapabilitiesKhr>("vkGetPhysicalDeviceSurfaceCapabilitiesKHR");
        _getPhysicalDeviceSurfaceFormatsKhr = LoadInstanceProc<VkGetPhysicalDeviceSurfaceFormatsKhr>("vkGetPhysicalDeviceSurfaceFormatsKHR");
        _getPhysicalDeviceSurfacePresentModesKhr = LoadInstanceProc<VkGetPhysicalDeviceSurfacePresentModesKhr>("vkGetPhysicalDeviceSurfacePresentModesKHR");
        _destroySurfaceKhr = LoadInstanceProc<VkDestroySurfaceKhr>("vkDestroySurfaceKHR");
    }

    private void CreateSurface()
    {
        VkNonDispatchableHandle surfaceHandle = default;
        Result result = (Result)_windowHost.Glfw.CreateWindowSurface(_instance.ToHandle(), _windowHost.Handle, null, &surfaceHandle);
        Check(result, "create the GLFW presentation surface");
        _surface = new SurfaceKHR(surfaceHandle.Handle);
    }

    private void PickPhysicalDevice()
    {
        uint deviceCount = 0;
        Check(_vk.EnumeratePhysicalDevices(_instance, &deviceCount, null), "enumerate physical devices");
        if (deviceCount == 0)
        {
            throw new InvalidOperationException("No Vulkan-compatible physical devices were found.");
        }

        PhysicalDevice[] devices = new PhysicalDevice[deviceCount];
        fixed (PhysicalDevice* devicesPtr = devices)
        {
            Check(_vk.EnumeratePhysicalDevices(_instance, &deviceCount, devicesPtr), "retrieve the physical devices");
        }

        foreach (PhysicalDevice device in devices)
        {
            if (TrySelectQueueFamily(device, out uint queueFamilyIndex))
            {
                _physicalDevice = device;
                _graphicsQueueFamilyIndex = queueFamilyIndex;
                return;
            }
        }

        throw new InvalidOperationException("No suitable physical device with graphics, presentation, and swapchain support was found.");
    }

    private bool TrySelectQueueFamily(PhysicalDevice device, out uint queueFamilyIndex)
    {
        queueFamilyIndex = 0;

        if (!SupportsSwapchain(device))
        {
            return false;
        }

        uint propertyCount = 0;
        _vk.GetPhysicalDeviceQueueFamilyProperties(device, &propertyCount, null);
        QueueFamilyProperties[] properties = new QueueFamilyProperties[propertyCount];
        fixed (QueueFamilyProperties* propertiesPtr = properties)
        {
            _vk.GetPhysicalDeviceQueueFamilyProperties(device, &propertyCount, propertiesPtr);
        }

        for (uint index = 0; index < propertyCount; index++)
        {
            uint supportsPresentation;
            Check(_getPhysicalDeviceSurfaceSupportKhr!(device, index, _surface, out supportsPresentation), "query surface presentation support");
            if ((properties[index].QueueFlags & QueueFlags.GraphicsBit) != 0 && supportsPresentation != 0)
            {
                queueFamilyIndex = index;
                return true;
            }
        }

        return false;
    }

    private bool SupportsSwapchain(PhysicalDevice device)
    {
        uint extensionCount = 0;
        Check(_vk.EnumerateDeviceExtensionProperties(device, (byte*)null, &extensionCount, null), "enumerate device extensions");
        ExtensionProperties[] extensions = new ExtensionProperties[extensionCount];
        fixed (ExtensionProperties* extensionsPtr = extensions)
        {
            Check(_vk.EnumerateDeviceExtensionProperties(device, (byte*)null, &extensionCount, extensionsPtr), "retrieve device extensions");
        }

        foreach (ExtensionProperties extension in extensions)
        {
            string? extensionName = Marshal.PtrToStringAnsi((nint)extension.ExtensionName);
            if (string.Equals(extensionName, KhrSwapchainExtensionName, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private void CreateLogicalDevice()
    {
        float queuePriority = 1f;
        byte** extensionNames = (byte**)SilkMarshal.StringArrayToPtr([KhrSwapchainExtensionName]);
        _vk.GetPhysicalDeviceFeatures(_physicalDevice, out PhysicalDeviceFeatures supportedFeatures);
        _supportsWireframe = supportedFeatures.FillModeNonSolid != 0u;

        try
        {
            DeviceQueueCreateInfo queueCreateInfo = new()
            {
                SType = StructureType.DeviceQueueCreateInfo,
                QueueFamilyIndex = _graphicsQueueFamilyIndex,
                QueueCount = 1,
                PQueuePriorities = &queuePriority,
            };

            PhysicalDeviceFeatures features = new()
            {
                FillModeNonSolid = _supportsWireframe ? 1u : 0u,
            };
            DeviceCreateInfo createInfo = new()
            {
                SType = StructureType.DeviceCreateInfo,
                QueueCreateInfoCount = 1,
                PQueueCreateInfos = &queueCreateInfo,
                EnabledExtensionCount = 1,
                PpEnabledExtensionNames = extensionNames,
                PEnabledFeatures = &features,
            };

            Check(_vk.CreateDevice(_physicalDevice, in createInfo, null, out _device), "create the logical device");
            _vk.GetDeviceQueue(_device, _graphicsQueueFamilyIndex, 0, out _graphicsQueue);

            _createSwapchainKhr = LoadDeviceProc<VkCreateSwapchainKhr>("vkCreateSwapchainKHR");
            _destroySwapchainKhr = LoadDeviceProc<VkDestroySwapchainKhr>("vkDestroySwapchainKHR");
            _getSwapchainImagesKhr = LoadDeviceProc<VkGetSwapchainImagesKhr>("vkGetSwapchainImagesKHR");
            _acquireNextImageKhr = LoadDeviceProc<VkAcquireNextImageKhr>("vkAcquireNextImageKHR");
            _queuePresentKhr = LoadDeviceProc<VkQueuePresentKhr>("vkQueuePresentKHR");
        }
        finally
        {
            SilkMarshal.Free((nint)extensionNames);
        }
    }

    private void CreateCommandPool()
    {
        CommandPoolCreateInfo createInfo = new()
        {
            SType = StructureType.CommandPoolCreateInfo,
            Flags = CommandPoolCreateFlags.ResetCommandBufferBit,
            QueueFamilyIndex = _graphicsQueueFamilyIndex,
        };

        Check(_vk.CreateCommandPool(_device, in createInfo, null, out _commandPool), "create the command pool");
    }

    private void CreateGeometryBuffers()
    {
        ToroidMesh mesh = GraphicsDemoScene.CreateToroidMesh();
        Vertex[] vertices = mesh.Vertices;
        uint[] indices = mesh.Indices;
        _indexCount = (uint)indices.Length;

        ulong vertexBufferSize = (ulong)(Vertex.SizeInBytes * vertices.Length);
        ulong indexBufferSize = (ulong)(sizeof(uint) * indices.Length);

        CreateBuffer(vertexBufferSize, BufferUsageFlags.VertexBufferBit, MemoryPropertyFlags.HostVisibleBit | MemoryPropertyFlags.HostCoherentBit, out _vertexBuffer, out _vertexBufferMemory);
        CreateBuffer(indexBufferSize, BufferUsageFlags.IndexBufferBit, MemoryPropertyFlags.HostVisibleBit | MemoryPropertyFlags.HostCoherentBit, out _indexBuffer, out _indexBufferMemory);

        CopyToDeviceMemory(_vertexBufferMemory, vertexBufferSize, vertices);
        CopyToDeviceMemory(_indexBufferMemory, indexBufferSize, indices);
    }

    private void RecreateSwapchain(int framebufferWidth, int framebufferHeight)
    {
        if (framebufferWidth <= 0 || framebufferHeight <= 0)
        {
            return;
        }

        WaitIdle();
        DestroySwapchainResources();

        SwapchainSupportDetails details = QuerySwapchainSupport();
        SurfaceFormatKHR surfaceFormat = ChooseSurfaceFormat(details.Formats);
        PresentModeKHR presentMode = ChoosePresentMode(details.PresentModes);
        Extent2D extent = ChooseExtent(details.Capabilities, framebufferWidth, framebufferHeight);

        uint imageCount = details.Capabilities.MinImageCount + 1;
        if (details.Capabilities.MaxImageCount > 0 && imageCount > details.Capabilities.MaxImageCount)
        {
            imageCount = details.Capabilities.MaxImageCount;
        }

        SwapchainCreateInfoKHR createInfo = new()
        {
            SType = StructureType.SwapchainCreateInfoKhr,
            Surface = _surface,
            MinImageCount = imageCount,
            ImageFormat = surfaceFormat.Format,
            ImageColorSpace = surfaceFormat.ColorSpace,
            ImageExtent = extent,
            ImageArrayLayers = 1,
            ImageUsage = ImageUsageFlags.ColorAttachmentBit,
            ImageSharingMode = SharingMode.Exclusive,
            PreTransform = details.Capabilities.CurrentTransform,
            CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr,
            PresentMode = presentMode,
            Clipped = true,
            OldSwapchain = default,
        };

        Check(_createSwapchainKhr!(_device, in createInfo, null, out _swapchain), "create the swapchain");
        _swapchainImageFormat = surfaceFormat.Format;
        _swapchainExtent = extent;

        uint swapchainImageCount = 0;
        Check(_getSwapchainImagesKhr!(_device, _swapchain, &swapchainImageCount, null), "enumerate the swapchain images");
        _swapchainImages = new Image[swapchainImageCount];
        fixed (Image* imagesPtr = _swapchainImages)
        {
            Check(_getSwapchainImagesKhr!(_device, _swapchain, &swapchainImageCount, imagesPtr), "retrieve the swapchain images");
        }

        _swapchainImageViews = new ImageView[_swapchainImages.Length];
        for (int index = 0; index < _swapchainImages.Length; index++)
        {
            _swapchainImageViews[index] = CreateImageView(_swapchainImages[index], _swapchainImageFormat, ImageAspectFlags.ColorBit);
        }

        _depthFormat = FindDepthFormat();
        CreateDepthResources();
        CreateRenderPass();
        CreateGraphicsPipelines();
        CreateHudResources();
        CreateFramebuffers();
        AllocateCommandBuffers();
    }

    private void DestroySwapchainResources()
    {
        foreach (CommandBuffer commandBuffer in _commandBuffers)
        {
            if (commandBuffer.Handle != 0)
            {
                _vk.FreeCommandBuffers(_device, _commandPool, 1, in commandBuffer);
            }
        }

        _commandBuffers = [];

        foreach (Framebuffer framebuffer in _framebuffers)
        {
            if (framebuffer.Handle != 0)
            {
                _vk.DestroyFramebuffer(_device, framebuffer, null);
            }
        }

        _framebuffers = [];

        if (_solidGraphicsPipeline.Handle != 0)
        {
            _vk.DestroyPipeline(_device, _solidGraphicsPipeline, null);
            _solidGraphicsPipeline = default;
        }

        if (_wireframeGraphicsPipeline.Handle != 0)
        {
            _vk.DestroyPipeline(_device, _wireframeGraphicsPipeline, null);
            _wireframeGraphicsPipeline = default;
        }

        if (_hudGraphicsPipeline.Handle != 0)
        {
            _vk.DestroyPipeline(_device, _hudGraphicsPipeline, null);
            _hudGraphicsPipeline = default;
        }

        if (_pipelineLayout.Handle != 0)
        {
            _vk.DestroyPipelineLayout(_device, _pipelineLayout, null);
            _pipelineLayout = default;
        }

        if (_hudPipelineLayout.Handle != 0)
        {
            _vk.DestroyPipelineLayout(_device, _hudPipelineLayout, null);
            _hudPipelineLayout = default;
        }

        if (_hudVertexBuffer.Handle != 0)
        {
            _vk.DestroyBuffer(_device, _hudVertexBuffer, null);
            _hudVertexBuffer = default;
        }

        if (_hudVertexBufferMemory.Handle != 0)
        {
            _vk.FreeMemory(_device, _hudVertexBufferMemory, null);
            _hudVertexBufferMemory = default;
        }

        _hudVertexCount = 0;

        if (_renderPass.Handle != 0)
        {
            _vk.DestroyRenderPass(_device, _renderPass, null);
            _renderPass = default;
        }

        if (_depthImageView.Handle != 0)
        {
            _vk.DestroyImageView(_device, _depthImageView, null);
            _depthImageView = default;
        }

        if (_depthImage.Handle != 0)
        {
            _vk.DestroyImage(_device, _depthImage, null);
            _depthImage = default;
        }

        if (_depthImageMemory.Handle != 0)
        {
            _vk.FreeMemory(_device, _depthImageMemory, null);
            _depthImageMemory = default;
        }

        foreach (ImageView imageView in _swapchainImageViews)
        {
            if (imageView.Handle != 0)
            {
                _vk.DestroyImageView(_device, imageView, null);
            }
        }

        _swapchainImageViews = [];
        _swapchainImages = [];

        if (_swapchain.Handle != 0)
        {
            _destroySwapchainKhr!(_device, _swapchain, null);
            _swapchain = default;
        }
    }

    private SwapchainSupportDetails QuerySwapchainSupport()
    {
        Check(_getPhysicalDeviceSurfaceCapabilitiesKhr!(_physicalDevice, _surface, out SurfaceCapabilitiesKHR capabilities), "query surface capabilities");

        uint formatCount = 0;
        Check(_getPhysicalDeviceSurfaceFormatsKhr!(_physicalDevice, _surface, &formatCount, null), "enumerate surface formats");
        SurfaceFormatKHR[] formats = new SurfaceFormatKHR[formatCount];
        fixed (SurfaceFormatKHR* formatsPtr = formats)
        {
            Check(_getPhysicalDeviceSurfaceFormatsKhr!(_physicalDevice, _surface, &formatCount, formatsPtr), "retrieve surface formats");
        }

        uint presentModeCount = 0;
        Check(_getPhysicalDeviceSurfacePresentModesKhr!(_physicalDevice, _surface, &presentModeCount, null), "enumerate present modes");
        PresentModeKHR[] presentModes = new PresentModeKHR[presentModeCount];
        fixed (PresentModeKHR* presentModesPtr = presentModes)
        {
            Check(_getPhysicalDeviceSurfacePresentModesKhr!(_physicalDevice, _surface, &presentModeCount, presentModesPtr), "retrieve present modes");
        }

        return new SwapchainSupportDetails(capabilities, formats, presentModes);
    }

    private static SurfaceFormatKHR ChooseSurfaceFormat(SurfaceFormatKHR[] formats)
    {
        foreach (SurfaceFormatKHR format in formats)
        {
            if (format.Format == Format.B8G8R8A8Srgb && format.ColorSpace == ColorSpaceKHR.PaceSrgbNonlinearKhr)
            {
                return format;
            }
        }

        return formats[0];
    }

    private static PresentModeKHR ChoosePresentMode(PresentModeKHR[] presentModes)
    {
        foreach (PresentModeKHR presentMode in presentModes)
        {
            if (presentMode == PresentModeKHR.MailboxKhr)
            {
                return presentMode;
            }
        }

        return PresentModeKHR.FifoKhr;
    }

    private static Extent2D ChooseExtent(SurfaceCapabilitiesKHR capabilities, int framebufferWidth, int framebufferHeight)
    {
        if (capabilities.CurrentExtent.Width != uint.MaxValue)
        {
            return capabilities.CurrentExtent;
        }

        return new Extent2D
        {
            Width = (uint)Math.Clamp(framebufferWidth, (int)capabilities.MinImageExtent.Width, (int)capabilities.MaxImageExtent.Width),
            Height = (uint)Math.Clamp(framebufferHeight, (int)capabilities.MinImageExtent.Height, (int)capabilities.MaxImageExtent.Height),
        };
    }

    private void CreateDepthResources()
    {
        CreateImage(
            _swapchainExtent.Width,
            _swapchainExtent.Height,
            _depthFormat,
            ImageTiling.Optimal,
            ImageUsageFlags.DepthStencilAttachmentBit,
            MemoryPropertyFlags.DeviceLocalBit,
            out _depthImage,
            out _depthImageMemory);

        _depthImageView = CreateImageView(_depthImage, _depthFormat, ImageAspectFlags.DepthBit);
    }

    private void CreateRenderPass()
    {
        AttachmentDescription colorAttachment = new()
        {
            Format = _swapchainImageFormat,
            Samples = SampleCountFlags.Count1Bit,
            LoadOp = AttachmentLoadOp.Clear,
            StoreOp = AttachmentStoreOp.Store,
            StencilLoadOp = AttachmentLoadOp.DontCare,
            StencilStoreOp = AttachmentStoreOp.DontCare,
            InitialLayout = ImageLayout.Undefined,
            FinalLayout = ImageLayout.PresentSrcKhr,
        };

        AttachmentDescription depthAttachment = new()
        {
            Format = _depthFormat,
            Samples = SampleCountFlags.Count1Bit,
            LoadOp = AttachmentLoadOp.Clear,
            StoreOp = AttachmentStoreOp.DontCare,
            StencilLoadOp = AttachmentLoadOp.DontCare,
            StencilStoreOp = AttachmentStoreOp.DontCare,
            InitialLayout = ImageLayout.Undefined,
            FinalLayout = ImageLayout.DepthStencilAttachmentOptimal,
        };

        AttachmentReference colorAttachmentReference = new()
        {
            Attachment = 0,
            Layout = ImageLayout.ColorAttachmentOptimal,
        };

        AttachmentReference depthAttachmentReference = new()
        {
            Attachment = 1,
            Layout = ImageLayout.DepthStencilAttachmentOptimal,
        };

        SubpassDescription subpass = new()
        {
            PipelineBindPoint = PipelineBindPoint.Graphics,
            ColorAttachmentCount = 1,
            PColorAttachments = &colorAttachmentReference,
            PDepthStencilAttachment = &depthAttachmentReference,
        };

        SubpassDependency dependency = new()
        {
            SrcSubpass = Vk.SubpassExternal,
            DstSubpass = 0,
            SrcStageMask = PipelineStageFlags.ColorAttachmentOutputBit | PipelineStageFlags.EarlyFragmentTestsBit,
            DstStageMask = PipelineStageFlags.ColorAttachmentOutputBit | PipelineStageFlags.EarlyFragmentTestsBit,
            DstAccessMask = AccessFlags.ColorAttachmentWriteBit | AccessFlags.DepthStencilAttachmentWriteBit,
        };

        AttachmentDescription* attachments = stackalloc AttachmentDescription[2];
        attachments[0] = colorAttachment;
        attachments[1] = depthAttachment;

        RenderPassCreateInfo createInfo = new()
        {
            SType = StructureType.RenderPassCreateInfo,
            AttachmentCount = 2,
            PAttachments = attachments,
            SubpassCount = 1,
            PSubpasses = &subpass,
            DependencyCount = 1,
            PDependencies = &dependency,
        };

        Check(_vk.CreateRenderPass(_device, in createInfo, null, out _renderPass), "create the render pass");
    }

    private void CreateGraphicsPipelines()
    {
        byte[] vertexShaderBytes = CompileShader(ShaderSources.VertexShader, ShaderKind.VertexShader, "Toroid.vert");
        byte[] fragmentShaderBytes = CompileShader(ShaderSources.FragmentShader, ShaderKind.FragmentShader, "Toroid.frag");
        byte[] hudVertexShaderBytes = CompileShader(ShaderSources.HudVertexShader, ShaderKind.VertexShader, "Hud.vert");
        byte[] hudFragmentShaderBytes = CompileShader(ShaderSources.HudFragmentShader, ShaderKind.FragmentShader, "Hud.frag");

        ShaderModule vertexShaderModule = CreateShaderModule(vertexShaderBytes);
        ShaderModule fragmentShaderModule = CreateShaderModule(fragmentShaderBytes);
        ShaderModule hudVertexShaderModule = CreateShaderModule(hudVertexShaderBytes);
        ShaderModule hudFragmentShaderModule = CreateShaderModule(hudFragmentShaderBytes);
        byte* entryPointPtr = (byte*)SilkMarshal.StringToPtr("main");

        try
        {
            PipelineShaderStageCreateInfo* shaderStages = stackalloc PipelineShaderStageCreateInfo[2];
            shaderStages[0] = new PipelineShaderStageCreateInfo
            {
                SType = StructureType.PipelineShaderStageCreateInfo,
                Stage = ShaderStageFlags.VertexBit,
                Module = vertexShaderModule,
                PName = entryPointPtr,
            };

            shaderStages[1] = new PipelineShaderStageCreateInfo
            {
                SType = StructureType.PipelineShaderStageCreateInfo,
                Stage = ShaderStageFlags.FragmentBit,
                Module = fragmentShaderModule,
                PName = entryPointPtr,
            };

            VertexInputBindingDescription bindingDescription = Vertex.GetBindingDescription();
            VertexInputAttributeDescription[] attributeDescriptions = Vertex.GetAttributeDescriptions();

            fixed (VertexInputAttributeDescription* attributeDescriptionsPtr = attributeDescriptions)
            {
                PipelineVertexInputStateCreateInfo vertexInputState = new()
                {
                    SType = StructureType.PipelineVertexInputStateCreateInfo,
                    VertexBindingDescriptionCount = 1,
                    PVertexBindingDescriptions = &bindingDescription,
                    VertexAttributeDescriptionCount = (uint)attributeDescriptions.Length,
                    PVertexAttributeDescriptions = attributeDescriptionsPtr,
                };

                PipelineInputAssemblyStateCreateInfo inputAssemblyState = new()
                {
                    SType = StructureType.PipelineInputAssemblyStateCreateInfo,
                    Topology = PrimitiveTopology.TriangleList,
                    PrimitiveRestartEnable = false,
                };

                Viewport viewport = new()
                {
                    X = 0f,
                    Y = 0f,
                    Width = _swapchainExtent.Width,
                    Height = _swapchainExtent.Height,
                    MinDepth = 0f,
                    MaxDepth = 1f,
                };

                Rect2D scissor = new()
                {
                    Offset = new Offset2D(0, 0),
                    Extent = _swapchainExtent,
                };

                PipelineViewportStateCreateInfo viewportState = new()
                {
                    SType = StructureType.PipelineViewportStateCreateInfo,
                    ViewportCount = 1,
                    PViewports = &viewport,
                    ScissorCount = 1,
                    PScissors = &scissor,
                };

                PipelineMultisampleStateCreateInfo multisampleState = new()
                {
                    SType = StructureType.PipelineMultisampleStateCreateInfo,
                    RasterizationSamples = SampleCountFlags.Count1Bit,
                    SampleShadingEnable = false,
                };

                PipelineDepthStencilStateCreateInfo depthStencilState = new()
                {
                    SType = StructureType.PipelineDepthStencilStateCreateInfo,
                    DepthTestEnable = true,
                    DepthWriteEnable = true,
                    DepthCompareOp = CompareOp.Less,
                    DepthBoundsTestEnable = false,
                    StencilTestEnable = false,
                };

                PipelineColorBlendAttachmentState colorBlendAttachment = new()
                {
                    ColorWriteMask = ColorComponentFlags.RBit | ColorComponentFlags.GBit | ColorComponentFlags.BBit | ColorComponentFlags.ABit,
                    BlendEnable = false,
                };

                PipelineColorBlendStateCreateInfo colorBlendState = new()
                {
                    SType = StructureType.PipelineColorBlendStateCreateInfo,
                    LogicOpEnable = false,
                    AttachmentCount = 1,
                    PAttachments = &colorBlendAttachment,
                };

                PushConstantRange pushConstantRange = new()
                {
                    StageFlags = ShaderStageFlags.VertexBit,
                    Offset = 0,
                    Size = ScenePushConstants.SizeInBytes,
                };

                PipelineLayoutCreateInfo pipelineLayoutCreateInfo = new()
                {
                    SType = StructureType.PipelineLayoutCreateInfo,
                    PushConstantRangeCount = 1,
                    PPushConstantRanges = &pushConstantRange,
                };

                Check(_vk.CreatePipelineLayout(_device, in pipelineLayoutCreateInfo, null, out _pipelineLayout), "create the pipeline layout");

                _solidGraphicsPipeline = CreateGraphicsPipeline(
                    shaderStages,
                    vertexInputState,
                    inputAssemblyState,
                    viewportState,
                    multisampleState,
                    depthStencilState,
                    colorBlendState,
                    PolygonMode.Fill);

                if (_supportsWireframe)
                {
                    _wireframeGraphicsPipeline = CreateGraphicsPipeline(
                        shaderStages,
                        vertexInputState,
                        inputAssemblyState,
                        viewportState,
                        multisampleState,
                        depthStencilState,
                        colorBlendState,
                        PolygonMode.Line);
                }

                CreateHudGraphicsPipeline(hudVertexShaderModule, hudFragmentShaderModule, entryPointPtr);
            }
        }
        finally
        {
            SilkMarshal.Free((nint)entryPointPtr);
            _vk.DestroyShaderModule(_device, vertexShaderModule, null);
            _vk.DestroyShaderModule(_device, fragmentShaderModule, null);
            _vk.DestroyShaderModule(_device, hudVertexShaderModule, null);
            _vk.DestroyShaderModule(_device, hudFragmentShaderModule, null);
        }
    }

    private void CreateHudGraphicsPipeline(ShaderModule vertexShaderModule, ShaderModule fragmentShaderModule, byte* entryPointPtr)
    {
        PipelineShaderStageCreateInfo* shaderStages = stackalloc PipelineShaderStageCreateInfo[2];
        shaderStages[0] = new PipelineShaderStageCreateInfo
        {
            SType = StructureType.PipelineShaderStageCreateInfo,
            Stage = ShaderStageFlags.VertexBit,
            Module = vertexShaderModule,
            PName = entryPointPtr,
        };

        shaderStages[1] = new PipelineShaderStageCreateInfo
        {
            SType = StructureType.PipelineShaderStageCreateInfo,
            Stage = ShaderStageFlags.FragmentBit,
            Module = fragmentShaderModule,
            PName = entryPointPtr,
        };

        VertexInputBindingDescription bindingDescription = HudVertex.GetBindingDescription();
        VertexInputAttributeDescription[] attributeDescriptions = HudVertex.GetAttributeDescriptions();

        fixed (VertexInputAttributeDescription* attributeDescriptionsPtr = attributeDescriptions)
        {
            PipelineVertexInputStateCreateInfo vertexInputState = new()
            {
                SType = StructureType.PipelineVertexInputStateCreateInfo,
                VertexBindingDescriptionCount = 1,
                PVertexBindingDescriptions = &bindingDescription,
                VertexAttributeDescriptionCount = (uint)attributeDescriptions.Length,
                PVertexAttributeDescriptions = attributeDescriptionsPtr,
            };

            PipelineInputAssemblyStateCreateInfo inputAssemblyState = new()
            {
                SType = StructureType.PipelineInputAssemblyStateCreateInfo,
                Topology = PrimitiveTopology.TriangleList,
                PrimitiveRestartEnable = false,
            };

            Viewport viewport = new()
            {
                X = 0f,
                Y = 0f,
                Width = _swapchainExtent.Width,
                Height = _swapchainExtent.Height,
                MinDepth = 0f,
                MaxDepth = 1f,
            };

            Rect2D scissor = new()
            {
                Offset = new Offset2D(0, 0),
                Extent = _swapchainExtent,
            };

            PipelineViewportStateCreateInfo viewportState = new()
            {
                SType = StructureType.PipelineViewportStateCreateInfo,
                ViewportCount = 1,
                PViewports = &viewport,
                ScissorCount = 1,
                PScissors = &scissor,
            };

            PipelineRasterizationStateCreateInfo rasterizationState = new()
            {
                SType = StructureType.PipelineRasterizationStateCreateInfo,
                DepthClampEnable = false,
                RasterizerDiscardEnable = false,
                PolygonMode = PolygonMode.Fill,
                LineWidth = 1f,
                CullMode = CullModeFlags.None,
                FrontFace = FrontFace.CounterClockwise,
                DepthBiasEnable = false,
            };

            PipelineMultisampleStateCreateInfo multisampleState = new()
            {
                SType = StructureType.PipelineMultisampleStateCreateInfo,
                RasterizationSamples = SampleCountFlags.Count1Bit,
                SampleShadingEnable = false,
            };

            PipelineDepthStencilStateCreateInfo depthStencilState = new()
            {
                SType = StructureType.PipelineDepthStencilStateCreateInfo,
                DepthTestEnable = false,
                DepthWriteEnable = false,
                DepthCompareOp = CompareOp.Always,
                DepthBoundsTestEnable = false,
                StencilTestEnable = false,
            };

            PipelineColorBlendAttachmentState colorBlendAttachment = new()
            {
                ColorWriteMask = ColorComponentFlags.RBit | ColorComponentFlags.GBit | ColorComponentFlags.BBit | ColorComponentFlags.ABit,
                BlendEnable = false,
            };

            PipelineColorBlendStateCreateInfo colorBlendState = new()
            {
                SType = StructureType.PipelineColorBlendStateCreateInfo,
                LogicOpEnable = false,
                AttachmentCount = 1,
                PAttachments = &colorBlendAttachment,
            };

            PipelineLayoutCreateInfo pipelineLayoutCreateInfo = new()
            {
                SType = StructureType.PipelineLayoutCreateInfo,
            };

            Check(_vk.CreatePipelineLayout(_device, in pipelineLayoutCreateInfo, null, out _hudPipelineLayout), "create the HUD pipeline layout");

            GraphicsPipelineCreateInfo createInfo = new()
            {
                SType = StructureType.GraphicsPipelineCreateInfo,
                StageCount = 2,
                PStages = shaderStages,
                PVertexInputState = &vertexInputState,
                PInputAssemblyState = &inputAssemblyState,
                PViewportState = &viewportState,
                PRasterizationState = &rasterizationState,
                PMultisampleState = &multisampleState,
                PDepthStencilState = &depthStencilState,
                PColorBlendState = &colorBlendState,
                Layout = _hudPipelineLayout,
                RenderPass = _renderPass,
                Subpass = 0,
            };

            Check(_vk.CreateGraphicsPipelines(_device, default, 1, in createInfo, null, out _hudGraphicsPipeline), "create the HUD graphics pipeline");
        }
    }

    private void CreateHudResources()
    {
        UpdateHudResources(0.5f);
    }

    private void UpdateHudResources(float sliderValue)
    {
        HudVertex[] vertices = BuildHudVertices(sliderValue);
        if (vertices.Length == 0)
        {
            _hudVertexCount = 0;
            return;
        }

        ulong bufferSize = (ulong)(HudVertex.SizeInBytes * vertices.Length);
        if (_hudVertexBuffer.Handle == 0 || _hudVertexCount != (uint)vertices.Length)
        {
            if (_hudVertexBuffer.Handle != 0)
            {
                _vk.DestroyBuffer(_device, _hudVertexBuffer, null);
                _vk.FreeMemory(_device, _hudVertexBufferMemory, null);
            }

            CreateBuffer(bufferSize, BufferUsageFlags.VertexBufferBit, MemoryPropertyFlags.HostVisibleBit | MemoryPropertyFlags.HostCoherentBit, out _hudVertexBuffer, out _hudVertexBufferMemory);
        }

        _hudVertexCount = (uint)vertices.Length;
        CopyToDeviceMemory(_hudVertexBufferMemory, bufferSize, vertices);
    }

    private HudVertex[] BuildHudVertices(float sliderValue)
    {
        string[] lines =
        [
            "F1 WIRE  R ROTATE  P SNAP",
            "LMB ORBIT  RMB DRAG  WHEEL ZOOM",
            "W S KEYS ZOOM  ESC QUIT",
        ];

        return HudTextBuilder.Build(lines, _swapchainExtent.Width, _swapchainExtent.Height, sliderValue);
    }

    private void CreateFramebuffers()
    {
        _framebuffers = new Framebuffer[_swapchainImageViews.Length];
        ImageView[] attachments = new ImageView[2];

        for (int index = 0; index < _swapchainImageViews.Length; index++)
        {
            attachments[0] = _swapchainImageViews[index];
            attachments[1] = _depthImageView;

            fixed (ImageView* attachmentsPtr = attachments)
            {
                FramebufferCreateInfo createInfo = new()
                {
                    SType = StructureType.FramebufferCreateInfo,
                    RenderPass = _renderPass,
                    AttachmentCount = 2,
                    PAttachments = attachmentsPtr,
                    Width = _swapchainExtent.Width,
                    Height = _swapchainExtent.Height,
                    Layers = 1,
                };

                Check(_vk.CreateFramebuffer(_device, in createInfo, null, out _framebuffers[index]), "create a framebuffer");
            }
        }
    }

    private void AllocateCommandBuffers()
    {
        if (_commandBuffers.Length > 0)
        {
            fixed (CommandBuffer* commandBuffersPtr = _commandBuffers)
            {
                _vk.FreeCommandBuffers(_device, _commandPool, (uint)_commandBuffers.Length, commandBuffersPtr);
            }
        }

        _commandBuffers = new CommandBuffer[_framebuffers.Length];
        fixed (CommandBuffer* commandBuffersPtr = _commandBuffers)
        {
            CommandBufferAllocateInfo allocateInfo = new()
            {
                SType = StructureType.CommandBufferAllocateInfo,
                CommandPool = _commandPool,
                Level = CommandBufferLevel.Primary,
                CommandBufferCount = (uint)_commandBuffers.Length,
            };

            Check(_vk.AllocateCommandBuffers(_device, in allocateInfo, commandBuffersPtr), "allocate the command buffers");
        }
    }

    private Pipeline CreateGraphicsPipeline(
        PipelineShaderStageCreateInfo* shaderStages,
        PipelineVertexInputStateCreateInfo vertexInputState,
        PipelineInputAssemblyStateCreateInfo inputAssemblyState,
        PipelineViewportStateCreateInfo viewportState,
        PipelineMultisampleStateCreateInfo multisampleState,
        PipelineDepthStencilStateCreateInfo depthStencilState,
        PipelineColorBlendStateCreateInfo colorBlendState,
        PolygonMode polygonMode)
    {
        PipelineRasterizationStateCreateInfo rasterizationState = new()
        {
            SType = StructureType.PipelineRasterizationStateCreateInfo,
            DepthClampEnable = false,
            RasterizerDiscardEnable = false,
            PolygonMode = polygonMode,
            LineWidth = 1f,
            CullMode = CullModeFlags.None,
            FrontFace = FrontFace.CounterClockwise,
            DepthBiasEnable = false,
        };

        GraphicsPipelineCreateInfo createInfo = new()
        {
            SType = StructureType.GraphicsPipelineCreateInfo,
            StageCount = 2,
            PStages = shaderStages,
            PVertexInputState = &vertexInputState,
            PInputAssemblyState = &inputAssemblyState,
            PViewportState = &viewportState,
            PRasterizationState = &rasterizationState,
            PMultisampleState = &multisampleState,
            PDepthStencilState = &depthStencilState,
            PColorBlendState = &colorBlendState,
            Layout = _pipelineLayout,
            RenderPass = _renderPass,
            Subpass = 0,
        };

        Check(_vk.CreateGraphicsPipelines(_device, default, 1, in createInfo, null, out Pipeline pipeline), "create the graphics pipeline");
        return pipeline;
    }

    private void RecordCommandBuffer(CommandBuffer commandBuffer, Framebuffer framebuffer, DemoRenderOptions renderOptions)
    {
        CommandBufferBeginInfo beginInfo = new()
        {
            SType = StructureType.CommandBufferBeginInfo,
        };

        Check(_vk.BeginCommandBuffer(commandBuffer, in beginInfo), "begin recording the command buffer");

        ClearValue* clearValues = stackalloc ClearValue[2];
        clearValues[0] = new ClearValue
        {
            Color = new ClearColorValue(0.05f, 0.06f, 0.10f, 1f),
        };
        clearValues[1] = new ClearValue
        {
            DepthStencil = new ClearDepthStencilValue(1f, 0),
        };

        RenderPassBeginInfo renderPassBeginInfo = new()
        {
            SType = StructureType.RenderPassBeginInfo,
            RenderPass = _renderPass,
            Framebuffer = framebuffer,
            RenderArea = new Rect2D(new Offset2D(0, 0), _swapchainExtent),
            ClearValueCount = 2,
            PClearValues = clearValues,
        };

        _vk.CmdBeginRenderPass(commandBuffer, in renderPassBeginInfo, SubpassContents.Inline);
        Pipeline pipeline = renderOptions.UseWireframe && _supportsWireframe
            ? _wireframeGraphicsPipeline
            : _solidGraphicsPipeline;

        _vk.CmdBindPipeline(commandBuffer, PipelineBindPoint.Graphics, pipeline);

        Buffer vertexBuffer = _vertexBuffer;
        ulong offset = 0;
        _vk.CmdBindVertexBuffers(commandBuffer, 0, 1, in vertexBuffer, in offset);
        _vk.CmdBindIndexBuffer(commandBuffer, _indexBuffer, 0, IndexType.Uint32);

        ScenePushConstants pushConstants = new()
        {
            Mvp = Matrix4x4.Transpose(renderOptions.ModelMatrix * renderOptions.ViewProjectionMatrix),
            Model = Matrix4x4.Transpose(renderOptions.ModelMatrix),
        };

        _vk.CmdPushConstants(commandBuffer, _pipelineLayout, ShaderStageFlags.VertexBit, 0, ScenePushConstants.SizeInBytes, &pushConstants);

        _vk.CmdDrawIndexed(commandBuffer, _indexCount, 1, 0, 0, 0);

        if (_hudVertexCount > 0)
        {
            Buffer hudVertexBuffer = _hudVertexBuffer;
            ulong hudOffset = 0;
            _vk.CmdBindPipeline(commandBuffer, PipelineBindPoint.Graphics, _hudGraphicsPipeline);
            _vk.CmdBindVertexBuffers(commandBuffer, 0, 1, in hudVertexBuffer, in hudOffset);
            _vk.CmdDraw(commandBuffer, _hudVertexCount, 1, 0, 0);
        }

        _vk.CmdEndRenderPass(commandBuffer);

        Check(_vk.EndCommandBuffer(commandBuffer), "finish recording the command buffer");
    }

    private void CreateSyncObjects()
    {
        SemaphoreCreateInfo semaphoreCreateInfo = new()
        {
            SType = StructureType.SemaphoreCreateInfo,
        };

        FenceCreateInfo fenceCreateInfo = new()
        {
            SType = StructureType.FenceCreateInfo,
            Flags = FenceCreateFlags.SignaledBit,
        };

        Check(_vk.CreateSemaphore(_device, in semaphoreCreateInfo, null, out _imageAvailableSemaphore), "create the image-available semaphore");
        Check(_vk.CreateSemaphore(_device, in semaphoreCreateInfo, null, out _renderFinishedSemaphore), "create the render-finished semaphore");
        Check(_vk.CreateFence(_device, in fenceCreateInfo, null, out _inFlightFence), "create the in-flight fence");
    }

    private void CreateBuffer(ulong size, BufferUsageFlags usage, MemoryPropertyFlags properties, out Buffer buffer, out DeviceMemory memory)
    {
        BufferCreateInfo createInfo = new()
        {
            SType = StructureType.BufferCreateInfo,
            Size = size,
            Usage = usage,
            SharingMode = SharingMode.Exclusive,
        };

        Check(_vk.CreateBuffer(_device, in createInfo, null, out buffer), "create a buffer");

        _vk.GetBufferMemoryRequirements(_device, buffer, out MemoryRequirements memoryRequirements);

        MemoryAllocateInfo allocationInfo = new()
        {
            SType = StructureType.MemoryAllocateInfo,
            AllocationSize = memoryRequirements.Size,
            MemoryTypeIndex = FindMemoryType(memoryRequirements.MemoryTypeBits, properties),
        };

        Check(_vk.AllocateMemory(_device, in allocationInfo, null, out memory), "allocate buffer memory");
        Check(_vk.BindBufferMemory(_device, buffer, memory, 0), "bind the buffer memory");
    }

    private void CreateImage(uint width, uint height, Format format, ImageTiling tiling, ImageUsageFlags usage, MemoryPropertyFlags properties, out Image image, out DeviceMemory memory)
    {
        ImageCreateInfo createInfo = new()
        {
            SType = StructureType.ImageCreateInfo,
            ImageType = ImageType.Type2D,
            Extent = new Extent3D(width, height, 1),
            MipLevels = 1,
            ArrayLayers = 1,
            Format = format,
            Tiling = tiling,
            InitialLayout = ImageLayout.Undefined,
            Usage = usage,
            Samples = SampleCountFlags.Count1Bit,
            SharingMode = SharingMode.Exclusive,
        };

        Check(_vk.CreateImage(_device, in createInfo, null, out image), "create an image");
        _vk.GetImageMemoryRequirements(_device, image, out MemoryRequirements memoryRequirements);

        MemoryAllocateInfo allocationInfo = new()
        {
            SType = StructureType.MemoryAllocateInfo,
            AllocationSize = memoryRequirements.Size,
            MemoryTypeIndex = FindMemoryType(memoryRequirements.MemoryTypeBits, properties),
        };

        Check(_vk.AllocateMemory(_device, in allocationInfo, null, out memory), "allocate image memory");
        Check(_vk.BindImageMemory(_device, image, memory, 0), "bind the image memory");
    }

    private ImageView CreateImageView(Image image, Format format, ImageAspectFlags aspectFlags)
    {
        ImageSubresourceRange subresourceRange = new()
        {
            AspectMask = aspectFlags,
            BaseMipLevel = 0,
            LevelCount = 1,
            BaseArrayLayer = 0,
            LayerCount = 1,
        };

        ImageViewCreateInfo createInfo = new()
        {
            SType = StructureType.ImageViewCreateInfo,
            Image = image,
            ViewType = ImageViewType.Type2D,
            Format = format,
            SubresourceRange = subresourceRange,
        };

        Check(_vk.CreateImageView(_device, in createInfo, null, out ImageView imageView), "create an image view");
        return imageView;
    }

    private uint FindMemoryType(uint typeFilter, MemoryPropertyFlags properties)
    {
        _vk.GetPhysicalDeviceMemoryProperties(_physicalDevice, out PhysicalDeviceMemoryProperties memoryProperties);
        for (uint index = 0; index < memoryProperties.MemoryTypeCount; index++)
        {
            bool isRequestedType = (typeFilter & (1u << (int)index)) != 0;
            bool hasProperties = (memoryProperties.MemoryTypes[(int)index].PropertyFlags & properties) == properties;
            if (isRequestedType && hasProperties)
            {
                return index;
            }
        }

        throw new InvalidOperationException("A suitable Vulkan memory type could not be found.");
    }

    private Format FindDepthFormat()
    {
        return FindSupportedFormat(
            [Format.D32Sfloat, Format.D32SfloatS8Uint, Format.D24UnormS8Uint],
            ImageTiling.Optimal,
            FormatFeatureFlags.DepthStencilAttachmentBit);
    }

    private Format FindSupportedFormat(Format[] candidates, ImageTiling tiling, FormatFeatureFlags features)
    {
        foreach (Format candidate in candidates)
        {
            _vk.GetPhysicalDeviceFormatProperties(_physicalDevice, candidate, out FormatProperties properties);
            if (tiling == ImageTiling.Linear && (properties.LinearTilingFeatures & features) == features)
            {
                return candidate;
            }

            if (tiling == ImageTiling.Optimal && (properties.OptimalTilingFeatures & features) == features)
            {
                return candidate;
            }
        }

        throw new InvalidOperationException("No compatible Vulkan format could be found for the requested usage.");
    }

    private ShaderModule CreateShaderModule(byte[] shaderBytes)
    {
        fixed (byte* shaderBytesPtr = shaderBytes)
        {
            ShaderModuleCreateInfo createInfo = new()
            {
                SType = StructureType.ShaderModuleCreateInfo,
                CodeSize = (nuint)shaderBytes.Length,
                PCode = (uint*)shaderBytesPtr,
            };

            Check(_vk.CreateShaderModule(_device, in createInfo, null, out ShaderModule shaderModule), "create a shader module");
            return shaderModule;
        }
    }

    private byte[] CompileShader(string source, ShaderKind shaderKind, string fileName)
    {
        Compiler* compiler = _shaderc.CompilerInitialize();
        CompileOptions* options = _shaderc.CompileOptionsInitialize();
        if (compiler is null || options is null)
        {
            throw new InvalidOperationException("Shaderc could not be initialized.");
        }

        try
        {
            CompilationResult* result = _shaderc.CompileIntoSpv(compiler, source, (nuint)source.Length, shaderKind, fileName, "main", options);
            if (result is null)
            {
                throw new InvalidOperationException($"Shader compilation returned no result for '{fileName}'.");
            }

            try
            {
                CompilationStatus status = _shaderc.ResultGetCompilationStatus(result);
                if (status != CompilationStatus.Success)
                {
                    string message = _shaderc.ResultGetErrorMessageS(result);
                    throw new InvalidOperationException($"Shader compilation failed for '{fileName}': {message}");
                }

                int length = checked((int)_shaderc.ResultGetLength(result));
                byte[] shaderBytes = new byte[length];
                Marshal.Copy((nint)_shaderc.ResultGetBytes(result), shaderBytes, 0, length);
                return shaderBytes;
            }
            finally
            {
                _shaderc.ResultRelease(result);
            }
        }
        finally
        {
            _shaderc.CompileOptionsRelease(options);
            _shaderc.CompilerRelease(compiler);
        }
    }

    private void CopyToDeviceMemory<T>(DeviceMemory deviceMemory, ulong size, T[] data)
        where T : unmanaged
    {
        void* mappedMemory = null;
        Check(_vk.MapMemory(_device, deviceMemory, 0, size, 0, &mappedMemory), "map device memory");

        try
        {
            fixed (T* sourcePtr = data)
            {
                System.Buffer.MemoryCopy(sourcePtr, mappedMemory, size, size);
            }
        }
        finally
        {
            _vk.UnmapMemory(_device, deviceMemory);
        }
    }

    private void EnsureInitialized()
    {
        if (!_isInitialized)
        {
            throw new InvalidOperationException("The Vulkan renderer has not been initialized.");
        }
    }

    private T LoadInstanceProc<T>(string functionName)
        where T : Delegate
    {
        byte* namePtr = (byte*)SilkMarshal.StringToPtr(functionName);
        try
        {
            nint procAddress = _vk.GetInstanceProcAddr(_instance, namePtr);
            if (procAddress == 0)
            {
                throw new InvalidOperationException($"The Vulkan instance procedure '{functionName}' is not available.");
            }

            return Marshal.GetDelegateForFunctionPointer<T>(procAddress);
        }
        finally
        {
            SilkMarshal.Free((nint)namePtr);
        }
    }

    private T LoadDeviceProc<T>(string functionName)
        where T : Delegate
    {
        byte* namePtr = (byte*)SilkMarshal.StringToPtr(functionName);
        try
        {
            nint procAddress = _vk.GetDeviceProcAddr(_device, namePtr);
            if (procAddress == 0)
            {
                throw new InvalidOperationException($"The Vulkan device procedure '{functionName}' is not available.");
            }

            return Marshal.GetDelegateForFunctionPointer<T>(procAddress);
        }
        finally
        {
            SilkMarshal.Free((nint)namePtr);
        }
    }

    private static void Check(Result result, string operation)
    {
        if (result != Result.Success)
        {
            throw new InvalidOperationException($"Vulkan failed to {operation}. Result: {result}.");
        }
    }

    private readonly record struct SwapchainSupportDetails(
        SurfaceCapabilitiesKHR Capabilities,
        SurfaceFormatKHR[] Formats,
        PresentModeKHR[] PresentModes);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate Result VkGetPhysicalDeviceSurfaceSupportKhr(PhysicalDevice physicalDevice, uint queueFamilyIndex, SurfaceKHR surface, out uint supported);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate Result VkGetPhysicalDeviceSurfaceCapabilitiesKhr(PhysicalDevice physicalDevice, SurfaceKHR surface, out SurfaceCapabilitiesKHR capabilities);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate Result VkGetPhysicalDeviceSurfaceFormatsKhr(PhysicalDevice physicalDevice, SurfaceKHR surface, uint* formatCount, SurfaceFormatKHR* formats);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate Result VkGetPhysicalDeviceSurfacePresentModesKhr(PhysicalDevice physicalDevice, SurfaceKHR surface, uint* presentModeCount, PresentModeKHR* presentModes);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate void VkDestroySurfaceKhr(Instance instance, SurfaceKHR surface, AllocationCallbacks* allocator);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate Result VkCreateSwapchainKhr(Device device, in SwapchainCreateInfoKHR createInfo, AllocationCallbacks* allocator, out SwapchainKHR swapchain);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate void VkDestroySwapchainKhr(Device device, SwapchainKHR swapchain, AllocationCallbacks* allocator);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate Result VkGetSwapchainImagesKhr(Device device, SwapchainKHR swapchain, uint* imageCount, Image* images);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate Result VkAcquireNextImageKhr(Device device, SwapchainKHR swapchain, ulong timeout, Semaphore semaphore, Fence fence, out uint imageIndex);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate Result VkQueuePresentKhr(Queue queue, in PresentInfoKHR presentInfo);
}

















