using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Android;
using System;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject infoPanel;
    public GameObject newsPanel;
    public GameObject permissionPrompt;

    [Header("Permission UI")]
    public TMP_Text permissionMessage;
    public Button allowButton;
    public Button denyButton;

    [Header("NASA API Components")]
    public Image newsImageDisplay;
    public TMP_Text newsTitleText;
    public TMP_Text newsDescriptionText;
    public Sprite errorImageSprite;

    [Header("Scene Names")]
    public string celestialSceneName;
    public string observeSceneName;

    private const string NASA_API_KEY = "sPICuP7OiVYXnFHrgXj1ojY2Dh6cTuc7wQEdyv6D";
    private string nasaUrl = "https://api.nasa.gov/planetary/apod?api_key=";

    private GameObject currentPanel;
    private Stack<GameObject> panelHistory = new Stack<GameObject>();
    private Action pendingSceneAction;

    void Start()
    {
        nasaUrl += NASA_API_KEY;
        DeactivateAllPanels();
        OpenPanel(mainMenuPanel);

        // Initialize permission UI
        if (permissionPrompt != null)
        {
            permissionPrompt.SetActive(false);
            allowButton.onClick.AddListener(OnAllowPermission);
            denyButton.onClick.AddListener(OnDenyPermission);
        }
    }

    public void OpenPanel(GameObject panel)
    {
        if (panel == null)
        {
            Debug.LogError("Trying to open null panel!");
            return;
        }

        if (currentPanel != null)
        {
            currentPanel.SetActive(false);
            panelHistory.Push(currentPanel);
        }

        currentPanel = panel;
        currentPanel.SetActive(true);

        if (panel == newsPanel)
        {
            StartCoroutine(FetchNASAData());
        }
    }

    IEnumerator FetchNASAData()
    {
        using UnityWebRequest request = UnityWebRequest.Get(nasaUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("API Error: " + request.error);
            ShowErrorState();
            yield break;
        }

        NasaData data = JsonConvert.DeserializeObject<NasaData>(request.downloadHandler.text);
        UpdateNASAUI(data);

        if (data.media_type == "image")
        {
            yield return StartCoroutine(LoadNASAImage(data.url));
        }
    }

    void UpdateNASAUI(NasaData data)
    {
        newsTitleText.text = data.title;
        newsDescriptionText.text = data.explanation;

        if (data.media_type != "image")
        {
            newsDescriptionText.text += $"\n\n(Media type not supported: {data.media_type})";
        }
    }

    IEnumerator LoadNASAImage(string url)
    {
        using UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(url);
        yield return imageRequest.SendWebRequest();

        if (imageRequest.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(imageRequest);
            newsImageDisplay.sprite = CreateSprite(texture);
        }
        else
        {
            ShowErrorState();
        }
    }

    Sprite CreateSprite(Texture2D texture)
    {
        return Sprite.Create(texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f));
    }

    void ShowErrorState()
    {
        newsImageDisplay.sprite = errorImageSprite;
        newsDescriptionText.text = "Failed to load NASA data. Please try again later.";
    }

    public void OpenNewsPanel() => OpenPanel(newsPanel);
    public void OpenInfoPanel() => OpenPanel(infoPanel);

    public void GoBack()
    {
        if (panelHistory.Count > 0)
        {
            currentPanel.SetActive(false);
            currentPanel = panelHistory.Pop();
            currentPanel.SetActive(true);
        }
        else
        {
            ReturnToMainMenu();
        }
    }

    public void ReturnToMainMenu()
    {
        if (currentPanel != null)
            currentPanel.SetActive(false);

        DeactivateAllPanels();
        mainMenuPanel.SetActive(true);
        currentPanel = mainMenuPanel;
        panelHistory.Clear();
    }

    private void DeactivateAllPanels()
    {
        mainMenuPanel.SetActive(false);
        infoPanel.SetActive(false);
        newsPanel.SetActive(false);
    }

    #region Scene Loading with Permissions
    public void LoadCelestialScene()
    {
        RequestLocationPermission(() => {
            SceneManager.LoadScene(celestialSceneName);
            StartCoroutine(InitializeLocationAfterSceneLoad());
        });
    }

    public void LoadObserveScene()
    {
        RequestCameraPermission(() => {
            SceneManager.LoadScene(observeSceneName);
            StartCoroutine(InitializeCameraAfterSceneLoad());
        });
    }

    private void RequestLocationPermission(Action onGranted)
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            pendingSceneAction = onGranted;
            ShowPermissionPrompt("Location access is required for accurate star positioning");
            return;
        }
