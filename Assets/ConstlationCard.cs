using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class ConstellationCard : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI latinNameText;
    public TextMeshProUGUI seasonText;
    public TextMeshProUGUI PrincipalStar;
    public TextMeshProUGUI IAU;
    public TextMeshProUGUI Dec;
    
    public Image thumbnailImage;

    [Header("Expansion Settings")]
    public float expansionDuration = 0.3f;
    public Vector2 expandedSize = new Vector2(400, 600);

    private ConstellationDataFetcher.ConstellationData data;
    private RectTransform rectTransform;
    private Vector2 originalSize;
    private bool isExpanded = false;
    private bool needsImageLoad = false;


    void OnEnable()
    {
        if (needsImageLoad)
        {
            needsImageLoad = false;
            StartCoroutine(LoadImage());
        }
    }

    public void Initialize(ConstellationDataFetcher.ConstellationData cardData)
    {
        data = cardData;
        UpdateBasicInfo();

        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(LoadImage());
        }
        else
        {
            needsImageLoad = true;
        }
    }

    void UpdateBasicInfo()
    {
        if (data == null)
        {
            Debug.LogWarning("Constellation data is null!");
            return;
        }

        nameText.text = data.EnglishName;
        latinNameText.text = data.LatinName;
        seasonText.text = data.Season;
        PrincipalStar.text = data.PrincipalStar;
        IAU.text = data.IAUCode;
        Dec.text = data.Declination;

    }

    IEnumerator LoadImage()
    {
        if (data == null || data.Image == null || string.IsNullOrEmpty(data.Image.Url))
        {
            Debug.LogWarning("Image URL missing");
            yield break;
        }

        using var www = UnityWebRequestTexture.GetTexture(data.Image.Url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            var texture = DownloadHandlerTexture.GetContent(www);
            var sprite = Sprite.Create(texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f));

            if (thumbnailImage != null)
            {
                thumbnailImage.sprite = sprite;
                thumbnailImage.preserveAspect = true;
            }
        }
        else
        {
            Debug.LogError($"Image download failed: {www.error}");
        }
    }
}