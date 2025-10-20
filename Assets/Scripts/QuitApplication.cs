using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuitApplication : MonoBehaviour
{
    public void QuitGame()
    {
        Debug.Log("Quit button pressed - starting coroutine");
        StartCoroutine(QuitCoroutine());
    }

    private IEnumerator QuitCoroutine()
    {
        Debug.Log("Quit coroutine started");

        // Gaida vienu frame, lai izvairītos no Unity bug
        yield return null;

        Debug.Log("Executing quit command");

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}