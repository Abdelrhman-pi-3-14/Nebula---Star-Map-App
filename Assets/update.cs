using UnityEngine;

namespace Ursa
{
    public class UpdateLocationButtonHandler : MonoBehaviour
    {
        public LocationPopupManager locationPopupManager;

        public void OnButtonClicked()
        {
            if (locationPopupManager != null)
            {
                locationPopupManager.UpdateLocation();
            }
        }
    }
}
