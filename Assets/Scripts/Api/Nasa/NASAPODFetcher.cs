using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections;

public class NASAPODFetcher : MonoBehaviour
{
    public Image imageDisplay;        
    public TMP_Text titleText;          
    public TMP_Text descriptionText;    
    public Sprite errorImageSprite;    
    private const string NASA_API_KEY = "sPICuP7OiVYXnFHrgXj1ojY2Dh6cTuc7wQEdyv6D";
    private const string NASA_URL = "https://api.nasa.gov/planetary/apod?api_key=" + NASA_API_KEY;

    void Start()
    {
        StartCoroutine(GetAPODData());
    }

    IEnumerator GetAPODData()
    {
        UnityWebRequest request = UnityWebRequest.Get(NASA_URL);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("API Error: " + request.error);
            DisplayErrorImage();
            yield break;
        }

        string json = request.downloadHandler.text;

        NasaDto data = JsonConvert.DeserializeObject<NasaDto>(json);

        titleText.text = data.title;
        descriptionText.text = data.explanation;

        if (data.media_type == "image")
        {
            UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(data.url);
            yield return imageRequest.SendWebRequest();

            if (imageRequest.result == UnityWebRequest.Result.Success)
            {
                Texture2D tex = ((DownloadHandlerTexture)imageRequest.downloadHandler).texture;

                Sprite sprite = TextureToSprite(tex);

                imageDisplay.sprite = sprite;
            }
            else
            {
                Debug.LogError("Image Load Error: " + imageRequest.error);
                DisplayErrorImage();
            }
        }
        else
        {
            titleText.text += "\n\n(Note: Today's media is not an image.)";
        }
    }

    private void DisplayErrorImage()
    {
        if (errorImageSprite != null)
        {
            imageDisplay.sprite = errorImageSprite;
        }
        else
        {
            Debug.LogWarning("Error Image not set.");
        }
    }

    private Sprite TextureToSprite(Texture2D texture)
    {
        Rect rect = new Rect(0, 0, texture.width, texture.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);  
        return Sprite.Create(texture, rect, pivot);
    }
}
