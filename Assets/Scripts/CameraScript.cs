using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// ✅ Unified Camera Controller for Android + Editor Testing (supports dragging objects)
public class CameraScript : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float maxZoom = 530f;
    public float minZoom = 150f;
    public float pinchZoomSpeed = 0.9f;
    public float mouseZoomSpeed = 150f;

    [Header("Movement Settings")]
    public float mouseFollowSpeed = 1f;
    public float touchPanSpeed = 1f;
    [Range(0.1f, 1f)] public float dragSensitivity = 0.4f; // ✅ New adjustable sensitivity

    [Header("References")]
    public ScreenBoundriesScript screenBoundries;
    public Camera cam;

    private float startZoom;
    private Vector2 lastTouchPos;
    private int panFingerId = -1;
    private bool isTouchPanning = false;

    private float lastTapTime = 0f;
    public float doubleTapMaxDelay = 0.4f;
    public float doubleTapMaxDistance = 100f;

    private void Awake()
    {
        if (cam == null)
            cam = GetComponent<Camera>();

        if (screenBoundries == null)
            screenBoundries = FindFirstObjectByType<ScreenBoundriesScript>();
    }

    private void Start()
    {
        startZoom = cam.orthographicSize;
        screenBoundries.RecalculateBounds();
        transform.position = screenBoundries.GetClampedCameraPosition(transform.position);
    }

    private void Update()
    {
        if (TransformationScript.isTransforming)
            return;

#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput();
#else
        HandleTouchInput();
#endif

        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        screenBoundries.RecalculateBounds();
        transform.position = screenBoundries.GetClampedCameraPosition(transform.position);
    }

    // ===============================
    // 💻 DESKTOP (Editor) Input
    // ===============================
    private void HandleMouseInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > Mathf.Epsilon)
            cam.orthographicSize -= scroll * mouseZoomSpeed;

        Vector3 mouse = Input.mousePosition;
        if (mouse.x >= 0 && mouse.x <= Screen.width && mouse.y >= 0 && mouse.y <= Screen.height)
        {
            Vector3 screenPoint = new Vector3(mouse.x, mouse.y, cam.nearClipPlane);
            Vector3 targetWorld = cam.ScreenToWorldPoint(screenPoint);
            Vector3 desired = new Vector3(targetWorld.x, targetWorld.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, desired, mouseFollowSpeed * Time.deltaTime);
        }
    }

    // ===============================
    // 📱 ANDROID Input
    // ===============================
    private void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);

            // Let dragging + camera movement coexist
            if (t.phase == TouchPhase.Began)
            {
                float dt = Time.time - lastTapTime;
                if (dt <= doubleTapMaxDelay &&
                    Vector2.Distance(t.position, lastTouchPos) <= doubleTapMaxDistance)
                {
                    StartCoroutine(ResetZoomSmooth());
                    lastTapTime = 0f;
                    lastTouchPos = Vector2.zero;
                }
                else
                {
                    lastTapTime = Time.time;
                    lastTouchPos = t.position;
                    panFingerId = t.fingerId;
                    isTouchPanning = true;
                }
            }
            else if (t.phase == TouchPhase.Moved && isTouchPanning && t.fingerId == panFingerId)
            {
                Vector2 delta = t.position - lastTouchPos;

                // ✅ Move in same direction as finger, not inverted
                Vector3 move = ScreenDeltaToWorldDelta(delta) * touchPanSpeed;

                // ✅ Optional smooth motion
                transform.position = Vector3.Lerp(transform.position, transform.position + move, 0.5f);

                transform.position = screenBoundries.GetClampedCameraPosition(transform.position);
                lastTouchPos = t.position;
            }
            else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
            {
                isTouchPanning = false;
                panFingerId = -1;
            }
        }
        else if (Input.touchCount == 2)
        {
            HandlePinch();
        }
    }

    // ===============================
    // 🔍 PINCH ZOOM
    // ===============================
    private void HandlePinch()
    {
        Touch t0 = Input.GetTouch(0);
        Touch t1 = Input.GetTouch(1);

        float prevDist = (t0.position - t0.deltaPosition - (t1.position - t1.deltaPosition)).magnitude;
        float currDist = (t0.position - t1.position).magnitude;

        cam.orthographicSize += (prevDist - currDist) * pinchZoomSpeed;
    }

    // ===============================
    // ⚙️ Utilities
    // ===============================
    private Vector3 ScreenDeltaToWorldDelta(Vector2 delta)
    {
        float worldPerPixel = (cam.orthographicSize * 2f) / Screen.height;
        return new Vector3(delta.x * worldPerPixel * dragSensitivity, delta.y * worldPerPixel * dragSensitivity, 0f);
    }

    private IEnumerator ResetZoomSmooth()
    {
        float duration = 0.25f;
        float elapsed = 0f;
        float initialZoom = cam.orthographicSize;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cam.orthographicSize = Mathf.Lerp(initialZoom, startZoom, elapsed / duration);
            screenBoundries.RecalculateBounds();
            transform.position = screenBoundries.GetClampedCameraPosition(transform.position);
            yield return null;
        }

        cam.orthographicSize = startZoom;
        screenBoundries.RecalculateBounds();
        transform.position = screenBoundries.GetClampedCameraPosition(transform.position);
    }
}
