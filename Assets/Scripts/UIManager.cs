using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject winPanel;  // Assign WinPanel UI here in Inspector

    void Start()
    {
        if (winPanel != null)
            winPanel.SetActive(false);  // Hide at start
    }

    public void ShowWinScreen()
    {
        Debug.Log("ShowWinScreen called!");
        if (winPanel != null)
            winPanel.SetActive(true);
    }
}
