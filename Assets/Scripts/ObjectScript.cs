using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScript : MonoBehaviour
{
    public GameObject[] vehicles;
    public Transform[] spawnPoints;

    public GameObject[] shadows;
    public Transform[] shadowSpawnPoints;

    [HideInInspector]
    public Vector2[] startCoordinates;
    [HideInInspector]
    public Vector2[] shadowStartCoordinates;

    public Canvas can;
    public AudioSource effects;
    public AudioClip[] audioCli;
    [HideInInspector]
    public bool rightPlace = false;
    public static GameObject lastDragged = null;
    public static bool drag = false;
    public bool[] placedCorrectly;
    public int correctPlacements = 0;
    public GameObject winPanel; 



  

    void Start()
    {
        placedCorrectly = new bool[vehicles.Length];
        for (int i = 0; i < placedCorrectly.Length; i++)
            placedCorrectly[i] = false;
    




    startCoordinates = new Vector2[vehicles.Length];
        shadowStartCoordinates = new Vector2[shadows.Length];

       
        Transform[] shuffledSpawnPoints = new Transform[spawnPoints.Length];
        spawnPoints.CopyTo(shuffledSpawnPoints, 0);
        ShuffleArray(shuffledSpawnPoints);

        for (int i = 0; i < vehicles.Length; i++)
        {
            Vector3 spawnPos = shuffledSpawnPoints[i % shuffledSpawnPoints.Length].position;
            vehicles[i].GetComponent<RectTransform>().position = spawnPos;
            startCoordinates[i] = vehicles[i].GetComponent<RectTransform>().localPosition;
        }

     
        Transform[] shuffledShadowSpawnPoints = new Transform[shadowSpawnPoints.Length];
        shadowSpawnPoints.CopyTo(shuffledShadowSpawnPoints, 0);
        ShuffleArray(shuffledShadowSpawnPoints);

        for (int i = 0; i < shadows.Length; i++)
        {
            Vector3 shadowSpawnPos = shuffledShadowSpawnPoints[i % shuffledShadowSpawnPoints.Length].position;
            shadows[i].GetComponent<RectTransform>().position = shadowSpawnPos;
            shadowStartCoordinates[i] = shadows[i].GetComponent<RectTransform>().localPosition;
        }
    }

    void ShuffleArray<T>(T[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int randIndex = Random.Range(i, array.Length);
            T temp = array[i];
            array[i] = array[randIndex];
            array[randIndex] = temp;
        }
    }
}
