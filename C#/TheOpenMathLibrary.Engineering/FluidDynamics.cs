namespace TheOpenMathLibrary.Engineering
{
    /// <summary>
    /// Provides helper formulas for introductory fluid-dynamics calculations.
    /// </summary>
    public class FluidDynamics
    {
        /// <summary>
        /// Calculates average flow velocity in a circular pipe from volumetric flow rate.
        /// </summary>
        /// <param name="flowRate">The volumetric flow rate.</param>
        /// <param name="pipeDiameter">The pipe diameter.</param>
        /// <returns>The average flow velocity.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="pipeDiameter"/> is zero.</exception>
        public static double FlowVelocity(double flowRate, double pipeDiameter)
        {
            EnsureNonZero(pipeDiameter, nameof(pipeDiameter));
            return 4d * flowRate / (Math.PI * pipeDiameter * pipeDiameter);
        }

        /// <summary>
        /// Calculates a velocity pseudovector magnitude from linear velocity and radius.
        /// </summary>
        /// <param name="velocity">The velocity.</param>
        /// <param name="radius">The radius.</param>
        /// <returns>The pseudovector magnitude.</returns>
        public static double VelocityPseudovector(double velocity, double radius)
        {
            return velocity * radius;
        }

        /// <summary>
        /// Calculates volume flux through a circular cross section.
        /// </summary>
        /// <param name="flowRate">The volumetric flow rate.</param>
        /// <param name="radius">The radius of the cross section.</param>
        /// <returns>The volume flux.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="radius"/> is zero.</exception>
        public static double VolumeFlux(double flowRate, double radius)
        {
            EnsureNonZero(radius, nameof(radius));
            return flowRate / (Math.PI * radius * radius);
        }

        /// <summary>
        /// Calculates mass current per unit volume.
        /// </summary>
        /// <param name="massCurrent">The mass current.</param>
        /// <param name="volume">The volume.</param>
        /// <returns>The mass current per unit volume.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="volume"/> is zero.</exception>
        public static double MassCurrentPerVolume(double massCurrent, double volume)
        {
            EnsureNonZero(volume, nameof(volume));
            return massCurrent / volume;
        }

        /// <summary>
        /// Calculates mass flow rate from density and volume flux.
        /// </summary>
        /// <param name="density">The fluid density.</param>
        /// <param name="volumeFlux">The volume flux.</param>
        /// <returns>The mass flow rate.</returns>
        public static double MassFlowRate(double density, double volumeFlux)
        {
            return density * volumeFlux;
        }

        /// <summary>
        /// Calculates mass current density.
        /// </summary>
        /// <param name="density">The fluid density.</param>
        /// <param name="velocity">The fluid velocity.</param>
        /// <returns>The mass current density.</returns>
        public static double MassCurrentDensity(double density, double velocity)
        {
            return density * velocity;
        }

        /// <summary>
        /// Calculates momentum current density.
        /// </summary>
        /// <param name="density">The fluid density.</param>
        /// <param name="velocity">The fluid velocity.</param>
        /// <returns>The momentum current density.</returns>
        public static double MomentumCurrentDensity(double density, double velocity)
        {
            return density * velocity * velocity;
        }

        /// <summary>
        /// Calculates pressure gradient magnitude over a distance.
        /// </summary>
        /// <param name="pressure">The pressure difference.</param>
        /// <param name="distance">The distance over which the pressure changes.</param>
        /// <returns>The pressure gradient.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="distance"/> is zero.</exception>
        public static double PressureGradient(double pressure, double distance)
        {
            EnsureNonZero(distance, nameof(distance));
            return pressure / distance;
        }

        /// <summary>
        /// Calculates buoyancy force.
        /// </summary>
        /// <param name="density">The fluid density.</param>
        /// <param name="volume">The displaced volume.</param>
        /// <param name="gravity">The gravitational acceleration.</param>
        /// <returns>The buoyancy force.</returns>
        public static double BuoyancyForce(double density, double volume, double gravity)
        {
            return density * volume * gravity;
        }

        /// <summary>
        /// Evaluates a Bernoulli-style energy-per-volume expression.
        /// </summary>
        /// <param name="pressure">The static pressure term.</param>
        /// <param name="density">The fluid density.</param>
        /// <param name="velocity">The fluid velocity.</param>
        /// <param name="height">The height term.</param>
        /// <param name="gravity">The gravitational acceleration.</param>
        /// <returns>The Bernoulli expression value.</returns>
        public static double BernoullisEquation(double pressure, double density, double velocity, double height, double gravity)
        {
            return pressure + density * velocity * velocity + density * gravity * height;
        }

        /// <summary>
        /// Evaluates an Euler-equation-style energy expression.
        /// </summary>
        /// <param name="pressure">The pressure term.</param>
        /// <param name="density">The fluid density.</param>
        /// <param name="velocity">The fluid velocity.</param>
        /// <param name="height">The height term.</param>
        /// <param name="gravity">The gravitational acceleration.</param>
        /// <returns>The expression value.</returns>
        public static double EulersEquations(double pressure, double density, double velocity, double height, double gravity)
        {
            return pressure + density * velocity * velocity + density * gravity * height;
        }

        /// <summary>
        /// Calculates convective acceleration from velocity and acceleration scale terms.
        /// </summary>
        /// <param name="velocity">The fluid velocity.</param>
        /// <param name="acceleration">The acceleration scale.</param>
        /// <returns>The convective acceleration estimate.</returns>
        public static double ConvectiveAcceleration(double velocity, double acceleration)
        {
            return velocity * acceleration;
        }

        /// <summary>
        /// Evaluates a Navier-Stokes-style scalar expression.
        /// </summary>
        /// <param name="pressure">The pressure term.</param>
        /// <param name="density">The fluid density.</param>
        /// <param name="velocity">The fluid velocity.</param>
        /// <param name="height">The height term.</param>
        /// <param name="gravity">The gravitational acceleration.</param>
        /// <returns>The expression value.</returns>
        public static double NavierStokesEquations(double pressure, double density, double velocity, double height, double gravity)
        {
            return pressure + density * velocity * velocity + density * gravity * height;
        }

        private static void EnsureNonZero(double value, string parameterName)
        {
            if (value == 0d)
            {
                throw new ArgumentOutOfRangeException(parameterName, "The value must not be zero.");
            }
        }
    }
}
