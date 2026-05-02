namespace TheOpenMathLibrary.GraphicsDemo;

internal static class Program
{
    private static void Main()
    {
        try
        {
            GraphicsDemoApp app = new();
            app.Run();
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine("The Vulkan toroid demo failed to start.");
            Console.Error.WriteLine(exception);
            Environment.ExitCode = 1;
        }
    }
}


