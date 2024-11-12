using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class CameraBehavior : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 5000f;  
    [SerializeField] private float minZoom = 2f;     
    [SerializeField] private float maxZoom = 1000f;
    [SerializeField] private float overviewFieldOfView = 30f;
    [SerializeField] private float overviewDistanceAddition = 10f;
    private bool overviewing = false;
    private Vector3 savedPos;
    private float savedZoom;
    private Quaternion savedRotation;

    private Camera cam;
    [SerializeField] private float rotationSpeed = 50f; 
    [SerializeField] private float moveSpeed = 50f;

    [SerializeField] private GameObject circleCenter;
    [SerializeField] private float cameraCircleRadius = 10f;

    private Vector3 realPosition;
    private Quaternion realRotation;
    private float realFieldOfView;
    [SerializeField] private float lerpSpeed = 1f;
    [SerializeField] private float decrementLerpSpeed = 0.6f;

    void HandleMovement()
    {
        Ray ray = cam.ScreenPointToRay(new(Screen.width/2, (Screen.height/2),0));
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        // 计算摄像机到 y=0 平面的交点
        if (plane.Raycast(ray, out float enter))
        {
            Vector3 cameraGroundPosition = ray.GetPoint(enter);

            // 计算从交点到中心的距离
            

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 f = new((realRotation * Vector3.forward).x, 0, (realRotation * Vector3.forward).z);
            Vector3 r = new((realRotation * Vector3.right).x, 0, (realRotation * Vector3.right).z);

            Vector3 move = moveSpeed * Time.deltaTime * (vertical * f.normalized + horizontal * r.normalized);
            Vector3 direction = realPosition - circleCenter.transform.position;
            direction.y = 0;
            direction = direction.normalized;
            Vector3 originalMove = move;
            realPosition += move;
            cameraGroundPosition += move;

            Vector3 toCenter = circleCenter.transform.position - cameraGroundPosition;
            toCenter.y = 0; // 确保水平距离
            float distance = toCenter.magnitude;
            
            if (distance > cameraCircleRadius)
            {
                realPosition += (distance - cameraCircleRadius) * 0.1f * toCenter.normalized;
            }

            
        }
        
    }

    void HandleRotation()
    {
        float spin = Input.GetAxis("Spin Clockwise");
        if (spin != 0)
        {
            // Calculate the point where the camera's center line collides with the 0-0 surface
            Ray ray = new Ray(realPosition, (realRotation * Vector3.forward));
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            if (plane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);

                // Rotate the camera around the hit point
                //transform.RotateAround(hitPoint, Vector3.up, spin * rotationSpeed * Time.deltaTime);
                
                float angle = spin * rotationSpeed * Time.deltaTime;
                Quaternion rotationDelta = Quaternion.AngleAxis(angle, Vector3.up);

                Vector3 offset = realPosition - hitPoint;
                realPosition = hitPoint + rotationDelta * offset;

                realRotation = rotationDelta * realRotation;
            }
        }
    }

    void Awake()
    {
        cam = GetComponent<Camera>();
        realPosition = transform.position;
        realRotation = transform.rotation;
        realFieldOfView = cam.fieldOfView;
    }

    void Update()
    {
        if (!overviewing && !RoundManager.instance.operationForbidden)
        {
            HandleMovement();
            HandleRotation();
            HandleZoom();
        }

        //transform.position = HandleOutBound(transform.position, oldPosition);
        transform.position = Vector3.Lerp(transform.position, realPosition, lerpSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, lerpSpeed*1.5f);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, realFieldOfView, lerpSpeed);


    }



    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            // Calculate new FOV
            float newFOV = realFieldOfView - scroll * zoomSpeed;
            newFOV = Mathf.Clamp(newFOV, minZoom, maxZoom);

            // Apply new FOV
            realFieldOfView = newFOV;
        }
    }
    


    public async Task OverviewEnter()
    {
        savedPos = realPosition;
        savedZoom = realFieldOfView;
        savedRotation = realRotation;
        overviewing = true;
        lerpSpeed -= decrementLerpSpeed;

        realFieldOfView = overviewFieldOfView;
        Ray ray = new Ray(realPosition, (realRotation * Vector3.forward));
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 delta = circleCenter.transform.position - hitPoint;
            delta.y = 0;
            realPosition += delta;
            realPosition += overviewDistanceAddition * (realPosition - Vector3.zero).normalized;
            float angle = Mathf.Atan2(hitPoint.x, hitPoint.z) * Mathf.Rad2Deg;
            int degDelta = (int)((Math.Floor((realRotation.eulerAngles.y + 10) / 120f) * 120 - 10) -
                                 savedRotation.eulerAngles.y);
            Quaternion rotationDelta = Quaternion.AngleAxis(degDelta, new(0,1,0));
            realRotation = rotationDelta * realRotation;
            realPosition = rotationDelta * realPosition;


        }
        await Task.Delay(1500);
    }

    public async Task OverviewExit()
    {
        realPosition = savedPos;
        realFieldOfView = savedZoom;
        realRotation = savedRotation;
        overviewing = false;
        
        await Task.Delay(1500);
        lerpSpeed += decrementLerpSpeed;
    }
}