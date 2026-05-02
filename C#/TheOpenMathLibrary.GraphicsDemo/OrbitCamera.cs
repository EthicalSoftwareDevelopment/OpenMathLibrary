using System.Numerics;

namespace TheOpenMathLibrary.GraphicsDemo;

/// <summary>
/// Maintains a simple orbit camera for inspecting the toroid.
/// </summary>
public sealed class OrbitCamera
{
    private const float MinDistance = 1.5f;
    private const float MaxDistance = 12f;
    private const float MinPitch = -1.2f;
    private const float MaxPitch = 1.2f;
    private const float MouseOrbitSensitivity = 0.0125f;
    private const float MouseZoomSensitivity = 0.025f;
    private const float ScrollZoomSensitivity = 0.45f;

    private float _yaw;
    private float _pitch;
    private float _distance;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrbitCamera"/> class.
    /// </summary>
    public OrbitCamera(float yaw, float pitch, float distance)
    {
        _yaw = yaw;
        _pitch = pitch;
        _distance = distance;
    }

    /// <summary>
    /// Creates a default orbit camera suited for viewing the toroid.
    /// </summary>
    public static OrbitCamera CreateDefault()
    {
        return new OrbitCamera(0.7f, 0.35f, 4.5f);
    }

    /// <summary>
    /// Updates the camera using the current user input and elapsed time.
    /// </summary>
    public void Update(OrbitCameraInput input, float deltaSeconds)
    {
        float rotationSpeed = 1.35f * deltaSeconds;
        float zoomSpeed = 2.25f * deltaSeconds;

        if (input.RotateLeft)
        {
            _yaw -= rotationSpeed;
        }

        if (input.RotateRight)
        {
            _yaw += rotationSpeed;
        }

        if (input.RotateUp)
        {
            _pitch = Math.Clamp(_pitch + rotationSpeed, MinPitch, MaxPitch);
        }

        if (input.RotateDown)
        {
            _pitch = Math.Clamp(_pitch - rotationSpeed, MinPitch, MaxPitch);
        }

        if (input.ZoomIn)
        {
            _distance = Math.Clamp(_distance - zoomSpeed, MinDistance, MaxDistance);
        }

        if (input.ZoomOut)
        {
            _distance = Math.Clamp(_distance + zoomSpeed, MinDistance, MaxDistance);
        }

        if (input.IsMouseOrbiting)
        {
            _yaw += input.MouseDeltaX * MouseOrbitSensitivity;
            _pitch = Math.Clamp(_pitch - input.MouseDeltaY * MouseOrbitSensitivity, MinPitch, MaxPitch);
        }

        if (input.IsMouseZooming)
        {
            _distance = Math.Clamp(_distance + input.MouseDeltaY * MouseZoomSensitivity, MinDistance, MaxDistance);
        }

        if (input.ScrollDeltaY != 0f)
        {
            _distance = Math.Clamp(_distance - input.ScrollDeltaY * ScrollZoomSensitivity, MinDistance, MaxDistance);
        }
    }

    /// <summary>
    /// Builds a view-projection matrix for the current camera.
    /// </summary>
    public Matrix4x4 CreateViewProjectionMatrix(float aspectRatio)
    {
        Vector3 eye = new(
            _distance * MathF.Cos(_pitch) * MathF.Cos(_yaw),
            _distance * MathF.Sin(_pitch),
            _distance * MathF.Cos(_pitch) * MathF.Sin(_yaw));

        Matrix4x4 view = Matrix4x4.CreateLookAt(eye, Vector3.Zero, Vector3.UnitY);
        Matrix4x4 projection = Matrix4x4.CreatePerspectiveFieldOfView(MathF.PI / 4f, aspectRatio, 0.1f, 100f);
        projection.M22 *= -1f;
        return view * projection;
    }

    /// <summary>
    /// Gets the current camera distance.
    /// </summary>
    public float Distance => _distance;

    /// <summary>
    /// Gets the current yaw angle.
    /// </summary>
    public float Yaw => _yaw;

    /// <summary>
    /// Gets the current pitch angle.
    /// </summary>
    public float Pitch => _pitch;
}


