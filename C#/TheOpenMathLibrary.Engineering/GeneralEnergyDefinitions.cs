using System;

namespace TheOpenMathLibrary.Engineering
{
    /// <summary>
    /// Provides general mechanical energy and work formulas.
    /// </summary>
    public class GeneralEnergyDefinitions
    {
        /// <summary>
        /// Calculates mechanical work.
        /// </summary>
        /// <param name="force">The applied force magnitude.</param>
        /// <param name="displacement">The displacement magnitude.</param>
        /// <param name="angle">The angle between the force and displacement vectors, in radians.</param>
        /// <returns>The mechanical work.</returns>
        public static double MechanicalWork(double force, double displacement, double angle)
        {
            return force * displacement * Math.Cos(angle);
        }

        /// <summary>
        /// Calculates work done on a mechanical system.
        /// </summary>
        /// <param name="force">The applied force magnitude.</param>
        /// <param name="displacement">The displacement magnitude.</param>
        /// <param name="angle">The angle between the force and displacement vectors, in radians.</param>
        /// <returns>The work done on the system.</returns>
        public static double WorkDoneOnMechanicalSystem(double force, double displacement, double angle)
        {
            return force * displacement * Math.Cos(angle);
        }

        /// <summary>
        /// Calculates gravitational potential energy.
        /// </summary>
        /// <param name="mass">The mass of the object.</param>
        /// <param name="height">The height relative to a reference point.</param>
        /// <param name="gravity">The gravitational acceleration.</param>
        /// <returns>The potential energy.</returns>
        public static double PotentialEnergy(double mass, double height, double gravity)
        {
            return mass * height * gravity;
        }

        /// <summary>
        /// Calculates total mechanical energy from kinetic and potential energy components.
        /// </summary>
        /// <param name="kineticEnergy">The kinetic energy component.</param>
        /// <param name="potentialEnergy">The potential energy component.</param>
        /// <returns>The total mechanical energy.</returns>
        public static double MechanicalEnergy(double kineticEnergy, double potentialEnergy)
        {
            return kineticEnergy + potentialEnergy;
        }

        /// <summary>
        /// Calculates mechanical power from force and velocity.
        /// </summary>
        /// <param name="force">The applied force.</param>
        /// <param name="velocity">The velocity.</param>
        /// <returns>The mechanical power.</returns>
        public static double MechanicalPower(double force, double velocity)
        {
            return force * velocity;
        }
    }
}
