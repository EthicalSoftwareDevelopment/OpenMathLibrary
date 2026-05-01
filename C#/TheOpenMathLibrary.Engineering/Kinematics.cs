using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheOpenMathLibrary.Engineering
{
    /// <summary>
    /// Provides basic kinematics formulas for translational and angular motion.
    /// </summary>
    public class Kinematics
    {
        /// <summary>
        /// Calculates velocity from displacement and elapsed time.
        /// </summary>
        /// <param name="displacement">The displacement.</param>
        /// <param name="time">The elapsed time.</param>
        /// <returns>The velocity.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="time"/> is zero.</exception>
        public static double Velocity(double displacement, double time)
        {
            EnsureNonZero(time, nameof(time));
            return displacement / time;
        }

        /// <summary>
        /// Calculates acceleration from velocity change and elapsed time.
        /// </summary>
        /// <param name="velocity">The change in velocity.</param>
        /// <param name="time">The elapsed time.</param>
        /// <returns>The acceleration.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="time"/> is zero.</exception>
        public static double Acceleration(double velocity, double time)
        {
            EnsureNonZero(time, nameof(time));
            return velocity / time;
        }

        /// <summary>
        /// Calculates jerk from acceleration change and elapsed time.
        /// </summary>
        /// <param name="acceleration">The change in acceleration.</param>
        /// <param name="time">The elapsed time.</param>
        /// <returns>The jerk.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="time"/> is zero.</exception>
        public static double Jerk(double acceleration, double time)
        {
            EnsureNonZero(time, nameof(time));
            return acceleration / time;
        }

        /// <summary>
        /// Calculates jounce from jerk change and elapsed time.
        /// </summary>
        /// <param name="jerk">The change in jerk.</param>
        /// <param name="time">The elapsed time.</param>
        /// <returns>The jounce.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="time"/> is zero.</exception>
        public static double Jounce(double jerk, double time)
        {
            EnsureNonZero(time, nameof(time));
            return jerk / time;
        }

        /// <summary>
        /// Calculates angular acceleration from angular velocity change and elapsed time.
        /// </summary>
        /// <param name="angularVelocity">The change in angular velocity.</param>
        /// <param name="time">The elapsed time.</param>
        /// <returns>The angular acceleration.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="time"/> is zero.</exception>
        public static double AngularAcceleration(double angularVelocity, double time)
        {
            EnsureNonZero(time, nameof(time));
            return angularVelocity / time;
        }

        /// <summary>
        /// Calculates angular jerk from angular acceleration change and elapsed time.
        /// </summary>
        /// <param name="angularAcceleration">The change in angular acceleration.</param>
        /// <param name="time">The elapsed time.</param>
        /// <returns>The angular jerk.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="time"/> is zero.</exception>
        public static double AngularJerk(double angularAcceleration, double time)
        {
            EnsureNonZero(time, nameof(time));
            return angularAcceleration / time;
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
