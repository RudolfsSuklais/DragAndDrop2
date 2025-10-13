using UnityEngine;
using UnityEngine.UI; // Needed for UI elements like Text, Image, etc.

public class UIManager : MonoBehaviour
{
    public GameObject winPanel;  // Assign WinPanel UI here in Inspector
    public GameObject[] stars;   // Assign 3 star GameObjects (or images) in Inspector
    public Text starCountText;   // Optional: To show "You placed X vehicles!" message

    void Start()
    {
        if (winPanel != null)
            winPanel.SetActive(false);  // Hide at start

        // Hide all stars initially
        if (stars != null)
        {
            foreach (var star in stars)
            {
                star.SetActive(false);
            }
        }
    }

    // Old method if you want to keep it:
    public void ShowWinScreen()
    {
        Debug.Log("ShowWinScreen called!");
        if (winPanel != null)
            winPanel.SetActive(true);
    }

    // New method to show win screen with stars based on placed vehicles count
    public void ShowWinScreenWithStars(int placedCount)
    {
        Debug.Log($"ShowWinScreenWithStars called with {placedCount} stars.");

        if (winPanel != null)
            winPanel.SetActive(true);

        // Safety check
        if (stars == null || stars.Length < 3)
        {
            Debug.LogWarning("Stars array not set or has less than 3 stars assigned.");
            return;
        }

        // Enable stars based on count (1, 2, or 3)
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].SetActive(i < placedCount);
        }

        // Optional: update text message
        if (starCountText != null)
        {
            starCountText.text = $"You placed {placedCount} vehicle{(placedCount > 1 ? "s" : "")}!";
        }
    }
}
