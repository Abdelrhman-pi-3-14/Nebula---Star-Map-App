// UrsaUiManager.cs
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
using System;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif


public class UrsaUiManager : MonoBehaviour
{
    #region Sensor Components
    [Header("Sensor Configuration")]
    [Tooltip("Reference to the sensor handler component")]
    public UrsaSensorHandler sensorHandler;
    #endregion

    #region UI Core Components
    [Header("UI Foundation")]
    [Tooltip("Main canvas for UI elements")]
    public Canvas mainCanvas;

    [Tooltip("Blocks interactions during animations")]
    public CanvasGroup uiBlocker;

    [Tooltip("Handles UI element interaction detection")]
    public GraphicRaycaster graphicRaycaster;
    #endregion

    #region Location UI
    [Header("Location Panel")]
    [Tooltip("Popup panel for location updates")]
    public GameObject updateLocationPanel;

    [SerializeField] private TextMeshProUGUI latValue;
    [SerializeField] private TextMeshProUGUI longValue;

    [Tooltip("Confirms location update")]
    public Button updateLocationButton;

    [Tooltip("Cancels location update")]
    public Button denyLocationButton;
    #endregion

    #region Panel Animation Control
    [Header("Control Panel Animation")]
    [Tooltip("The main control panel transform")]
    public RectTransform controlPanel;

    [Tooltip("Visible position in viewport coordinates")]
    public Vector2 panelVisiblePos = new Vector2(0, 0);

    [Tooltip("Hidden position in viewport coordinates")]
    public Vector2 panelHiddenPos = new Vector2(-500, 0);

    [Tooltip("Animation duration in seconds")]
    [Range(0.1f, 2f)] public float panelSlideDuration = 0.3f;
    #endregion

    #region Back Button Control
    [Header("Back Button Animation")]
    [Tooltip("The back button transform")]
    public RectTransform backButton;

    [Tooltip("Visible position when active")]
    public Vector2 backButtonVisiblePos = new Vector2(-100, 0);

    [Tooltip("Hidden position when inactive")]
    public Vector2 backButtonHiddenPos = new Vector2(-100, -200);

    [Tooltip("Animation duration in seconds")]
    [Range(0.1f, 2f)] public float backButtonSlideDuration = 0.3f;
    #endregion

    #region Constellation Controls
    [Header("Constellation Features")]
    [Tooltip("Reference to constellation renderer")]
    public GameObject constellationRenderer;

    [Tooltip("Toggles constellation lines visibility")]
    public Button toggleLinesButton;

    [Tooltip("Toggles constellation labels visibility")]
    public Button toggleLabelsButton;

    private bool constellationVisble = true;

    [Tooltip("Toggles the coordinate lines")]
    public GameObject coordinateObj;
    private bool isCoordinateVisible = true;
    #endregion

    private bool isPanelVisible = false;
    private const int UPDATE_INTERVAL_DAYS = 7;
    private const int CANCEL_INTERVAL_HOURS = 24;


    void Start()
    {
        InitializeUI();
        SetupEventListeners();
        ConfigureCamera();
        if (ShouldShowLocationPrompt())
        {
            ShowLocationPrompt();
        }
    }


    private bool ShouldShowLocationPrompt()
    {
        if (!PlayerPrefs.HasKey("LocationPromptLastShown"))
            return true;

        DateTime lastShown = DateTime.Parse(PlayerPrefs.GetString("LocationPromptLastShown"));
        TimeSpan timeSinceLast = DateTime.UtcNow - lastShown;

        if (PlayerPrefs.GetInt("LocationPromptChoice", 0) == 1) // Update chosen
            return timeSinceLast.TotalDays >= UPDATE_INTERVAL_DAYS;
        else // Cancel chosen
            return timeSinceLast.TotalHours >= CANCEL_INTERVAL_HOURS;
    }

    private void ShowLocationPrompt()
    {
        if (updateLocationPanel != null)
        {
            updateLocationPanel.SetActive(true);
            PlayerPrefs.SetString("LocationPromptLastShown", DateTime.UtcNow.ToString("o"));
            PlayerPrefs.Save();
           
        }
    }

    #region Initialization Methods
    private void InitializeUI()
    {
        mainCanvas.renderMode = RenderMode.WorldSpace;
        mainCanvas.worldCamera = Camera.main;

        controlPanel.anchoredPosition = panelHiddenPos;
        backButton.anchoredPosition = backButtonHiddenPos;

        if (graphicRaycaster == null)
            graphicRaycaster = mainCanvas.gameObject.AddComponent<GraphicRaycaster>();
    }

