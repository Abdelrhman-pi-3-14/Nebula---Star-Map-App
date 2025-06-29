using UnityEngine;

using System;
using System.Collections;

namespace Ursa
{
    public class SensorReader : MonoBehaviour
    {
        // Expose sensor data so you can see it in the Inspector or use it elsewhere
        [Header("Accelerometer")]
        public Vector3 Accelerometer; // Device acceleration

        [Header("Gyroscope")]
        public Vector3 GyroRotationRate;  // Angular velocity in rad/s
        public Quaternion GyroAttitude;   // Orientation

        [Header("Compass")]
        public Vector3 CompassRawVector;  // Raw magnetic field vector
        public float CompassTrueHeading;  // Computed true heading

        [Header("Location")]
        public LocationInfo LocationData;

        [Header("Time")]
        public DateTime CurrentTime;

        void Start()
        {
            // Enable gyroscope if supported
            if (SystemInfo.supportsGyroscope)
            {
                Input.gyro.enabled = true;
            }
            else
            {
                Debug.LogWarning("Gyroscope not supported on this device.");
            }

            // Enable compass
            Input.compass.enabled = true;


            // Start location services
            StartCoroutine(StartLocationService());
        }

        IEnumerator StartLocationService()
        {
            // Check if user has location enabled
            if (!Input.location.isEnabledByUser)
            {
                Debug.LogWarning("Location services are not enabled by the user.");
                yield break;
            }

            Input.location.Start();

            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            if (maxWait <= 0)
            {
                Debug.LogWarning("Timed out while initializing location services.");
                yield break;
            }

            if (Input.location.status == LocationServiceStatus.Failed)
            {
                Debug.LogWarning("Unable to determine device location.");
                yield break;
            }
        }

        void Update()
        {
            // Read accelerometer data
            Accelerometer = Input.acceleration;
            CurrentTime = DateTime.Now;

            // Read gyroscope data if supported
            if (SystemInfo.supportsGyroscope)
            {
                GyroRotationRate = Input.gyro.rotationRate;
                GyroAttitude = Input.gyro.attitude;
            }

            // Read compass data
            CompassRawVector = Input.compass.rawVector;
            CompassTrueHeading = Input.compass.trueHeading;

            // Read location data if available
            if (Input.location.status == LocationServiceStatus.Running)
            {
                LocationData = Input.location.lastData;
            }
        }
    }

    public class SensorApplier : MonoBehaviour
    {
        [Tooltip("Reference to the SensorReader script in your scene.")]
        public SensorReader sensorReader;

        void Update()
        {
            if (sensorReader != null && SystemInfo.supportsGyroscope)
            {
                // Apply the gyroscope attitude to this GameObject's rotation.
                // Note: Depending on your use-case, you may need to adjust for device orientation.
                transform.localRotation = sensorReader.GyroAttitude;
            }
        }
    }
}