#endif
        onGranted?.Invoke();
    }

    private void RequestCameraPermission(Action onGranted)
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            pendingSceneAction = onGranted;
            ShowPermissionPrompt("Camera access is required for sky observation");
            return;
        }
#endif
        onGranted?.Invoke();
    }

    private void ShowPermissionPrompt(string message)
    {
        if (permissionPrompt == null) return;

        permissionMessage.text = message;
        permissionPrompt.SetActive(true);
    }

    private void OnAllowPermission()
    {
        permissionPrompt.SetActive(false);

#if UNITY_ANDROID
        if (pendingSceneAction == LoadCelestialScene)
        {
            RequestLocation();
        }
        else if (pendingSceneAction == LoadObserveScene)
        {
            RequestCamera();
        }
#endif
    }

    private void OnDenyPermission()
    {
        permissionPrompt.SetActive(false);
        Debug.Log("Permission denied by user");
        // Optional: Show message that feature won't work without permission
    }

    private void RequestLocation()
    {
#if UNITY_ANDROID
        PermissionCallbacks callbacks = new PermissionCallbacks();
        callbacks.PermissionGranted += _ => {
            Debug.Log("Location permission granted");
            pendingSceneAction?.Invoke();
        };
        callbacks.PermissionDenied += _ => {
            Debug.LogError("Location permission denied");
            ShowPermissionPrompt("Location permission is required for this feature");
        };
        Permission.RequestUserPermission(Permission.FineLocation, callbacks);
#endif
    }

    private void RequestCamera()
    {
#if UNITY_ANDROID
        PermissionCallbacks callbacks = new PermissionCallbacks();
        callbacks.PermissionGranted += _ => {
            Debug.Log("Camera permission granted");
            pendingSceneAction?.Invoke();
        };
        callbacks.PermissionDenied += _ => {
            Debug.LogError("Camera permission denied");
            ShowPermissionPrompt("Camera permission is required for this feature");
        };
        Permission.RequestUserPermission(Permission.Camera, callbacks);
#endif
    }
    #endregion

    #region Scene Initialization
    private IEnumerator InitializeLocationAfterSceneLoad()
    {
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == celestialSceneName);

        // Initialize location services
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogError("Location services not enabled");
            yield break;
        }

        Input.location.Start(10f, 10f); // Accuracy of 10 meters, update every 10 meters

        int maxWait = 20; // 20 seconds max
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (Input.location.status == LocationServiceStatus.Running)
        {
            LocationInfo location = Input.location.lastData;
            Debug.Log($"Location initialized: {location.latitude}, {location.longitude}");

            // Pass to celestial calculator
            CelestialCalculator.Instance?.SetLocation(location.latitude, location.longitude);
        }
        else
        {
            Debug.LogError($"Location services failed: {Input.location.status}");
        }
    }

    private IEnumerator InitializeCameraAfterSceneLoad()
    {
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == observeSceneName);

        // Find camera handler in the scene
        CameraFeedHandler cameraHandler = FindObjectOfType<CameraFeedHandler>();
        if (cameraHandler == null)
        {
            Debug.LogError("CameraFeedHandler not found in scene!");
            yield break;
        }

        // Initialize camera
        cameraHandler.InitializeCamera();
    }
    #endregion

    private class NasaData
    {
        public string title;
        public string explanation;
        public string url;
        public string media_type;
    }
}




