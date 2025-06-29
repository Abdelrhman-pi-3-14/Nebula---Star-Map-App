using NUnit.Framework;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/*public class dataSetFeltering : MonoBehaviour
{
    public float updateInterval = 3600f; // Update every hour
    private float lastUpdateTime = -1;
    private List<Star> starList;
    private UserData user;
    private LocationManger locationManager = new LocationManger();

    void Start()
    {
        user = new UserData(31.2001, 29.9187, System.DateTime.Now); // Example location
        starList = StarDataLoader.LoadData();

        if (starList == null || starList.Count == 0)
        {
            Debug.LogError("? Star list is empty after loading! Check StarDataLoader.");
            return;
        }

        Debug.Log($"? Total stars loaded: {starList.Count}");
    }

    void Update()
    {
        if (Time.time - lastUpdateTime > updateInterval || locationManager.isLocationChanged(user.latitude, user.longitude))
        {
            Debug.Log("?? Updating star positions and filtering...");
            lastUpdateTime = Time.time;
        }
    }



   public static List<Star> FilterAndUpdateStars(List<Star> starList , UserData user
       )
    {
        // Convert RA-Dec to Alt-Az and filter stars based on altitude
        List<Star> filteredStars = new List<Star>();

        foreach (Star star in starList)
        {
            star.position = StarPositions.CalculatePosition(star.ra, star.dec, user);
            if (star.position.y >= 0) // Keep only visible stars (altitude > 0)
            {
                filteredStars.Add(star);
            }
        }

        Debug.Log($"?? Stars after filtering: {filteredStars.Count}");

        if (filteredStars.Count == 0)
        {
            Debug.LogError("? No stars left after filtering! Check altitude calculations.");
        }

        // Update the star list and re-render
        return filteredStars;
    }

  
}*/
