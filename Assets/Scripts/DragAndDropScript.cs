using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// ✅ Drag-and-drop handler compatible with camera panning
public class DragAndDropScript : MonoBehaviour, IPointerDownHandler, IBeginDragHandler,
    IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGro;
    private RectTransform rectTra;
    public ObjectScript objectScr;
    public ScreenBoundriesScript screenBou;

    private Vector3 dragOffsetWorld;
    private Camera uiCamera;
    private Canvas canvas;

    void Awake()
    {
        canvasGro = GetComponent<CanvasGroup>();
        rectTra = GetComponent<RectTransform>();

        if (objectScr == null)
            objectScr = Object.FindFirstObjectByType<ObjectScript>();

        if (screenBou == null)
            screenBou = Object.FindFirstObjectByType<ScreenBoundriesScript>();

        canvas = GetComponentInParent<Canvas>();

        if (canvas == null)
        {
            Debug.LogError("Canvas not found for DragAndDropScript");
        }
        else
        {
            // ✅ Handle both Overlay and Camera canvases
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                uiCamera = null;
            else
                uiCamera = canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        objectScr.effects.PlayOneShot(objectScr.audioCli[0]);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        ObjectScript.drag = true;
        ObjectScript.lastDragged = eventData.pointerDrag;
        canvasGro.blocksRaycasts = false;
        canvasGro.alpha = 0.6f;

        // Calculate drag offset
        Vector3 pointerWorld;
        if (ScreenPointToWorld(eventData.position, out pointerWorld))
            dragOffsetWorld = transform.position - pointerWorld;
        else
            dragOffsetWorld = Vector3.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 pointerWorld;
        if (!ScreenPointToWorld(eventData.position, out pointerWorld))
            return;

        // ✅ Correct world follow (no inversion)
        Vector3 desiredPosition = pointerWorld + dragOffsetWorld;
        desiredPosition.z = transform.position.z;

        screenBou.RecalculateBounds();
        Vector2 clamped = screenBou.GetClampedPosition(desiredPosition);
        transform.position = new Vector3(clamped.x, clamped.y, desiredPosition.z);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        objectScr.effects.PlayOneShot(objectScr.audioCli[0]);
        ObjectScript.drag = false;
        canvasGro.blocksRaycasts = true;
        canvasGro.alpha = 1.0f;

        if (objectScr.rightPlace)
        {
            canvasGro.blocksRaycasts = false;
            ObjectScript.lastDragged = null;
        }
    }

    private bool ScreenPointToWorld(Vector2 screenPoint, out Vector3 worldPoint)
    {
        if (rectTra == null)
        {
            worldPoint = Vector3.zero;
            return false;
        }

        // ✅ Works for both Overlay and Camera canvas
        return RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTra, screenPoint, uiCamera, out worldPoint);
    }
}
