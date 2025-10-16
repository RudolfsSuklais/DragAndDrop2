using System;
using UnityEngine;
using System.Collections;

public class ObjectScript : MonoBehaviour
{
    [Header("Vehicles & Shadows")]
    public GameObject[] vehicles;
    public Transform[] spawnPoints;

    public GameObject[] shadows;
    public Transform[] shadowSpawnPoints;

    [HideInInspector] public Vector2[] startCoordinates;
    [HideInInspector] public Vector2[] shadowStartCoordinates;

    [Header("Audio & Canvas")]
    public Canvas can;
    public AudioSource effects;
    public AudioClip[] audioCli;

    [HideInInspector] public bool rightPlace = false;
    public static GameObject lastDragged = null;
    public static bool drag = false;
    [HideInInspector] public bool[] placedCorrectly;

    [Header("UI Reference")]
    public UIManager uiManager;
    public DropPlaceScript dropPlaceScript; // reference to DropPlaceScript

    [Header("Time Tracking")]
    private float levelStartTime;

    void Awake()
    {
        // Auto-find references if not set in inspector
        if (dropPlaceScript == null)
        {
            dropPlaceScript = FindFirstObjectByType<DropPlaceScript>();
            if (dropPlaceScript != null)
            {
                Debug.Log("✅ Auto-assigned dropPlaceScript reference");
            }
            else
            {
                Debug.LogError("❌ Could not find DropPlaceScript in scene!");
            }
        }

        if (uiManager == null)
        {
            uiManager = FindFirstObjectByType<UIManager>();
            if (uiManager != null)
            {
                Debug.Log("✅ Auto-assigned uiManager reference");
            }
            else
            {
                Debug.LogError("❌ Could not find UIManager in scene!");
            }
        }
    }

    void Start()
    {
        levelStartTime = Time.time;

        placedCorrectly = new bool[vehicles.Length];
        startCoordinates = new Vector2[vehicles.Length];
        shadowStartCoordinates = new Vector2[shadows.Length];

        ShuffleAndPlaceVehicles();
        ShuffleAndPlaceShadows();

        Debug.Log($"🎮 Game initialized with {vehicles.Length} vehicles");
    }

    private void ShuffleAndPlaceVehicles()
    {
        Transform[] shuffledSpawnPoints = new Transform[spawnPoints.Length];
        spawnPoints.CopyTo(shuffledSpawnPoints, 0);
        ShuffleArray(shuffledSpawnPoints);

        for (int i = 0; i < vehicles.Length; i++)
        {
            if (vehicles[i] == null) continue;

            RectTransform rectTransform = vehicles[i].GetComponent<RectTransform>();

            Vector3 spawnPos = shuffledSpawnPoints[i % shuffledSpawnPoints.Length].position;
            rectTransform.position = spawnPos;

            // Save the start coordinates (localPosition)
            startCoordinates[i] = rectTransform.localPosition;
            placedCorrectly[i] = false; // ensure initial state

            // Apply random rotation around Z axis between 0 and 360 degrees
            float randomRotation = UnityEngine.Random.Range(0f, 360f);
            rectTransform.localRotation = Quaternion.Euler(0f, 0f, randomRotation);

            // Apply random scale between 0.3 and 0.9 (same scale for x and y for uniform scaling)
            float randomScale = UnityEngine.Random.Range(0.3f, 0.9f);
            rectTransform.localScale = new Vector3(randomScale, randomScale, 1f);
        }
    }


    private void ShuffleAndPlaceShadows()
    {
        Transform[] shuffledShadowSpawnPoints = new Transform[shadowSpawnPoints.Length];
        shadowSpawnPoints.CopyTo(shuffledShadowSpawnPoints, 0);
        ShuffleArray(shuffledShadowSpawnPoints);

        for (int i = 0; i < shadows.Length; i++)
        {
            if (shadows[i] == null) continue;

            RectTransform rectTransform = shadows[i].GetComponent<RectTransform>();

            Vector3 shadowSpawnPos = shuffledShadowSpawnPoints[i % shuffledShadowSpawnPoints.Length].position;
            rectTransform.position = shadowSpawnPos;

            shadowStartCoordinates[i] = rectTransform.localPosition;

            // Apply random rotation around Z axis between 0 and 360 degrees
            float randomRotation = UnityEngine.Random.Range(0f, 360f);
            rectTransform.localRotation = Quaternion.Euler(0f, 0f, randomRotation);

            // Apply random uniform scale between 0.3 and 0.9 for both x and y
            float randomScale = UnityEngine.Random.Range(0.3f, 0.9f);
            rectTransform.localScale = new Vector3(randomScale, randomScale, 1f);
        }
    }