    private void ConfigureCamera()
    {
        if (Camera.main != null && !Camera.main.TryGetComponent<PhysicsRaycaster>(out _))
            Camera.main.gameObject.AddComponent<PhysicsRaycaster>();
    }

    private void SetupEventListeners()
    {
        updateLocationButton.onClick.AddListener(OnLocationUpdateConfirmed);
        denyLocationButton.onClick.AddListener(OnLocationUpdateDenied);
    }
    #endregion

    #region Input Handling
    void Update()
    {
        HandleUserInput();
    }

    private void HandleUserInput()
    {
        if ((Input.GetMouseButtonDown(0) && !IsPointerOverUI()) || Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleControlPanel();
        }
    }

    private bool IsPointerOverUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
    #endregion

    #region Animation System
    public void ToggleControlPanel()
    {
        StopAllCoroutines();
        StartCoroutine(AnimateInterfaceElements());
        isPanelVisible = !isPanelVisible;
    }

    private IEnumerator AnimateInterfaceElements()
    {
        Vector2 panelTarget = isPanelVisible ? panelHiddenPos : panelVisiblePos;
        Vector2 backButtonTarget = isPanelVisible ? backButtonHiddenPos : backButtonVisiblePos;

        var panelRoutine = SlideElement(controlPanel, panelTarget, panelSlideDuration);
        var backButtonRoutine = SlideElement(backButton, backButtonTarget, backButtonSlideDuration);

        yield return StartCoroutine(panelRoutine);
        yield return StartCoroutine(backButtonRoutine);
    }

    private IEnumerator SlideElement(RectTransform element, Vector2 targetPos, float duration)
    {
        if (element == null || uiBlocker == null) yield break;

        uiBlocker.blocksRaycasts = true;
        Vector2 startPos = element.anchoredPosition;
        float elapsed = 0;

        while (elapsed < duration)
        {
            element.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        element.anchoredPosition = targetPos;
        uiBlocker.blocksRaycasts = false;
    }
    #endregion

    #region Location Handling
    private void RecordLocationChoice(int choice)
    {
        PlayerPrefs.SetInt("LocationPromptChoice", choice);
        PlayerPrefs.Save();
    }

    // Modify existing methods
    public void OnLocationUpdateConfirmed()
    {
        RecordLocationChoice(1); 
        StartCoroutine(ProcessLocationUpdate(true));
    }

    public void OnLocationUpdateDenied()
    {
        RecordLocationChoice(0); // Cancel
        StartCoroutine(ProcessLocationUpdate(false));
    }

    private IEnumerator ProcessLocationUpdate(bool confirmed)
    {
        if (uiBlocker == null) yield break;

        uiBlocker.blocksRaycasts = true;

        if (confirmed && sensorHandler != null)
        {
            sensorHandler.RefreshLocationOnce();
        }

        if (updateLocationPanel != null)
            updateLocationPanel.SetActive(false);

        uiBlocker.blocksRaycasts = false;
        yield return null;
    }

    
    #endregion

    #region Scene Management
    public void LoadScene(string sceneName)
    {
        StartCoroutine(RequestPermissionsAndLoad(sceneName));
    }

    private IEnumerator RequestPermissionsAndLoad(string sceneName)
    {
#if UNITY_ANDROID
        if (sceneName == "UrsaScene")
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Permission.RequestUserPermission(Permission.FineLocation);
                yield return new WaitUntil(() => Permission.HasUserAuthorizedPermission(Permission.FineLocation));
            }
        }
        else
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Permission.RequestUserPermission(Permission.Camera);
                yield return new WaitUntil(() => Permission.HasUserAuthorizedPermission(Permission.Camera));
            }
        }
#endif
        yield return StartCoroutine(TransitionToScene(sceneName));
    }

    private IEnumerator TransitionToScene(string sceneName)
    {
        yield return SlideElement(controlPanel, panelHiddenPos, panelSlideDuration);
        yield return SlideElement(backButton, backButtonHiddenPos, backButtonSlideDuration);

        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
    #endregion

    public void HideCoordnite()
    {
        isCoordinateVisible = !isCoordinateVisible;
        coordinateObj.SetActive(isCoordinateVisible);
    }

    public void ToggleConstellations()
    {
        constellationVisble = !constellationVisble;
        constellationRenderer.SetActive(constellationVisble);
    }



}
