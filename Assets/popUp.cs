using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LocationPopupManager : MonoBehaviour
{
    public GameObject popupPanel;
    public Text latLongText;
    public Text titleText;
    public Text latitudeText;
    public Text longitudeText;
    private const string LatitudeKey = "Latitude";
    private const string LongitudeKey = "Longitude";
    private const string LastUpdateKey = "LastUpdateTime";
    private const float UpdateIntervalDays = 7f;

    // Ensures the popup is shown only once per session if user cancels.
    private bool popupShownThisSession = false;

    void Start()
    {
        popupPanel.SetActive(false);
        float latitude = PlayerPrefs.GetFloat(LatitudeKey, 0f);
        float longitude = PlayerPrefs.GetFloat(LongitudeKey, 0f);
        string lastUpdateStr = PlayerPrefs.GetString(LastUpdateKey, DateTime.MinValue.ToString("o"));
        DateTime lastUpdate;
        if (!DateTime.TryParse(lastUpdateStr, out lastUpdate))
            lastUpdate = DateTime.MinValue;
        TimeSpan diff = DateTime.Now - lastUpdate;
        if (diff.TotalDays >= UpdateIntervalDays && !popupShownThisSession)
        {
            ShowPopup(latitude, longitude);
        }
    }

    public void ShowPopup(float latitude, float longitude)
    {
        if (titleText != null) titleText.text = "Is this your current location?";
        if (latitudeText != null) latitudeText.text = "LATITUDE : " + latitude;
        if (longitudeText != null) longitudeText.text = "LONGITUDE : " + longitude;
        if (popupPanel != null) popupPanel.SetActive(true);
    }


    // Called by the update button handler.
    public void UpdateLocation()
    {
        StartCoroutine(UpdateLocationCoroutine());
        popupPanel.SetActive(false);
    }

    IEnumerator UpdateLocationCoroutine()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location services not enabled");
            yield break;
        }
        Input.location.Start();
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        if (Input.location.status != LocationServiceStatus.Running)
        {
            Debug.Log("Location service failed to start.");
            yield break;
        }
        float newLat = Input.location.lastData.latitude;
        float newLon = Input.location.lastData.longitude;
        PlayerPrefs.SetFloat(LatitudeKey, newLat);
        PlayerPrefs.SetFloat(LongitudeKey, newLon);
        PlayerPrefs.SetString(LastUpdateKey, DateTime.Now.ToString("o"));
        PlayerPrefs.Save();
        Debug.Log($"Location updated: {newLat}, {newLon}");
        Input.location.Stop();
        yield return null;
    }

    // Called by the cancel button handler.
    public void CancelLocationUpdate()
    {
        popupPanel.SetActive(false);
    }
}
