namespace TheOpenMathLibrary.Engineering
{
    /// <summary>
    /// Provides helper formulas for introductory optics and photonics calculations.
    /// </summary>
    public class OpticsAndPhotonics
    {
        /// <summary>
        /// Calculates linear magnification.
        /// </summary>
        public static double Magnification(double imageDistance, double objectDistance)
        {
            EnsureNonZero(objectDistance, nameof(objectDistance));
            return imageDistance / objectDistance;
        }

        /// <summary>
        /// Calculates image height from magnification and object height.
        /// </summary>
        public static double ImageHeight(double magnification, double objectHeight)
        {
            return magnification * objectHeight;
        }

        /// <summary>
        /// Calculates image distance from focal length and object distance using the thin-lens relation.
        /// </summary>
        public static double ImageDistance(double focalLength, double objectDistance)
        {
            var denominator = objectDistance - focalLength;
            EnsureNonZero(denominator, nameof(objectDistance));
            return focalLength * objectDistance / denominator;
        }

        /// <summary>
        /// Calculates the magnitude of the Poynting vector in a simplified scalar form.
        /// </summary>
        public static double PoyntingVector(double electricField, double magneticField)
        {
            return electricField * magneticField;
        }

        /// <summary>
        /// Calculates Poynting flux in a simplified scalar form.
        /// </summary>
        public static double PoyntingFlux(double electricField, double magneticField)
        {
            return electricField * magneticField;
        }

        /// <summary>
        /// Calculates the root-mean-square electric field magnitude.
        /// </summary>
        public static double RmSElectricField(double electricField)
        {
            return electricField / Math.Sqrt(2d);
        }

        /// <summary>
        /// Calculates the root-mean-square electric field magnitude.
        /// </summary>
        /// <param name="electricField">The electric field magnitude.</param>
        /// <returns>The RMS electric field magnitude.</returns>
        public static double RMSElectricField(double electricField)
        {
            return RmSElectricField(electricField);
        }

        /// <summary>
        /// Calculates radiation momentum from intensity and speed of light.
        /// </summary>
        public static double RadiationMomentum(double intensity, double speedOfLight)
        {
            EnsureNonZero(speedOfLight, nameof(speedOfLight));
            return intensity / speedOfLight;
        }

        /// <summary>
        /// Calculates radiant intensity.
        /// </summary>
        public static double RadiantIntensity(double power, double solidAngle)
        {
            EnsureNonZero(solidAngle, nameof(solidAngle));
            return power / solidAngle;
        }

        /// <summary>
        /// Calculates radiosity using the Stefan-Boltzmann relation.
        /// </summary>
        public static double Radiosity(double emissivity, double stefanBoltzmannConstant, double temperature)
        {
            return emissivity * stefanBoltzmannConstant * Math.Pow(temperature, 4d);
        }

        /// <summary>
        /// Calculates spectral radiance from radiance per wavelength interval.
        /// </summary>
        public static double SpectralRadiance(double radiance, double wavelength)
        {
            EnsureNonZero(wavelength, nameof(wavelength));
            return radiance / wavelength;
        }

        /// <summary>
        /// Calculates spectral irradiance from irradiance per wavelength interval.
        /// </summary>
        public static double SpectralIrradiance(double irradiance, double wavelength)
        {
            EnsureNonZero(wavelength, nameof(wavelength));
            return irradiance / wavelength;
        }

        /// <summary>
        /// Calculates electromagnetic energy density in a simplified scalar form.
        /// </summary>
        public static double EnergyDensity(double electricField, double magneticField)
        {
            return 0.5d * (electricField * electricField + magneticField * magneticField);
        }

        /// <summary>
        /// Calculates linear momentum from mass and speed.
        /// </summary>
        public static double KineticMomentum(double speedOfLight, double mass)
        {
            return speedOfLight * mass;
        }

        /// <summary>
        /// Calculates a simplified Doppler-shifted frequency for light.
        /// </summary>
        public static double DopplerEffect(double frequency, double speedOfLight, double observerSpeed)
        {
            EnsureNonZero(speedOfLight, nameof(speedOfLight));
            return frequency * (speedOfLight + observerSpeed) / speedOfLight;
        }

        /// <summary>
        /// Calculates the Cherenkov threshold speed in a medium.
        /// </summary>
        public static double CherenkovRadiation(double speedOfLight, double refractiveIndex)
        {
            EnsureNonZero(refractiveIndex, nameof(refractiveIndex));
            return speedOfLight / refractiveIndex;
        }

        /// <summary>
        /// Calculates the magnitude of a simplified electromagnetic wave component vector.
        /// </summary>
        public static double EmWaveComponent(double electricField, double magneticField)
        {
            return Math.Sqrt(electricField * electricField + magneticField * magneticField);
        }

        /// <summary>
        /// Calculates the magnitude of a simplified electromagnetic wave component vector.
        /// </summary>
        /// <param name="electricField">The electric-field component.</param>
        /// <param name="magneticField">The magnetic-field component.</param>
        /// <returns>The combined component magnitude.</returns>
        public static double EMWaveComponent(double electricField, double magneticField)
        {
            return EmWaveComponent(electricField, magneticField);
        }

        /// <summary>
        /// Calculates the critical angle for total internal reflection.
        /// </summary>
        public static double CriticalAngle(double refractiveIndex1, double refractiveIndex2)
        {
            EnsureNonZero(refractiveIndex1, nameof(refractiveIndex1));
            var ratio = refractiveIndex2 / refractiveIndex1;
            if (ratio < -1d || ratio > 1d)
            {
                throw new ArgumentOutOfRangeException(nameof(refractiveIndex2), "The refractive-index ratio must be between -1 and 1.");
            }

            return Math.Asin(ratio);
        }

        /// <summary>
        /// Evaluates the thin-lens equation residual.
        /// </summary>
        public static double ThinLensEquation(double focalLength, double objectDistance, double imageDistance)
        {
            EnsureNonZero(focalLength, nameof(focalLength));
            EnsureNonZero(objectDistance, nameof(objectDistance));
            EnsureNonZero(imageDistance, nameof(imageDistance));
            return 1d / focalLength - 1d / objectDistance + 1d / imageDistance;
        }

        /// <summary>
        /// Calculates image distance for a plane mirror.
        /// </summary>
        public static double ImageDistancePlaneMirror(double objectDistance)
        {
            return -objectDistance;
        }

        /// <summary>
        /// Evaluates the spherical mirror equation residual.
        /// </summary>
        public static double SphericalMirrorEquation(double focalLength, double objectDistance, double imageDistance)
        {
            EnsureNonZero(focalLength, nameof(focalLength));
            EnsureNonZero(objectDistance, nameof(objectDistance));
            EnsureNonZero(imageDistance, nameof(imageDistance));
            return 1d / focalLength - 1d / objectDistance + 1d / imageDistance;
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
