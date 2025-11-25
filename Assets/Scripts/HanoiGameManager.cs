using UnityEngine;

public class HanoiGameManager : MonoBehaviour
{
    public HanoiTower tower1;
    public HanoiTower tower2;
    public HanoiTower tower3;

    public GameObject winPanel;

    private int diskCount;

    void Start()
    {
        winPanel.SetActive(false);

        DiskDragHandler[] disks = FindObjectsOfType<DiskDragHandler>();
        diskCount = disks.Length;

        System.Array.Sort(disks, (a, b) => b.size.CompareTo(a.size));

        foreach (DiskDragHandler disk in disks)
        {
            disk.SetTower(tower1);
            tower1.AddDisk(disk);

            RectTransform rt = disk.GetComponent<RectTransform>();
            rt.position = tower1.GetNextDiskPosition();
        }
    }

    public void CheckWin()
    {
        if (tower3.disks.Count == diskCount)
        {
            winPanel.SetActive(true);
        }
    }
}
