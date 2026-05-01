using System;

namespace TheOpenMathLibrary.Engineering
{
    /// <summary>
    /// Provides helper formulas for derived dynamic quantities.
    /// </summary>
    public class DerivedDynamicQuantities
    {
        /// <summary>
        /// Calculates linear momentum.
        /// </summary>
        /// <param name="mass">The mass of the object.</param>
        /// <param name="velocity">The velocity of the object.</param>
        /// <returns>The momentum.</returns>
        public static double Momentum(double mass, double velocity)
        {
            return mass * velocity;
        }

        /// <summary>
        /// Calculates force from mass and acceleration.
        /// </summary>
        /// <param name="mass">The mass of the object.</param>
        /// <param name="acceleration">The acceleration of the object.</param>
        /// <returns>The force.</returns>
        public static double Force(double mass, double acceleration)
        {
            return mass * acceleration;
        }

        /// <summary>
        /// Calculates impulse from force and time.
        /// </summary>
        /// <param name="force">The applied force.</param>
        /// <param name="time">The duration of the force application.</param>
        /// <returns>The impulse.</returns>
        public static double Impulse(double force, double time)
        {
            return force * time;
        }

        /// <summary>
        /// Calculates angular momentum for a point mass moving tangentially.
        /// </summary>
        /// <param name="mass">The mass of the object.</param>
        /// <param name="velocity">The tangential velocity.</param>
        /// <param name="radius">The radial distance from the axis.</param>
        /// <returns>The angular momentum.</returns>
        public static double AngularMomentum(double mass, double velocity, double radius)
        {
            return mass * velocity * radius;
        }

        /// <summary>
        /// Calculates torque from force and radius.
        /// </summary>
        /// <param name="force">The applied force.</param>
        /// <param name="radius">The lever arm length.</param>
        /// <returns>The torque.</returns>
        public static double Torque(double force, double radius)
        {
            return force * radius;
        }

        /// <summary>
        /// Calculates angular impulse from torque and time.
        /// </summary>
        /// <param name="torque">The torque applied.</param>
        /// <param name="time">The duration of the torque application.</param>
        /// <returns>The angular impulse.</returns>
        public static double AngularImpulse(double torque, double time)
        {
            return torque * time;
        }
    }
}
