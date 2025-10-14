using UnityEngine;
using UnityEngine.EventSystems;

public class DropPlaceScript : MonoBehaviour, IDropHandler
{
    [Header("References")]
    public ObjectScript objScript;
    public UIManager uiManager;

    // Rotation and size thresholds
    private const float RotationThreshold = 15f;
    private const float SizeThreshold = 0.15f;

    private void Awake()
    {
        // Auto-assign references if not set
        if (uiManager == null) uiManager = FindObjectOfType<UIManager>();
        if (objScript == null) objScript = FindObjectOfType<ObjectScript>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
        {
            Debug.LogWarning("Dropped object missing or destroyed.");
            return;
        }

        // Check if tags match
        if (!eventData.pointerDrag.CompareTag(tag))
        {
            HandleIncorrectPlacement(eventData);
            return;
        }

        RectTransform draggedRect = eventData.pointerDrag.GetComponent<RectTransform>();
        RectTransform placeRect = GetComponent<RectTransform>();

        float rotDiff = Mathf.Abs(draggedRect.eulerAngles.z - placeRect.eulerAngles.z);
        Vector3 scaleDiff = draggedRect.localScale - placeRect.localScale;
        float xSizeDiff = Mathf.Abs(scaleDiff.x);
        float ySizeDiff = Mathf.Abs(scaleDiff.y);

        Debug.Log($"Rotation diff: {rotDiff}, X diff: {xSizeDiff}, Y diff: {ySizeDiff}");

        // Correct placement if within thresholds
        if ((rotDiff <= RotationThreshold || rotDiff >= 360 - RotationThreshold) &&
            xSizeDiff <= SizeThreshold && ySizeDiff <= SizeThreshold)
        {
            HandleCorrectPlacement(eventData, draggedRect, placeRect);
        }
        else
        {
            HandleIncorrectPlacement(eventData);
        }
    }

    private void HandleCorrectPlacement(PointerEventData eventData, RectTransform draggedRect, RectTransform placeRect)
    {
        Debug.Log("Correct place");

        objScript.rightPlace = true;

        // Snap position, rotation, scale
        draggedRect.anchoredPosition = placeRect.anchoredPosition;
        draggedRect.localRotation = placeRect.localRotation;
        draggedRect.localScale = placeRect.localScale;

        int index = GetVehicleIndexByGameObject(eventData.pointerDrag);
        if (index != -1)
        {
            objScript.placedCorrectly[index] = true;

            Debug.Log($"✅ VEHICLE PLACED: {eventData.pointerDrag.name} at index {index}");

            // Show remaining count
            int remaining = objScript.GetRemainingToPlaceCount();
            int total = objScript.GetTotalVehiclesCount();
            int placed = objScript.GetCorrectlyPlacedCount();
            Debug.Log($"📊 AFTER PLACEMENT: {placed}/{total} placed, {remaining} remaining to be placed");

            CanvasGroup cg = eventData.pointerDrag.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.blocksRaycasts = false;
                cg.alpha = 0.5f;
            }

            PlayPlacementSound(eventData.pointerDrag.tag);
        }

