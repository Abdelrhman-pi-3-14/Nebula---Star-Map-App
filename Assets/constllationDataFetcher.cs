using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class ConstellationDataFetcher : MonoBehaviour
{
    [Header("API Settings")]
    [SerializeField] private string apiUrl = "https://www.datastro.eu/api/explore/v2.1/catalog/datasets/88-constellations/records?limit=88";

    [Header("UI Components")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardParent;

    [Serializable]
    private class ApiResponse
    {
        public List<ConstellationData> results;
    }

    [Serializable]
    public class ConstellationData
    {
        [JsonProperty("english_name_nom_en_anglais")]
        public string EnglishName { get; set; }

        [JsonProperty("latin_name_nom_latin")]
        public string LatinName { get; set; }

        [JsonProperty("french_name_nom_francais")]
        public string FrenchName { get; set; }

        [JsonProperty("iau_code")]
        public string IAUCode { get; set; }

        [JsonProperty("dec_declinaison")]
        public string Declination { get; set; }

        [JsonProperty("constellation_area_in_degrees_etendue_de_la_constellation_en_degres_2")]
        public float AreaInDegrees { get; set; }

        [JsonProperty("principal_star_etoile_principale")]
        public string PrincipalStar { get; set; }

        [JsonProperty("season_saison")]
        public string Season { get; set; }

        [JsonProperty("image")]
        public ImageData Image { get; set; }

        [Serializable]
        public class ImageData
        {
            [JsonProperty("url")]
            public string Url { get; set; }
        }
    }
    private void Awake()
    {
        if (cardPrefab == null)
            Debug.LogError("[ConstellationDataFetcher] cardPrefab is not assigned in the Inspector!");
        if (cardParent == null)
            Debug.LogError("[ConstellationDataFetcher] cardParent is not assigned in the Inspector!");
    }

    private IEnumerator Start()
    {
        if (cardPrefab == null || cardParent == null)
            yield break;

        using var request = UnityWebRequest.Get(apiUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[ConstellationDataFetcher] API request failed: {request.error}");
            yield break;
        }

        ProcessData(request.downloadHandler.text);
    }

    private void ProcessData(string json)
    {
        ApiResponse response;
        try
        {
            response = JsonConvert.DeserializeObject<ApiResponse>(json);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ConstellationDataFetcher] JSON parse error: {ex}");
            return;
        }

        if (response?.results == null || response.results.Count == 0)
        {
            Debug.LogWarning("[ConstellationDataFetcher] No constellation data returned.");
            return;
        }

        foreach (var data in response.results)
        {
            if (data == null) continue;
            CreateCard(data);
        }
    }

    private void CreateCard(ConstellationData data)
    {
        if (cardPrefab == null || cardParent == null) return;

        var cardGO = Instantiate(cardPrefab, cardParent);

        cardGO.SetActive(true);

        if (cardGO == null)
        {
            Debug.LogError("[ConstellationDataFetcher] Instantiate returned null!");
            return;
        }

        var card = cardGO.GetComponent<ConstellationCard>();
        if (card == null)
        {
            Debug.LogError("[ConstellationDataFetcher] The card prefab lacks a ConstellationCard component!");
            Destroy(cardGO);
            return;
        }
        Debug.Log($"Loaded: {data.EnglishName} | " +
          $"Latin: {data.LatinName} | " +
          $"Star: {data.PrincipalStar} | " +
          $"Season: {data.Season} | " );

        card.Initialize(data);
    }
}   
