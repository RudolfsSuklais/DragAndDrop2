using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ObstaclesControllerScript : MonoBehaviour
{
    [HideInInspector]
    public float speed = 1f;
    public float fadeDuration = 1.5f;
    public float waveAmplitude = 25f;
    public float waveFrequency = 1f;
    private ObjectScript objectScript;
    private ScreenBoundriesScript scrreenBoundriesScript;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private bool isFadingOut = false;
    private bool isExploading = false;
    private Image image;
    private Color originalColor;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        rectTransform = GetComponent<RectTransform>();

        image = GetComponent<Image>();
        originalColor = image.color;
        objectScript = FindFirstObjectByType<ObjectScript>();
        scrreenBoundriesScript = FindFirstObjectByType<ScreenBoundriesScript>();
        StartCoroutine(FadeIn());
    }

    // Update is called once per frame
    void Update()
    {
        float waveOffset = Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;
        rectTransform.anchoredPosition += new Vector2(-speed * Time.deltaTime, waveOffset * Time.deltaTime);

        // <-
        if (speed > 0 && transform.position.x < (scrreenBoundriesScript.minX + 80) && !isFadingOut)
        {
            StartCoroutine(FadeOutAndDestroy());
            isFadingOut = true;
        }

        // ->
        if (speed < 0 && transform.position.x > (scrreenBoundriesScript.maxX - 80) && !isFadingOut)
        {
            StartCoroutine(FadeOutAndDestroy());
            isFadingOut = true;
        }

        if (CompareTag("Bomb") && !isExploading &&
            RectTransformUtility.RectangleContainsScreenPoint(
                rectTransform, Input.mousePosition, Camera.main))
        {
            Debug.Log("The cursor collided with a bomb! (without car)");
            TriggerExplosion();
        }

        if (ObjectScript.drag && !isFadingOut &&
            RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, Camera.main))
        {
            Debug.Log("The cursor collided with a flying object!");

            if (ObjectScript.lastDragged != null)
            {
                // Find the vehicle index before destroying it
                int vehicleIndex = GetVehicleIndex(ObjectScript.lastDragged);

                if (vehicleIndex != -1)
                {
                    Debug.Log($"[Obstacle] Destroying vehicle {ObjectScript.lastDragged.name} at index {vehicleIndex}");
                    StartCoroutine(ShrinkAndDestroy(ObjectScript.lastDragged, 0.5f, vehicleIndex));
                }
                else
                {
                    Debug.LogWarning($"[Obstacle] Could not find vehicle index for {ObjectScript.lastDragged.name}");
                    // Still destroy the object even if index not found
                    StartCoroutine(ShrinkAndDestroy(ObjectScript.lastDragged, 0.5f, -1));
                }

                ObjectScript.lastDragged = null;
                ObjectScript.drag = false;
            }
            else
            {
                Debug.LogWarning("[Obstacle] ObjectScript.lastDragged is null!");
            }

            StartToDestroy();
        }
    }
    public void TriggerExplosion()
    {
        isExploading = true;
        objectScript.effects.PlayOneShot(objectScript.audioCli[6], 5f);

        if (TryGetComponent<Animator>(out Animator animator))
        {
            animator.SetBool("explode", true);
        }

        image.color = Color.red;
        StartCoroutine(RecoverColor(0.4f));

        StartCoroutine(Vibrate());
        StartCoroutine(WaitBeforeExpload());
    }

    IEnumerator WaitBeforeExpload()
    {
        float radius = 0f;
        if (TryGetComponent<CircleCollider2D>(out CircleCollider2D circleCollider))
        {
            radius = circleCollider.radius * transform.lossyScale.x;
        }
        ExploadAndDestroy(radius);
        yield return new WaitForSeconds(1.8f);
        ExploadAndDestroy(radius);
        Destroy(gameObject);
    }

    void ExploadAndDestroy(float radius)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider != null && hitCollider.gameObject != gameObject)
            {
                // Check if it's a vehicle by looking in the objectScript.vehicles array
                int vehicleIndex = GetVehicleIndex(hitCollider.gameObject);
                if (vehicleIndex != -1)
                {
                    // It's a vehicle - destroy it properly
                    objectScript.RemoveVehicle(vehicleIndex);
                    Destroy(hitCollider.gameObject);
                }
                else
                {
                    // It's another obstacle
                    ObstaclesControllerScript obj = hitCollider.gameObject.GetComponent<ObstaclesControllerScript>();
                    if (obj != null && !obj.isExploading)
                    {
                        obj.StartToDestroy();
                    }
                }
            }
        }
    }

    public void StartToDestroy()
    {
        if (!isFadingOut)
        {
            StartCoroutine(FadeOutAndDestroy());
            isFadingOut = true;

            image.color = Color.cyan;
            StartCoroutine(RecoverColor(0.5f));

            objectScript.effects.PlayOneShot(objectScript.audioCli[5]);
            StartCoroutine(Vibrate());
        }
    }

    IEnumerator Vibrate()
    {
        Vector2 originalPosition = rectTransform.anchoredPosition;
        float duration = 0.3f;
        float elpased = 0f;
        float intensity = 5f;

        while (elpased < duration)
        {
            rectTransform.anchoredPosition = originalPosition + Random.insideUnitCircle * intensity;
            elpased += Time.deltaTime;
            yield return null;
        }
        rectTransform.anchoredPosition = originalPosition;
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    IEnumerator FadeOutAndDestroy()
    {
        float t = 0f;
        float startAlpha = canvasGroup.alpha;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        Destroy(gameObject);
    }

    IEnumerator ShrinkAndDestroy(GameObject target, float duration, int vehicleIndex = -1)
    {
        Vector3 orginalScale = target.transform.localScale;
        Quaternion orginalRotation = target.transform.rotation;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            target.transform.localScale = Vector3.Lerp(orginalScale, Vector3.zero, t / duration);
            float angle = Mathf.Lerp(0f, 360f, t / duration);
            target.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            yield return null;
        }

        // Notify ObjectScript about the destroyed vehicle BEFORE destroying it
        if (vehicleIndex != -1 && objectScript != null)
        {
            objectScript.RemoveVehicle(vehicleIndex);
        }

        Destroy(target);
    }

    IEnumerator RecoverColor(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        image.color = originalColor;
    }

    // Helper method to find vehicle index in the objectScript.vehicles array
    private int GetVehicleIndex(GameObject vehicleObj)
    {
        if (objectScript == null || objectScript.vehicles == null) return -1;

        for (int i = 0; i < objectScript.vehicles.Length; i++)
        {
            if (objectScript.vehicles[i] == vehicleObj)
            {
                return i;
            }
        }
        return -1;
    }
}