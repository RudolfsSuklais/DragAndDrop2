using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [Header("Camera Settings")]
    public float panSpeed = 6f;

    // Fixed zoom values - not visible in inspector
    private const float MAX_ZOOM = 530f;
    private const float MIN_ZOOM = 300f;

    Vector3 bottomLeft, topRight;
    float cameraMaxX, cameraMinX, cameraMaxY, cameraMinY, x, y;
    public Camera cam;
    private UIManager uiManager;

    // Public properties to access the values
    public float MaxZoom => MAX_ZOOM;
    public float MinZoom => MIN_ZOOM;

    void Start()
    {
        cam = GetComponent<Camera>();
        topRight = cam.ScreenToWorldPoint(
            new Vector3(cam.pixelWidth, cam.pixelHeight, -transform.position.z));
        bottomLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, -transform.position.z));
        cameraMaxX = topRight.x;
        cameraMinX = bottomLeft.x;
        cameraMaxY = topRight.y;
        cameraMinY = bottomLeft.y;

        uiManager = FindFirstObjectByType<UIManager>();

        // Ensure camera starts within zoom bounds
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, MIN_ZOOM, MAX_ZOOM);
    }

    void Update()
    {
        if (IsWinScreenActive()) return;

        x = Input.GetAxis("Mouse X") * panSpeed;
        y = Input.GetAxis("Mouse Y") * panSpeed;
        transform.Translate(x, y, 0);

        if ((Input.GetAxis("Mouse ScrollWheel") > 0) && cam.orthographicSize > MIN_ZOOM)
        {
            cam.orthographicSize = cam.orthographicSize - 50f;
        }

        if ((Input.GetAxis("Mouse ScrollWheel") < 0) && cam.orthographicSize < MAX_ZOOM)
        {
            cam.orthographicSize = cam.orthographicSize + 50f;
        }

        // Ensure zoom stays within bounds
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, MIN_ZOOM, MAX_ZOOM);

        // ... rest of your existing code
        topRight = cam.ScreenToWorldPoint(
            new Vector3(cam.pixelWidth, cam.pixelHeight, -transform.position.z));
        bottomLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, -transform.position.z));

        if (topRight.x > cameraMaxX)
        {
            transform.position = new Vector3(
             transform.position.x - (topRight.x - cameraMaxX), transform.position.y, transform.position.z);
        }

        if (topRight.y > cameraMaxY)
        {
            transform.position = new Vector3(
             transform.position.x, transform.position.y - (topRight.y - cameraMaxY), transform.position.z);
        }

        if (bottomLeft.x < cameraMinX)
        {
            transform.position = new Vector3(
             transform.position.x + (cameraMinX - bottomLeft.x), transform.position.y, transform.position.z);
        }

        if (bottomLeft.y < cameraMinY)
        {
            transform.position = new Vector3(
             transform.position.x, transform.position.y + (cameraMinY - bottomLeft.y), transform.position.z);
        }
    }

    private bool IsWinScreenActive()
    {
        if (uiManager != null && uiManager.winPanel != null)
        {
            bool isActive = uiManager.winPanel.activeInHierarchy;
            if (isActive) this.enabled = false;
            return isActive;
        }

        if (uiManager == null) uiManager = FindFirstObjectByType<UIManager>();
        return false;
    }
}