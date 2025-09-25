using UnityEngine;
using UnityEngine.EventSystems;

public class DropPlaceScript : MonoBehaviour, IDropHandler
{
    private float placeZRot, vehicleZRot, rotDiff;
    private Vector3 placeSiz, vehicleSiz;
    private float xSizeDiff, ySizeDiff;
    public ObjectScript objScript;
    public UIManager uiManager;

    public void OnDrop(PointerEventData eventData)
    {
        if ((eventData.pointerDrag != null) &&
            Input.GetMouseButtonUp(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2))
        {
            if (eventData.pointerDrag.tag.Equals(tag))
            {
                placeZRot =
                    eventData.pointerDrag.GetComponent<RectTransform>().transform.eulerAngles.z;

                vehicleZRot =
                    GetComponent<RectTransform>().transform.eulerAngles.z;

                rotDiff = Mathf.Abs(placeZRot - vehicleZRot);
                Debug.Log("Rotation difference: " + rotDiff);

                placeSiz = eventData.pointerDrag.GetComponent<RectTransform>().localScale;
                vehicleSiz = GetComponent<RectTransform>().localScale;
                xSizeDiff = Mathf.Abs(placeSiz.x - vehicleSiz.x);
                ySizeDiff = Mathf.Abs(placeSiz.y - vehicleSiz.y);
                Debug.Log("X size difference: " + xSizeDiff);
                Debug.Log("Y size difference: " + ySizeDiff);

                if ((rotDiff <= 5 || (rotDiff >= 355 && rotDiff <= 360)) &&
                    (xSizeDiff <= 0.05 && ySizeDiff <= 0.05))
                {
                    Debug.Log("Correct place");

                    objScript.rightPlace = true;
                    eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition =
                        GetComponent<RectTransform>().anchoredPosition;

                    eventData.pointerDrag.GetComponent<RectTransform>().localRotation =
                        GetComponent<RectTransform>().localRotation;

                    eventData.pointerDrag.GetComponent<RectTransform>().localScale =
                         GetComponent<RectTransform>().localScale;

                    // Mark this vehicle as correctly placed
                    int index = GetVehicleIndexByTag(eventData.pointerDrag.tag);
                    if (index != -1)
                    {
                        objScript.placedCorrectly[index] = true;
                    }

                    // Play sound based on tag
                    switch (eventData.pointerDrag.tag)
                    {
                        case "Garbage":
                            objScript.effects.PlayOneShot(objScript.audioCli[2]);
                            break;

                        case "Medicine":
                            objScript.effects.PlayOneShot(objScript.audioCli[3]);
                            break;

                        case "Fire":
                            objScript.effects.PlayOneShot(objScript.audioCli[4]);
                            break;

                        default:
                            Debug.Log("Unknown tag detected");
                            break;
                    }

                    // Inside the if after marking vehicle correctly placed:
                    Debug.Log($"Vehicles placed correctly: {CountCorrectlyPlaced()}");
                    if (CountCorrectlyPlaced() >= 2)
                    {
                        Debug.Log("You Win! At least two vehicles placed correctly.");
                        uiManager.ShowWinScreen();
                    }


                }
            }
            else
            {
                objScript.rightPlace = false;
                objScript.effects.PlayOneShot(objScript.audioCli[1]);

                int index = GetVehicleIndexByTag(eventData.pointerDrag.tag);
                if (index != -1)
                {
                    objScript.vehicles[index].GetComponent<RectTransform>().localPosition =
                        objScript.startCoordinates[index];
                    objScript.placedCorrectly[index] = false;
                }
                else
                {
                    Debug.Log("Unknown tag detected");
                }
            }
        }
    }

    // Helper function to find vehicle index by tag
    private int GetVehicleIndexByTag(string tag)
    {
        for (int i = 0; i < objScript.vehicles.Length; i++)
        {
            if (objScript.vehicles[i].tag == tag)
                return i;
        }
        return -1;
    }

    // Count how many vehicles are correctly placed
    private int CountCorrectlyPlaced()
    {
        int count = 0;
        for (int i = 0; i < objScript.placedCorrectly.Length; i++)
        {
            if (objScript.placedCorrectly[i])
                count++;
        }
        return count;
    }
}
