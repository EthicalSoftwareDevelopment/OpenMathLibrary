namespace TheOpenMathLibrary.GraphicsDemo;

/// <summary>
/// Builds output file paths for exported demo snapshots.
/// </summary>
public static class SnapshotPathBuilder
{
    /// <summary>
    /// Creates a timestamped snapshot path under the given root directory.
    /// </summary>
    public static string Create(string rootDirectory, DateTimeOffset timestamp)
    {
        if (string.IsNullOrWhiteSpace(rootDirectory))
        {
            throw new ArgumentException("The root directory must not be empty.", nameof(rootDirectory));
        }

        string directory = Path.Combine(rootDirectory, "Screenshots");
        Directory.CreateDirectory(directory);
        string fileName = $"toroid-{timestamp:yyyyMMdd-HHmmss-fff}.ppm";
        return Path.Combine(directory, fileName);
    }
}

