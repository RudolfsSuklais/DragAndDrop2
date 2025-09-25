using UnityEngine;

public class QuitApplication : MonoBehaviour
{
    public void QuitGame()
    {
        Debug.Log("Quit button pressed");



        UnityEditor.EditorApplication.isPlaying = false;

    }

}