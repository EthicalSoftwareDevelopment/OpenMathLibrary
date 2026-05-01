namespace TheOpenMathLibrary.GeneralMathematics
{
    /// <summary>
    /// Provides basic geometry formulas.
    /// </summary>
    public class GeometryFunctions
    {
        /// <summary>
        /// Calculates the area of a triangle.
        /// </summary>
        public static double TriangleArea(double baseLength, double height)
        {
            EnsureNonNegative(baseLength, nameof(baseLength));
            EnsureNonNegative(height, nameof(height));
            return 0.5d * baseLength * height;
        }

        /// <summary>
        /// Calculates the volume of a cylinder.
        /// </summary>
        public static double CylinderVolume(double radius, double height)
        {
            EnsureNonNegative(radius, nameof(radius));
            EnsureNonNegative(height, nameof(height));
            return Math.PI * radius * radius * height;
        }

        /// <summary>
        /// Calculates the volume of a sphere.
        /// </summary>
        public static double SphereVolume(double radius)
        {
            EnsureNonNegative(radius, nameof(radius));
            return 4d / 3d * Math.PI * radius * radius * radius;
        }

        /// <summary>
        /// Calculates the volume of a rectangular prism.
        /// </summary>
        public static double RectangularPrismVolume(double length, double width, double height)
        {
            EnsureNonNegative(length, nameof(length));
            EnsureNonNegative(width, nameof(width));
            EnsureNonNegative(height, nameof(height));
            return length * width * height;
        }

        private static void EnsureNonNegative(double value, string parameterName)
        {
            if (value < 0d)
            {
                throw new ArgumentOutOfRangeException(parameterName, "The value must be non-negative.");
            }
        }
    }
}
