namespace TheOpenMathLibrary.GraphicsDemo.Tests;

[TestClass]
public class OrbitCameraTests
{
    [TestMethod]
    public void Update_WithMouseOrbit_AdjustsYawAndPitch()
    {
        OrbitCamera camera = OrbitCamera.CreateDefault();
        float initialYaw = camera.Yaw;
        float initialPitch = camera.Pitch;

        camera.Update(new OrbitCameraInput(
            RotateLeft: false,
            RotateRight: false,
            RotateUp: false,
            RotateDown: false,
            ZoomIn: false,
            ZoomOut: false,
            IsMouseOrbiting: true,
            IsMouseZooming: false,
            MouseDeltaX: 20f,
            MouseDeltaY: -10f,
            ScrollDeltaY: 0f),
            0f);

        Assert.IsTrue(camera.Yaw > initialYaw);
        Assert.IsTrue(camera.Pitch > initialPitch);
    }

    [TestMethod]
    public void Update_WithMouseZoom_AdjustsDistance()
    {
        OrbitCamera camera = OrbitCamera.CreateDefault();
        float initialDistance = camera.Distance;

        camera.Update(new OrbitCameraInput(
            RotateLeft: false,
            RotateRight: false,
            RotateUp: false,
            RotateDown: false,
            ZoomIn: false,
            ZoomOut: false,
            IsMouseOrbiting: false,
            IsMouseZooming: true,
            MouseDeltaX: 0f,
            MouseDeltaY: -8f,
            ScrollDeltaY: 0f),
            0f);

        Assert.IsTrue(camera.Distance < initialDistance);
    }

    [TestMethod]
    public void Update_WithScrollZoom_AdjustsDistance()
    {
        OrbitCamera camera = OrbitCamera.CreateDefault();
        float initialDistance = camera.Distance;

        camera.Update(new OrbitCameraInput(
            RotateLeft: false,
            RotateRight: false,
            RotateUp: false,
            RotateDown: false,
            ZoomIn: false,
            ZoomOut: false,
            IsMouseOrbiting: false,
            IsMouseZooming: false,
            MouseDeltaX: 0f,
            MouseDeltaY: 0f,
            ScrollDeltaY: 2f),
            0f);

        Assert.IsTrue(camera.Distance < initialDistance);
    }
}