    void ShuffleArray<T>(T[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int randIndex = UnityEngine.Random.Range(i, array.Length);
            T temp = array[i];
            array[i] = array[randIndex];
            array[randIndex] = temp;
        }
    }

    public int GetTotalVehiclesCount() => vehicles.Length;

    public int GetCorrectlyPlacedCount()
    {
        int count = 0;
        for (int i = 0; i < vehicles.Length; i++)
        {
            // Safe check for destroyed vehicles
            if (vehicles[i] != null && placedCorrectly[i])
                count++;
        }
        return count;
    }

    public int GetRemainingToPlaceCount()
    {
        int remaining = 0;
        for (int i = 0; i < vehicles.Length; i++)
        {
            // Safe check for destroyed vehicles
            if (vehicles[i] != null && vehicles[i].activeInHierarchy && !placedCorrectly[i])
                remaining++;
        }
        return remaining;
    }

    public bool AreAllVehiclesPlaced() => GetRemainingToPlaceCount() == 0;

    public float GetElapsedTime() => Time.time - levelStartTime;

    // Call this when a vehicle is destroyed or removed
    public void RemoveVehicle(int index)
    {
        if (index < 0 || index >= vehicles.Length || vehicles[index] == null)
        {
            Debug.LogWarning($"[RemoveVehicle] Invalid index {index} or vehicle is null");
            return;
        }

        string vehicleName = vehicles[index].name;

        vehicles[index].SetActive(false);
        placedCorrectly[index] = false;

        Debug.Log($"🔥 VEHICLE DESTROYED: {vehicleName} at index {index}");

        // Calculate remaining before calling win check
        int remainingBefore = GetRemainingToPlaceCount();
        int total = GetTotalVehiclesCount();
        int placed = GetCorrectlyPlacedCount();

        Debug.Log($"📊 AFTER DESTRUCTION: {placed}/{total} placed, {remainingBefore} remaining to be placed");

        // ✅ Call CheckWinCondition on the next frame so Unity updates object states first
        StartCoroutine(DelayedCheckWin());
    }

    // Add this helper method to find vehicle by GameObject
    public int GetVehicleIndex(GameObject vehicleObj)
    {
        for (int i = 0; i < vehicles.Length; i++)
        {
            if (vehicles[i] == vehicleObj)
                return i;
        }
        return -1;
    }

    private IEnumerator DelayedCheckWin()
    {
        yield return null; // wait one frame

        // If dropPlaceScript is still null, try to find it one more time
        if (dropPlaceScript == null)
        {
            dropPlaceScript = FindFirstObjectByType<DropPlaceScript>();
            if (dropPlaceScript == null)
            {
                Debug.LogError("❌ DropPlaceScript reference missing and cannot be found!");

                // Try alternative approach - find UIManager and trigger win directly
                if (uiManager == null)
                {
                    uiManager = FindFirstObjectByType<UIManager>();
                }

                if (uiManager != null)
                {
                    Debug.Log("🔄 Using UIManager directly for win condition check");
                    TriggerWinConditionDirectly();
                }
                yield break;
            }
        }

        Debug.Log("🔄 DelayedCheckWin calling CheckWinCondition");
        dropPlaceScript.CheckWinCondition();
    }

    private void TriggerWinConditionDirectly()
    {
        int totalVehicles = GetTotalVehiclesCount();
        int placedCount = GetCorrectlyPlacedCount();
        int remaining = GetRemainingToPlaceCount();

        Debug.Log($"🎯 Direct Win Check: {placedCount}/{totalVehicles} placed, {remaining} remaining");

        if (remaining == 0 && uiManager != null)
        {
            Debug.Log("🎉 DIRECT WIN CONDITION MET! Showing win screen...");

            float percentage = (float)placedCount / totalVehicles;
            int starsToShow = 1;
            if (percentage >= 0.9f) starsToShow = 3;
            else if (percentage >= 0.6f) starsToShow = 2;

            float elapsedTime = GetElapsedTime();
            uiManager.ShowWinScreenWithStars(starsToShow, placedCount, totalVehicles, elapsedTime);
        }
    }
}