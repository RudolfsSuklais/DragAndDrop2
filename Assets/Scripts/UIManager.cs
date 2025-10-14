using UnityEngine;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject winPanel;
    public GameObject[] stars;
    public Text starCountText;
    public Text timeSpentText; // Optional: UI Text for time spent

    [Header("Win Screen Buttons")]
    public Button restartButton;
    public Button quitButton;

    [Header("Game Reference")]
    public ObjectScript objectScript;

    private bool winShown = false;

    void Start()
    {
        // Hide win panel initially
        if (winPanel != null)
            winPanel.SetActive(false);

        // Hide stars initially
        if (stars != null)
        {
            foreach (var star in stars)
                star.SetActive(false);
        }

        // Clear time text
        if (timeSpentText != null)
            timeSpentText.text = "";

        // Setup button listeners
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    /// <summary>
    /// Displays the win screen.
    /// </summary>
    /// <param name="starsToShow">Number of stars to display (1-3)</param>
    /// <param name="placedCount">Number of vehicles correctly placed</param>
    /// <param name="totalVehicles">Total vehicles in the level</param>
    /// <param name="elapsedTime">Time spent in seconds (optional)</param>
    /// <summary>
    /// Displays the win screen.
    /// </summary>
    /// <param name="starsToShow">Number of stars to display (1-3)</param>
    /// <param name="placedCount">Number of vehicles correctly placed</param>
    /// <param name="totalVehicles">Total vehicles in the level</param>
    /// <param name="elapsedTime">Time spent in seconds (optional)</param>
    public void ShowWinScreenWithStars(int starsToShow, int placedCount, int totalVehicles, float elapsedTime = -1f)
    {
        if (winShown) return; // Prevent multiple triggers
        winShown = true;

        // Calculate stars based on your new criteria
        int calculatedStars = CalculateStars(placedCount, totalVehicles);

        // Use the calculated stars instead of the passed parameter
        starsToShow = calculatedStars;

        // Show win panel
        if (winPanel != null)
            winPanel.SetActive(true);

        // Show stars
        if (stars != null)
        {
            for (int i = 0; i < stars.Length; i++)
                stars[i].SetActive(i < starsToShow);
        }

        // Show vehicle placement info
        if (starCountText != null)
            starCountText.text = $"You placed {placedCount}/{totalVehicles} vehicles correctly!";

        // Show elapsed time
        if (timeSpentText != null && elapsedTime >= 0f)
        {
            TimeSpan time = TimeSpan.FromSeconds(elapsedTime);
            timeSpentText.text = $"Time spent: {time.Minutes:D2}:{time.Seconds:D2}";
        }

        // Enable buttons
        if (restartButton != null)
            restartButton.interactable = true;
        if (quitButton != null)
            quitButton.interactable = true;

        Debug.Log($"Win screen shown: {placedCount}/{totalVehicles} vehicles, {starsToShow} star(s), time={elapsedTime} seconds");
    }

    /// <summary>
    /// Calculates the number of stars based on vehicles placed correctly
    /// </summary>
    /// <param name="placedCount">Number of vehicles placed correctly</param>
    /// <param name="totalVehicles">Total number of vehicles</param>
    /// <returns>Number of stars (1-3)</returns>
    private int CalculateStars(int placedCount, int totalVehicles)
    {
        // Your criteria:
        // 0-5 vehicles placed correctly = 1 star
        // 6-11 vehicles placed correctly = 2 stars  
        // 12 vehicles placed correctly = 3 stars

        if (placedCount >= totalVehicles) // All vehicles placed (12)
        {
            return 3;
        }
        else if (placedCount >= 6) // 6-11 vehicles placed
        {
            return 2;
        }
        else // 0-5 vehicles placed
        {
            return 1;
        }
    }
    /// <summary>
    /// Restarts the current level
    /// </summary>
    public void RestartGame()
    {
        Debug.Log("Restarting game...");

        // Disable buttons to prevent multiple clicks
        if (restartButton != null)
            restartButton.interactable = false;
        if (quitButton != null)
            quitButton.interactable = false;

        // Get the current scene name and reload it
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneName);
    }

    /// <summary>
    /// Quits the game
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quitting game...");

        // Disable buttons to prevent multiple clicks
        if (restartButton != null)
            restartButton.interactable = false;
        if (quitButton != null)
            quitButton.interactable = false;

#if UNITY_EDITOR
        // If running in the editor, stop play mode
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // If running in a build, quit the application
            Application.Quit();
#endif
    }
}