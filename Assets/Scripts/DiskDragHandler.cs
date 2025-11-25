using UnityEngine;
using UnityEngine.EventSystems;

public class DiskDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int size;

    private Canvas canvas;
    private RectTransform rect;
    private CanvasGroup group;

    private HanoiTower currentTower;
    private HanoiGameManager manager;

    private Vector2 dragOffset;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        group = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        manager = FindFirstObjectByType<HanoiGameManager>();
    }

    public void SetTower(HanoiTower tower)
    {
        currentTower = tower;
    }

    public void OnBeginDrag(PointerEventData e)
    {
        if (currentTower == null) return;
        if (currentTower.GetTopDisk() != this) return;

        group.blocksRaycasts = false;
        group.alpha = 0.6f;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect, e.position, e.pressEventCamera, out dragOffset
        );
    }

    public void OnDrag(PointerEventData e)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect.parent as RectTransform,
            e.position,
            e.pressEventCamera,
            out pos
        );

        rect.anchoredPosition = pos;
    }

    public void OnEndDrag(PointerEventData e)
    {
        group.blocksRaycasts = true;
        group.alpha = 1;

        GameObject hit = e.pointerEnter;
        if (hit == null)
        {
            SnapBack();
            return;
        }

        HanoiTower targetTower = hit.GetComponent<HanoiTower>();
        if (targetTower == null)
        {
            SnapBack();
            return;
        }

        if (!targetTower.CanPlaceDisk(this))
        {
            SnapBack();
            return;
        }

        currentTower.RemoveDisk(this);
        targetTower.AddDisk(this);

        currentTower = targetTower;

        StopAllCoroutines();
        StartCoroutine(SmoothSnap(targetTower.GetNextDiskPosition()));

        manager.CheckWin();
    }

    private void SnapBack()
    {
        StopAllCoroutines();
        StartCoroutine(SmoothSnap(currentTower.GetNextDiskPosition()));
    }

    private System.Collections.IEnumerator SmoothSnap(Vector3 target)
    {
        float t = 0f;
        Vector3 startPos = rect.position;

        while (t < 1f)
        {
            t += Time.deltaTime * 7f;
            rect.position = Vector3.Lerp(startPos, target, t);
            yield return null;
        }
        rect.position = target;
    }
}
