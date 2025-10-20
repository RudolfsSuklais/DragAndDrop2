using UnityEngine;

public class PlaneFlyover : MonoBehaviour
{
    public GameObject plane;
    public float speed = 50f;
    public Vector3 startPosition = new Vector3(10, 0, 0);
    public Vector3 endPosition = new Vector3(-10, 0, 0);

    void Start()
    {
        FindAndSetupPlane();
    }

    void OnEnable()
    {
        FindAndSetupPlane();
    }

    void FindAndSetupPlane()
    {
        // Ja atsauce ir null, mēģinam atrast lidmašīnu
        if (plane == null)
        {
            // Meklējam pēc nosaukuma vai taga
            GameObject planeObject = GameObject.Find("Plane"); // Mainiet uz jūsu lidmašīnas nosaukumu
            if (planeObject != null)
            {
                plane = planeObject;
                Debug.Log("Found plane: " + plane.name);
            }
            else
            {
                Debug.LogError("Plane object not found in scene!");
                return;
            }
        }

        // Reset pozīciju
        ResetPlane();
    }

    void ResetPlane()
    {
        if (plane != null)
        {
            plane.transform.position = startPosition;
            plane.transform.rotation = Quaternion.identity;
            Debug.Log("Plane reset to start position");
        }
    }

    void Update()
    {
        // Katrā frame pārbaudam, vai plane vēl eksistē
        if (plane == null)
        {
            FindAndSetupPlane();
            return;
        }

        plane.transform.position = Vector3.MoveTowards(plane.transform.position, endPosition, speed * Time.deltaTime);

        if (Vector3.Distance(plane.transform.position, endPosition) < 0.1f)
        {
            plane.transform.position = startPosition;
        }
    }
}