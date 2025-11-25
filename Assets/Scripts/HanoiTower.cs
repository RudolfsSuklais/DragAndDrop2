using System.Collections.Generic;
using UnityEngine;

public class HanoiTower : MonoBehaviour
{
    public RectTransform dropArea;
    public float diskHeight = 50f;

    public Stack<DiskDragHandler> disks = new Stack<DiskDragHandler>();

    public Vector3 GetNextDiskPosition()
    {
        Vector3 pos = dropArea.position;
        pos.y -= disks.Count * diskHeight;
        return pos;
    }

    public DiskDragHandler GetTopDisk()
    {
        return disks.Count > 0 ? disks.Peek() : null;
    }

    public bool CanPlaceDisk(DiskDragHandler disk)
    {
        if (disks.Count == 0) return true;
        return disk.size < disks.Peek().size;
    }

    public void AddDisk(DiskDragHandler disk)
    {
        disks.Push(disk);
    }

    public void RemoveDisk(DiskDragHandler disk)
    {
        if (disks.Count > 0 && disks.Peek() == disk)
            disks.Pop();
    }
}
