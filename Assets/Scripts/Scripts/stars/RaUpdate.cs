using UnityEngine;

public class RaUpdate 
{
    private float lastUpdateTime = -1;
    private const float UPDATE_INTERVAL = 3600f;

    public bool shouldUpdate()
    {
        if (lastUpdateTime == -1)
        {
            lastUpdateTime = Time.time;
            return true;
        }
        else if (Time.time - lastUpdateTime > UPDATE_INTERVAL)
        {
            lastUpdateTime = Time.time;
            return true;
        }
        return false;
    }

}
