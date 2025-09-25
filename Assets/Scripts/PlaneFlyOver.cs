using UnityEngine;

public class PlaneFlyover : MonoBehaviour
{
    public GameObject plane;
    public float speed = 50f;
    public Vector3 startPosition = new Vector3(10, 0, 0);
    public Vector3 endPosition = new Vector3(-10, 0, 0);

    void Start()
    {
        plane.transform.position = startPosition;
        plane.transform.rotation = Quaternion.identity; 
    }

    void Update()
    {
        plane.transform.position = Vector3.MoveTowards(plane.transform.position, endPosition, speed * Time.deltaTime);

        if (Vector3.Distance(plane.transform.position, endPosition) < 0.1f)
        {
            plane.transform.position = startPosition;
        }
    }
}
