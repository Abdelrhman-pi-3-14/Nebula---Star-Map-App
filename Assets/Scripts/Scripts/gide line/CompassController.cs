using UnityEngine;

namespace Ursa
{
    public class CompassController : MonoBehaviour
    {

        public float heading = 0f;

        public Transform rotatingContainer;

        public Transform meridian;

        void Update()
        {

            if (rotatingContainer != null)
            {
                rotatingContainer.rotation = Quaternion.Euler(0, -heading, 0);
            }


            if (meridian != null)
            {
                meridian.rotation = Quaternion.identity;
            }
        }


        public void SetHeading(string newHeading)
        {
            if (float.TryParse(newHeading, out float parsedHeading))
            {
                heading = parsedHeading;
            }
            else
            {
                Debug.LogWarning("SetHeading: Unable to parse heading: " + newHeading);
            }
        }
    }
}