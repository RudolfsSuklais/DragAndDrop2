using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuitApplication : MonoBehaviour
{
    public void QuitGame()
    {
        Debug.Log("Quit button pressed");

#if UNITY_EDITOR
        // Stop play mode in the editor
        EditorApplication.isPlaying = false;
#else
        // Quit the application in the build
        Application.Quit();
#endif
    }
}
