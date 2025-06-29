using UnityEngine;
using UnityEngine.UI;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class CameraFeedHandler : MonoBehaviour
{
    [Header("UI Preview")]
    public RawImage previewImage;

    private WebCamTexture _webcamTexture;

    void Awake()
    {
        _webcamTexture = new WebCamTexture();
    }

    public void InitializeCamera()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Debug.LogError("Camera permission not granted!");
            return;
        }
#endif
        if (previewImage != null)
        {
            previewImage.texture = _webcamTexture;
        }
        _webcamTexture.Play();
    }

    void OnDisable()
    {
        if (_webcamTexture != null && _webcamTexture.isPlaying)
            _webcamTexture.Stop();
    }
}
