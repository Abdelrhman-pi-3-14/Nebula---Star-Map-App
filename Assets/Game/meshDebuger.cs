using UnityEngine;

public class ColliderChecker : MonoBehaviour
{
    void Start()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>(true);
        if (colliders.Length == 0)
        {
            Debug.Log("No colliders found in children.");
        }
        else
        {
            foreach (Collider col in colliders)
            {
                Debug.Log("Collider found on: " + col.gameObject.name);
            }
        }
    }
}
