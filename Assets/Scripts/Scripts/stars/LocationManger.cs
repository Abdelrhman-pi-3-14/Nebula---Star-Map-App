using UnityEngine;

public class LocationManger
{
    private double lastLatitude = double.NaN;
    private double lastLongitude = double.NaN;
    private const float updateDistanceLat = 0.5f;
    private const float updateDistanceLon = 0.5f;


    public bool isLocationChanged(double latitude, double longitude)
    {
        if (double.IsNaN(lastLatitude) || double.IsNaN(lastLongitude))
        {
            lastLatitude = latitude;
            lastLongitude = longitude;
            return true;
        }
        else
        {
            if (Mathf.Abs((float)(lastLatitude - latitude)) > updateDistanceLat || Mathf.Abs((float)(lastLongitude - longitude)) > updateDistanceLon)
            {
                lastLatitude = latitude;
                lastLongitude = longitude;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}
