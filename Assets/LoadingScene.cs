using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    // Function to load the Ursa scene
    public void LoadOrsaScene()
    {
        SceneManager.LoadScene("OrsaScene");
    }

    // Function to load the Galactic scene (for AR experience)
    public void LoadGalacticScene()
    {
        SceneManager.LoadScene("GalacticScene");
    }
}
