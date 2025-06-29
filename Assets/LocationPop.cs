using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class LocationPopupUI : MonoBehaviour
{
    public GameObject popupPanel;
    public TMP_Text longitudeText;
    public TMP_Text latitudeText;
    public Button updateButton;
    public Button cancelButton;

    public float updateCooldownDays = 7f;
    public float cancelCooldownHours = 24f;

    private UrsaSensorHandler gps;
    private bool isWaitingForUpdate;

    void Start()
    {
        gps = FindObjectOfType<UrsaSensorHandler>();
        updateButton.onClick.AddListener(OnUpdateClicked);
        cancelButton.onClick.AddListener(OnCancelClicked);
        popupPanel.SetActive(false);
        updateButton.interactable = true;
        gps.onLocationUpdated.AddListener(OnLocationUpdated);
        gps.onLocationFailed.AddListener(OnLocationFailed);
        if (ShouldShowPopup() && gps.IsLocationReady)
        {
            ShowPopup();
        }
    }

    private bool ShouldShowPopup()
    {
        if (!PlayerPrefs.HasKey("LastPopupTime")) return true;
        int lastChoice = PlayerPrefs.GetInt("LocationChoice", 0);
        DateTime lastTime = DateTime.Parse(PlayerPrefs.GetString("LastPopupTime"));
        TimeSpan timeSince = DateTime.UtcNow - lastTime;
        if (lastChoice == 1)
            return timeSince.TotalDays >= updateCooldownDays;
        else
            return timeSince.TotalHours >= cancelCooldownHours;
    }

    public void ShowPopup()
    {
        if (!gps.IsLocationReady) return;
        popupPanel.SetActive(true);
        RefreshLocationDisplay();
    }

    public void RefreshLocationDisplay()
    {
        if (gps.IsLocationReady)
        {
            latitudeText.text = gps.Latitude.ToString("F6");
            longitudeText.text = gps.Longitude.ToString("F6");
        }
    }

    private void OnUpdateClicked()
    {
        if (isWaitingForUpdate) return;
        StartCoroutine(ProcessLocationUpdate());
    }

    private IEnumerator ProcessLocationUpdate()
    {
        isWaitingForUpdate = true;
        updateButton.interactable = false;
        gps.RefreshLocationOnce();
        float timeout = 30f;
        while (isWaitingForUpdate && timeout > 0)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }
        if (isWaitingForUpdate)
        {
            OnLocationFailed();
        }
    }

    private void OnCancelClicked()
    {
        PlayerPrefs.SetInt("LocationChoice", 0);
        PlayerPrefs.SetString("LastPopupTime", DateTime.UtcNow.ToString("o"));
        popupPanel.SetActive(false);
    }

    private void OnLocationUpdated(float lat, float lon)
    {
        if (!isWaitingForUpdate) return;
        isWaitingForUpdate = false;
        updateButton.interactable = true;
        RefreshLocationDisplay();
        PlayerPrefs.SetInt("LocationChoice", 1);
        PlayerPrefs.SetString("LastPopupTime", DateTime.UtcNow.ToString("o"));
        popupPanel.SetActive(false);
    }

    private void OnLocationFailed()
    {
        isWaitingForUpdate = false;
        updateButton.interactable = true;
    }

    void OnDestroy()
    {
        if (gps != null)
        {
            gps.onLocationUpdated.RemoveListener(OnLocationUpdated);
            gps.onLocationFailed.RemoveListener(OnLocationFailed);
        }
    }
}
