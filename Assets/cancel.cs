using UnityEngine;
using UnityEngine.UI;

namespace Ursa
{
    public class CancelLocationButtonHandler : MonoBehaviour
    {
        public LocationPopupManager locationPopupManager;
       
        public void OnCancelButtonClicked() {
           /* if (locationPopupManager != null)
            {
                locationPopupManager.CancelLocationUpdate();
            }*/
            Debug.Log("🔥 Listener added!");

            

        }
       
          
        }
    }

