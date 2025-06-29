using UnityEngine;

public class CelestialCalculator : MonoBehaviour
{
    public static CelestialCalculator Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetLocation(float latitude, float longitude)
    {
        // Implement your celestial calculations here
        Debug.Log($"Setting location for calculations: Lat={latitude}, Lon={longitude}");

        // Example: Pass to other components
        // StarPositioner.Instance.SetObserverLocation(latitude, longitude);
    }
}