namespace TheOpenMathLibrary.GeneralMathematics
{
    /// <summary>
    /// Provides trigonometric helper functions derived from the standard sine and cosine relationships.
    /// </summary>
    public class TrigonometryLibrary
    {
        /// <summary>
        /// Calculates the sine of an angle in radians.
        /// </summary>
        public static double Sine(double x)
        {
            return Math.Sin(x);
        }

        /// <summary>
        /// Calculates the cosine of an angle in radians.
        /// </summary>
        public static double Cosine(double x)
        {
            return Math.Cos(x);
        }

        /// <summary>
        /// Calculates the tangent of an angle in radians.
        /// </summary>
        public static double Tangent(double x)
        {
            return Math.Tan(x);
        }

        /// <summary>
        /// Calculates the cotangent of an angle in radians.
        /// </summary>
        public static double Cotangent(double x)
        {
            var tangent = Math.Tan(x);
            EnsureNonZero(tangent, nameof(x), "Cotangent is undefined where tangent is zero.");
            return 1d / tangent;
        }

        /// <summary>
        /// Calculates the secant of an angle in radians.
        /// </summary>
        public static double Secant(double x)
        {
            var cosine = Math.Cos(x);
            EnsureNonZero(cosine, nameof(x), "Secant is undefined where cosine is zero.");
            return 1d / cosine;
        }

        /// <summary>
        /// Calculates the cosecant of an angle in radians.
        /// </summary>
        public static double Cosecant(double x)
        {
            var sine = Math.Sin(x);
            EnsureNonZero(sine, nameof(x), "Cosecant is undefined where sine is zero.");
            return 1d / sine;
        }

        /// <summary>
        /// Calculates the exsecant of an angle in radians.
        /// </summary>
        public static double Exsecant(double x)
        {
            return Secant(x) - 1d;
        }

        /// <summary>
        /// Calculates the excosecant of an angle in radians.
        /// </summary>
        public static double Excosecant(double x)
        {
            return Cosecant(x) - 1d;
        }

        /// <summary>
        /// Calculates the versine of an angle in radians.
        /// </summary>
        public static double Versine(double x)
        {
            return 1d - Math.Cos(x);
        }

        /// <summary>
        /// Calculates the coversine of an angle in radians.
        /// </summary>
        public static double Coversine(double x)
        {
            return 1d - Math.Sin(x);
        }

        /// <summary>
        /// Calculates the vercosine of an angle in radians.
        /// </summary>
        public static double Vercosine(double x)
        {
            return 1d + Math.Cos(x);
        }

        /// <summary>
        /// Calculates the covercosine of an angle in radians.
        /// </summary>
        public static double Covercosine(double x)
        {
            return 1d + Math.Sin(x);
        }

        /// <summary>
        /// Calculates the haversine of an angle in radians.
        /// </summary>
        public static double Haversine(double x)
        {
            return 0.5d * Versine(x);
        }

        /// <summary>
        /// Calculates the hacoversine of an angle in radians.
        /// </summary>
        public static double Hacoversine(double x)
        {
            return 0.5d * Coversine(x);
        }

        /// <summary>
        /// Calculates the havercosine of an angle in radians.
        /// </summary>
        public static double Havercosine(double x)
        {
            return 0.5d * Vercosine(x);
        }

        /// <summary>
        /// Calculates the hacovercosine of an angle in radians.
        /// </summary>
        public static double Hacovercosine(double x)
        {
            return 0.5d * Covercosine(x);
        }

        private static void EnsureNonZero(double value, string parameterName, string message)
        {
            if (Math.Abs(value) <= 1e-12)
            {
                throw new ArgumentOutOfRangeException(parameterName, message);
            }
        }
    }
}
