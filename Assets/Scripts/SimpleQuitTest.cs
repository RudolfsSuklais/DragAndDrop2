using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SimpleQuitTest : MonoBehaviour
{
    public void TestQuit()
    {
        Debug.Log("=== SIMPLE QUIT TEST ===");
        Debug.Log("Time: " + Time.time);
        Debug.Log("Frame: " + Time.frameCount);

        // Force quit immediately
#if UNITY_EDITOR
        Debug.Log("EDITOR: Force quitting...");
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Debug.Log("BUILD: Application.Quit()");
            Application.Quit();
#endif
    }
}