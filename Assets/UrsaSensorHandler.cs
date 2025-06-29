using System;
using UnityEngine;
using System.Collections;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif
using UnityEngine.Events;

[Serializable]
public class LocationEvent : UnityEvent<float, float> { }

public class UrsaSensorHandler : MonoBehaviour
{
    public float Latitude { get; private set; }
    public float Longitude { get; private set; }
    public float CurrentHeading => compass != null ? compass.trueHeading : 0f;
    public Quaternion CurrentGyroRotation => gyroAttitude;

    public bool IsLocationReady { get; private set; } = false;
    public DateTime LastLocationUpdateTime { get; private set; }
    public LocationServiceStatus LocationStatus { get; private set; }

    [Header("Configuration")]
    public float desiredAccuracyMeters = 10f;
    public float updateDistanceMeters = 10f;
    public int maxInitializationWaitTime = 30;


    [Header("Smoothing")]
    public float smoothingSpeed = 5f;

    [Header("Events")]
    public LocationEvent onLocationUpdated;
    public UnityEvent onLocationFailed;
    public UnityEvent onLocationPermissionDenied;
    public UnityEvent onLocationDisabledInSettings;

    private Compass compass;
    private Quaternion gyroAttitude;
    private bool locationServiceRunning;
    private bool locationPermissionGranted;
    private Quaternion referenceRotation = Quaternion.identity;

    void Start()
    {
        InitializeSensors();
        LoadCachedLocation();
        StartCoroutine(TestAndInitializeLocationService());
    }

    void InitializeSensors()
    {
        compass = Input.compass;
        compass.enabled = true;

        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
            Quaternion rawRotation = ConvertGyroRotation(Input.gyro.attitude);
            Quaternion relativeRotation = Quaternion.Inverse(referenceRotation) * rawRotation;
            gyroAttitude = Quaternion.Slerp(gyroAttitude, relativeRotation, Time.deltaTime * smoothingSpeed);
        }
    }

    private void LoadCachedLocation()
    {
        if (PlayerPrefs.HasKey("CachedLatitude") && PlayerPrefs.HasKey("CachedLongitude"))
        {
            Latitude = PlayerPrefs.GetFloat("CachedLatitude");
            Longitude = PlayerPrefs.GetFloat("CachedLongitude");
            IsLocationReady = true;

            if (PlayerPrefs.HasKey("LastLocationUpdate"))
            {
                LastLocationUpdateTime = DateTime.Parse(PlayerPrefs.GetString("LastLocationUpdate"));
            }
            Debug.Log($"[GPS] Loaded cached location: Lat={Latitude}, Lon={Longitude}");
            onLocationUpdated?.Invoke(Latitude, Longitude);
        }
    }

    public IEnumerator TestAndInitializeLocationService()
    {
        Debug.Log("[GPS] Starting location service test...");

#if UNITY_EDITOR
        // Simulate location in editor
        Debug.Log("[GPS] Running in Editor - using simulated location");
        CacheLocation(31.2001f, 29.9187f);
        LocationStatus = LocationServiceStatus.Running;
        yield break;
#else
        // First check if location is enabled in device settings
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogWarning("[GPS] Location services disabled in device settings!");
            LocationStatus = LocationServiceStatus.Stopped;
            onLocationDisabledInSettings?.Invoke();
            yield break;
        }

        // Check/request permission on Android
#if UNITY_ANDROID
        yield return RequestLocationPermission();
        if (!locationPermissionGranted)
        {
            Debug.LogWarning("[GPS] Location permission not granted");
            LocationStatus = LocationServiceStatus.Failed;
            onLocationPermissionDenied?.Invoke();
            yield break;
        }
#endif

        // Now test the location service
        yield return TestLocationService();
#endif
    }

#if UNITY_ANDROID
    private IEnumerator RequestLocationPermission()
    {
        if (Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            locationPermissionGranted = true;
            yield break;
        }

        Debug.Log("[GPS] Requesting location permission...");

        var callbacks = new PermissionCallbacks();
        callbacks.PermissionGranted += (permission) => {
            Debug.Log("[GPS] Permission granted");
            locationPermissionGranted = true;
        };
        callbacks.PermissionDenied += (permission) => {
            Debug.LogWarning("[GPS] Permission denied");
            locationPermissionGranted = false;
        };
        callbacks.PermissionDeniedAndDontAskAgain += (permission) => {
            Debug.LogWarning("[GPS] Permission permanently denied");
            locationPermissionGranted = false;
        };

        Permission.RequestUserPermission(Permission.FineLocation, callbacks);

        // Wait for permission decision
        float timeout = 10f;
        while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation) && timeout > 0)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }
    }