        CheckWinCondition();
    }
    private void HandleIncorrectPlacement(PointerEventData eventData)
    {
        objScript.rightPlace = false;
        objScript.effects.PlayOneShot(objScript.audioCli[1]);

        int index = GetVehicleIndexByGameObject(eventData.pointerDrag);
        if (index != -1 && objScript.vehicles[index] != null)
        {
            RectTransform rect = objScript.vehicles[index].GetComponent<RectTransform>();
            rect.localPosition = objScript.startCoordinates[index];
            objScript.placedCorrectly[index] = false;
        }
        else
        {
            Debug.LogWarning("Vehicle not found in array.");
        }
    }

    private int GetVehicleIndexByGameObject(GameObject vehicleObj)
    {
        for (int i = 0; i < objScript.vehicles.Length; i++)
        {
            if (objScript.vehicles[i] == vehicleObj)
                return i;
        }
        return -1;
    }

    private void PlayPlacementSound(string tag)
    {
        switch (tag)
        {
            case "Garbage": objScript.effects.PlayOneShot(objScript.audioCli[2]); break;
            case "Medicine": objScript.effects.PlayOneShot(objScript.audioCli[3]); break;
            case "Fire": objScript.effects.PlayOneShot(objScript.audioCli[4]); break;
            case "Bus": objScript.effects.PlayOneShot(objScript.audioCli[7]); break;
            case "b2": objScript.effects.PlayOneShot(objScript.audioCli[8]); break;
            case "Cement": objScript.effects.PlayOneShot(objScript.audioCli[9]); break;
            case "e46": objScript.effects.PlayOneShot(objScript.audioCli[7]); break;
            case "e61": objScript.effects.PlayOneShot(objScript.audioCli[8]); break;
            case "eskavators": objScript.effects.PlayOneShot(objScript.audioCli[9]); break;
            case "police": objScript.effects.PlayOneShot(objScript.audioCli[7]); break;
            case "tractor1": objScript.effects.PlayOneShot(objScript.audioCli[8]); break;
            case "tractor2": objScript.effects.PlayOneShot(objScript.audioCli[9]); break;
   
            default: Debug.Log("Unknown tag detected"); break;
        }
    }

    public void CheckWinCondition()
    {
        Debug.Log("=== CHECK WIN CONDITION TRIGGERED ===");

        int totalVehicles = objScript.GetTotalVehiclesCount();
        int placedCount = objScript.GetCorrectlyPlacedCount();
        int activeAndUnplacedCount = 0;

        // Detailed debug for each vehicle - SAFELY check destroyed objects
        Debug.Log($"Total vehicles: {totalVehicles}");
        for (int i = 0; i < totalVehicles; i++)
        {
            // Safe check for destroyed objects
            bool isDestroyed = objScript.vehicles[i] == null;
            bool isActive = !isDestroyed && objScript.vehicles[i].activeInHierarchy;
            bool isPlaced = objScript.placedCorrectly[i];

            string status = isPlaced ? "PLACED" : (isActive ? "ACTIVE" : "DESTROYED");
            string name = isDestroyed ? "DESTROYED" : objScript.vehicles[i].name;

            Debug.Log($"Vehicle {i}: {status} - {name}");

            if (isActive && !isPlaced)
            {
                activeAndUnplacedCount++;
            }
        }

        Debug.Log($"[CheckWin] placedCount={placedCount}, activeAndUnplacedCount={activeAndUnplacedCount}, totalVehicles={totalVehicles}");

        // NEW: Show remaining vehicles count clearly
        int remainingVehicles = totalVehicles - placedCount;
        Debug.Log($"📊 VEHICLE STATUS: {placedCount} placed correctly, {activeAndUnplacedCount} active but unplaced, {remainingVehicles} remaining to be placed/destroyed");

        // Win condition: No active vehicles that are not placed correctly
        if (activeAndUnplacedCount == 0)
        {
            Debug.Log("🎉 WIN CONDITION MET! Showing win screen...");

            // EXTENSIVE UIManager debugging
            if (uiManager != null)
            {
                Debug.Log("✅ UIManager reference is NOT null");

                // Check if winPanel is assigned
                if (uiManager.winPanel != null)
                {
                    Debug.Log("✅ WinPanel reference is NOT null");
                }
                else
                {
                    Debug.LogError("❌ WinPanel reference is NULL in UIManager!");
                }

                float percentage = (float)placedCount / totalVehicles;
                int starsToShow = 1;
                if (percentage >= 0.9f) starsToShow = 3;
                else if (percentage >= 0.6f) starsToShow = 2;

                float elapsedTime = objScript.GetElapsedTime();

                Debug.Log($"📊 Calling ShowWinScreenWithStars: stars={starsToShow}, placed={placedCount}, total={totalVehicles}, time={elapsedTime}");

                uiManager.ShowWinScreenWithStars(starsToShow, placedCount, totalVehicles, elapsedTime);

                Debug.Log("✅ ShowWinScreenWithStars method called successfully");
            }
            else
            {
                Debug.LogError("❌ UIManager reference is NULL in DropPlaceScript!");

                // Try to find it dynamically
                uiManager = FindFirstObjectByType<UIManager>();
                if (uiManager != null)
                {
                    Debug.Log("✅ Found UIManager dynamically, calling win screen...");
                    float percentage = (float)placedCount / totalVehicles;
                    int starsToShow = 1;
                    if (percentage >= 0.9f) starsToShow = 3;
                    else if (percentage >= 0.6f) starsToShow = 2;
                    float elapsedTime = objScript.GetElapsedTime();
                    uiManager.ShowWinScreenWithStars(starsToShow, placedCount, totalVehicles, elapsedTime);
                }
                else
                {
                    Debug.LogError("❌ Could not find UIManager in scene!");
                }
            }
        }
        else
        {
            Debug.Log($"[CheckWin] Still {activeAndUnplacedCount} vehicles remaining to place or destroy");
        }
    }
}