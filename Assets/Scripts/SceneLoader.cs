using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadSceneByName(string sceneName)
    {
        Debug.Log($"Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneByIndex(int sceneIndex)
    {
        Debug.Log($"Loading scene index: {sceneIndex}");
        SceneManager.LoadScene(sceneIndex);
    }

    public void LoadCarGame()
    {
        LoadSceneByName("Car Game");
    }

    public void LoadMainMenu()
    {
        LoadSceneByName("MainMenu");
    }

    public void RestartCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        LoadSceneByName(currentSceneName);
    }

    // QuitGame metode ir izdzēsta - izmantosim tikai QuitApplication
}