#endif

    private IEnumerator TestLocationService()
    {
        Debug.Log("[GPS] Testing location service...");

        // Stop service if already running
        if (locationServiceRunning)
        {
            Input.location.Stop();
            yield return new WaitForSeconds(1);
        }

        // Start service with desired settings
        Input.location.Start(desiredAccuracyMeters, updateDistanceMeters);
        LocationStatus = Input.location.status;
        locationServiceRunning = true;

        // Wait for initialization
        int waitTime = maxInitializationWaitTime;
        while (Input.location.status == LocationServiceStatus.Initializing && waitTime > 0)
        {
            Debug.Log($"[GPS] Initializing... {waitTime} seconds remaining");
            LocationStatus = Input.location.status;
            yield return new WaitForSeconds(1);
            waitTime--;
        }

        // Check final status
        LocationStatus = Input.location.status;

        if (LocationStatus != LocationServiceStatus.Running)
        {
            Debug.LogError($"[GPS] Failed to initialize. Status: {LocationStatus}");
            onLocationFailed?.Invoke();
            yield break;
        }

        // Success - get first reading
        UpdateLocationData();
    }

    private void UpdateLocationData()
    {
        LocationStatus = Input.location.status;

        if (LocationStatus != LocationServiceStatus.Running)
        {
            Debug.LogWarning("[GPS] Can't update - service not running");
            return;
        }

        LocationInfo data = Input.location.lastData;
        if (data.timestamp > 0)
        {
            CacheLocation(data.latitude, data.longitude);
            Debug.Log($"[GPS] Location updated: Lat={Latitude}, Lon={Longitude}, Accuracy={data.horizontalAccuracy}m");
            onLocationUpdated?.Invoke(Latitude, Longitude);
        }
    }

    private void CacheLocation(float latitude, float longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
        IsLocationReady = true;
        LastLocationUpdateTime = DateTime.UtcNow;
        PlayerPrefs.SetString("LastLocationUpdate", LastLocationUpdateTime.ToString("o"));
        PlayerPrefs.SetFloat("CachedLatitude", latitude);
        PlayerPrefs.SetFloat("CachedLongitude", longitude);
        PlayerPrefs.Save();
    }

    public void RefreshLocationOnce()
    {
        if (!locationServiceRunning)
        {
            StartCoroutine(TestAndInitializeLocationService());
        }
        else
        {
            UpdateLocationData();
        }
    }

    private void UpdateOrientation()
    {
        if (SystemInfo.supportsGyroscope)
        {
            gyroAttitude = ConvertGyroRotation(Input.gyro.attitude);
        }
    }

    private Quaternion ConvertGyroRotation(Quaternion raw)
    {
        return new Quaternion(raw.x, raw.y, -raw.z, -raw.w);
    }

    void Update()
    {
        UpdateOrientation();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && !locationServiceRunning)
        {
            StartCoroutine(TestAndInitializeLocationService());
        }
    }

    void OnApplicationPause(bool isPaused)
    {
        if (isPaused && locationServiceRunning)
        {
            StopLocationService();
        }
        else if (!isPaused && !locationServiceRunning)
        {
            StartCoroutine(TestAndInitializeLocationService());
        }
    }

    public void StopLocationService()
    {
        if (locationServiceRunning)
        {
            Input.location.Stop();
            locationServiceRunning = false;
            LocationStatus = LocationServiceStatus.Stopped;
            Debug.Log("[GPS] Location service stopped");
        }
    }

    void OnDestroy()
    {
        StopLocationService();
    }

    // Debug GUI - useful for testing
    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 24;
        style.normal.textColor = Color.white;

        GUI.Label(new Rect(10, 10, 500, 100), $"[GPS Status]\n" +
            $"Running: {locationServiceRunning}\n" +
            $"Status: {LocationStatus}\n" +
            $"Lat: {Latitude:F6}\n" +
            $"Lon: {Longitude:F6}", style);
    }
}