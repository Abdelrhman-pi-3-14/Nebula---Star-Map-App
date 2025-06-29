using UnityEngine;

public class testUI : MonoBehaviour
{

    public GameObject popUP;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void BackClicked() => Debug.Log("back button Clicked :) ");
    public void NameClicked() => Debug.Log("Label button Clicked :) ");
    public void ConstellationClicked() => Debug.Log("constellaiton lines button Clicked :) ");
    public void coordinateClicked() => Debug.Log("coordinate button Clicked :) ");
    public void CancelClicked() => Debug.Log("Canel button Clicked :) ");
    public void UpdateClicked() => Debug.Log("Update button Clicked :) ");

    public void CanelPopUP() => popUP.SetActive(false);
}
