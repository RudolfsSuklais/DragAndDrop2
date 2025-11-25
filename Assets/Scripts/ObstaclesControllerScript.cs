using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ObstaclesControllerScript : MonoBehaviour
{
    [HideInInspector] public float speed = 1f;

    public float fadeDuration = 1.5f;
    public float waveAmplitude = 25f;
    public float waveFrequency = 1f;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private ObjectScript objectScript;
    private ScreenBoundriesScript screenBoundries;

    private bool isFadingOut = false;
    private bool isExploding = false;

    private Image image;
    private Color originalColor;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        image = GetComponent<Image>();
        originalColor = image.color;

        objectScript = FindFirstObjectByType<ObjectScript>();
        screenBoundries = FindFirstObjectByType<ScreenBoundriesScript>();

        StartCoroutine(FadeIn());
    }

    void Update()
    {
        MoveWave();

        if (!isFadingOut && !IsVisibleToCamera(gameObject))
        {
            StartCoroutine(FadeOutAndDestroy());
            isFadingOut = true;
        }

        HandleBombHover();
        HandleVehicleCollision();
    }

    // -------------------------
    // MOVEMENT
    // -------------------------
    void MoveWave()
    {
        float waveOffset = Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;
        rectTransform.anchoredPosition += new Vector2(-speed * Time.deltaTime, waveOffset * Time.deltaTime);
    }

    // -------------------------
    // BOMB HOVER TRIGGER
    // -------------------------
    void HandleBombHover()
    {
        if (CompareTag("Bomb") && !isExploding &&
            RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, Camera.main))
        {
            TriggerExplosion();
        }
    }

    // -------------------------
    // VEHICLE COLLISION
    // -------------------------
    void HandleVehicleCollision()
    {
        if (!ObjectScript.drag || isFadingOut)
            return;

        if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, Camera.main))
        {
            if (ObjectScript.lastDragged != null)
            {
                int vehicleIndex = GetVehicleIndex(ObjectScript.lastDragged);

                StartCoroutine(ShrinkAndDestroy(ObjectScript.lastDragged, 0.5f, vehicleIndex));

                ObjectScript.lastDragged = null;
                ObjectScript.drag = false;
            }

            StartToDestroy();
        }
    }

    // -------------------------
    // VISIBILITY CHECK
    // -------------------------
    bool IsVisibleToCamera(GameObject obj)
    {
        Vector3 viewport = Camera.main.WorldToViewportPoint(obj.transform.position);
        return viewport.x > 0 && viewport.x < 1 &&
               viewport.y > 0 && viewport.y < 1 &&
               viewport.z > 0;
    }

    // -------------------------
    // EXPLOSION LOGIC
    // -------------------------
    public void TriggerExplosion()
    {
        isExploding = true;
        objectScript.effects.PlayOneShot(objectScript.audioCli[6], 5f);

        if (TryGetComponent(out Animator animator))
            animator.SetBool("explode", true);

        image.color = Color.red;
        StartCoroutine(RecoverColor(0.4f));

        StartCoroutine(Vibrate());
        StartCoroutine(WaitBeforeExplode());
    }

    IEnumerator WaitBeforeExplode()
    {
        float radius = 0f;

        if (TryGetComponent(out CircleCollider2D circle))
            radius = circle.radius * transform.lossyScale.x;

        ExplodeAndDestroyNearby(radius);
        yield return new WaitForSeconds(1.8f);
        ExplodeAndDestroyNearby(radius);

        Destroy(gameObject);
    }

    void ExplodeAndDestroyNearby(float radius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            int vehicleIndex = GetVehicleIndex(hit.gameObject);

            if (vehicleIndex != -1)
            {
                objectScript.RemoveVehicle(vehicleIndex);
                Destroy(hit.gameObject);
            }
        }
    }

    // -------------------------
    // NORMAL DESTROY
    // -------------------------
    public void StartToDestroy()
    {
        if (isFadingOut) return;

        isFadingOut = true;

        image.color = Color.cyan;
        StartCoroutine(RecoverColor(0.5f));

        objectScript.effects.PlayOneShot(objectScript.audioCli[5]);
        StartCoroutine(Vibrate());
        StartCoroutine(FadeOutAndDestroy());
    }

    // -------------------------
    // EFFECTS
    // -------------------------
    IEnumerator FadeIn()
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    IEnumerator FadeOutAndDestroy()
    {
        float t = 0;
        float startAlpha = canvasGroup.alpha;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, t / fadeDuration);
            yield return null;
        }

        Destroy(gameObject);
    }

    IEnumerator Vibrate()
    {
        Vector2 originalPos = rectTransform.anchoredPosition;
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            rectTransform.anchoredPosition = originalPos + Random.insideUnitCircle * 5f;
            elapsed += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = originalPos;
    }

    IEnumerator RecoverColor(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        image.color = originalColor;
    }

    IEnumerator ShrinkAndDestroy(GameObject target, float duration, int vehicleIndex)
    {
        Vector3 startScale = target.transform.localScale;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            target.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t / duration);
            yield return null;
        }

        if (vehicleIndex != -1)
            objectScript.RemoveVehicle(vehicleIndex);

        Destroy(target);
    }

    int GetVehicleIndex(GameObject obj)
    {
        if (objectScript == null || objectScript.vehicles == null) return -1;
        for (int i = 0; i < objectScript.vehicles.Length; i++)
            if (objectScript.vehicles[i] == obj)
                return i;
        return -1;
    }
}
