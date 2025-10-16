using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenu : MonoBehaviour
{
    // This method can be linked to the button OnClick event in the Inspector
    public void GoToSceneZero()
    {
        SceneManager.LoadScene(0);
    }
}
