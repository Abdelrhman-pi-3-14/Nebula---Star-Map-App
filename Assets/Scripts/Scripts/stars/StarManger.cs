using System;
using System.Collections.Generic;
using UnityEngine;

public class StarManager : MonoBehaviour
{
    public GameObject starPrefab;
    public float skyRadius = 200f;
    public float updateInterval = 3600f;
    public Dictionary<int, Star> starDictionary = new Dictionary<int, Star>();
    private List<Star> starList;
    private UserData user;
    private float lastUpdateTime = 0f;
    private UrsaSensorHandler locationManager;

    void Start()
    {
        locationManager = FindObjectOfType<UrsaSensorHandler>();
        if (locationManager == null)
        {
            Debug.LogError("SensorHandler not found!");
            return;
        }

        if (locationManager.IsLocationReady)
        {
            user = new UserData(locationManager.Longitude, locationManager.Latitude, DateTime.UtcNow);
        }
        else
        {
            user = new UserData(0, 0, DateTime.UtcNow);
        }
        LoadStars();
    }

    void Update()
    {
        if (Time.time - lastUpdateTime > updateInterval ||
            (locationManager.IsLocationReady ))
        {
            lastUpdateTime = Time.time;
            user.time = DateTime.UtcNow;
            if (locationManager.IsLocationReady)
            {
                user.latitude = locationManager.Latitude;
                user.longitude = locationManager.Longitude;
            }
            starList = StarPositions.AltFiltration(starList, user);
            StarPositions.UpdateStarPositions(starList, user);
            UpdateStarObjects();
            FindObjectOfType<ConstellationRenderer>().UpdateConstellations();
        }
    }

    private void LoadStars()
    {
        starList = StarDataLoader.LoadData();
        starDictionary = new Dictionary<int, Star>();
        foreach (Star s in starList)
        {
            int id = (int)s.catalog_number;
            if (!starDictionary.ContainsKey(id))
            {
                starDictionary.Add(id, s);
            }
        }

        if (starList == null || starList.Count == 0)
        {
            Debug.LogError("Star list is empty after loading! Check StarDataLoader.");
            return;
        }

        StarPositions.UpdateStarPositions(starList, user);
        PlotStars();
    }

    void PlotStars()
    {
        if (starPrefab == null)
        {
            Debug.LogError("Star prefab is null!");
            return;
        }

        foreach (Star star in starList)
        {
            Vector3 pos = star.position.normalized * skyRadius;
            GameObject starObj = Instantiate(starPrefab, pos, Quaternion.identity, transform);
            StarPrefab sp = starObj.GetComponent<StarPrefab>();
            if (sp != null)
            {
                sp.Initialize(star);
            }
            else
            {
                Debug.LogError("StarPrefab component missing!");
            }
        }
    }

    void UpdateStarObjects()
    {
        StarPrefab[] starObjects = GetComponentsInChildren<StarPrefab>();
        foreach (StarPrefab sp in starObjects)
        {
            if (sp.starData != null)
            {
                sp.starData.position = StarPositions.CalculatePosition(sp.starData.ra, sp.starData.dec, user);
                sp.transform.position = sp.starData.position.normalized * skyRadius;
            }
        }
    }
}
