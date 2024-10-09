using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public float zoomSpeed = 5000f;  
    public float minZoom = 2f;     
    public float maxZoom = 1000f;    

    private Camera cam;
    public float rotationSpeed = 50f; 
    public float moveSpeed = 50f;

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 f = new(transform.forward.x, 0, transform.forward.z);
        Vector3 r = new(transform.right.x, 0, transform.right.z);

        Vector3 move = (vertical * f.normalized + horizontal * r.normalized) * moveSpeed * Time.deltaTime;
        transform.position += move;
    }

    void HandleRotation()
    {
        float spin = Input.GetAxis("Spin Clockwise");
        if (spin != 0)
        {
            // Calculate the point where the camera's center line collides with the 0-0 surface
            Ray ray = new Ray(transform.position, transform.forward);
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            if (plane.Raycast(ray, out float enter))
            {
            Vector3 hitPoint = ray.GetPoint(enter);

            // Rotate the camera around the hit point
            transform.RotateAround(hitPoint, Vector3.up, spin * rotationSpeed * Time.deltaTime);
            }
        }
    }

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
    }

    

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            // Calculate new FOV
            float newFOV = cam.fieldOfView - scroll * zoomSpeed;
            newFOV = Mathf.Clamp(newFOV, minZoom, maxZoom);

            // Apply new FOV
            cam.fieldOfView = newFOV;
        }
    }
}