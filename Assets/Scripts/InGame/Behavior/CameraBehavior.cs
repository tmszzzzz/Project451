using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

public class CameraBehavior : MonoBehaviour
{
    public static CameraBehavior instance;
    [SerializeField] private float zoomSpeed = 5000f;  
    [SerializeField] private float minZoom = 2f;     
    [SerializeField] private float maxZoom = 1000f;
    [SerializeField] private float overviewFieldOfView = 30f;
    [SerializeField] private float focusFieldOfView = 30f;
    [SerializeField] private float focusAngle = 30f;
    [SerializeField] private float overviewDistanceAddition = 10f;
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
    [SerializeField] private GameObject PlotPtrPrefab;
    public AudioClip pageSound;
    public AudioClip pop;
    public AudioClip tap;

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
        instance = this;
    }

    void Update()
    {
        if (!RoundManager.instance.operationForbidden)
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

    public async Task FocusExit()
    {
        realPosition = savedPos;
        realFieldOfView = savedZoom;
        realRotation = savedRotation;
        
        await Task.Delay(1500);
        lerpSpeed += decrementLerpSpeed;
    }

    public async Task PlotFocusEnter(GameObject target)
    {
        savedPos = realPosition;
        savedZoom = realFieldOfView;
        savedRotation = realRotation;
        lerpSpeed -= decrementLerpSpeed;

        realFieldOfView = focusFieldOfView;
        Vector3 centerToTarget = target.transform.position - circleCenter.transform.position;
        centerToTarget.y = 0;
        Vector3 newHorizonVector3 = Quaternion.AngleAxis(focusAngle, Vector3.up) * centerToTarget;
        newHorizonVector3.y = 0;
        float angleX = realRotation.eulerAngles.x;
        Vector3 newDir = Quaternion.AngleAxis(-angleX, Vector3.Cross(newHorizonVector3,Vector3.up)) * newHorizonVector3;
        realRotation = Quaternion.LookRotation(newDir);
        Ray ray = new Ray(realPosition, (realRotation * Vector3.forward));
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 delta = target.transform.position - hitPoint;
            delta.y = 0;
            realPosition += delta;
        }

        Instantiate(PlotPtrPrefab, target.transform.position, realRotation);
        
        
        await Task.Delay(1500);
    }
    
    public async Task SwitchTo1Enter()
    {
        savedPos = realPosition;
        savedZoom = realFieldOfView;
        savedRotation = realRotation;
        lerpSpeed -= decrementLerpSpeed;
        
        realFieldOfView = 50;
        realPosition = new Vector3(2.35759401f,58.9099998f,-0.711277723f);
        realRotation = Quaternion.Euler(new Vector3(56.5750008f, 60f, 0f));
        await Task.Delay(1500);
    }

    public async Task SwitchTo2Enter()
    {
        savedPos = realPosition;
        savedZoom = realFieldOfView;
        savedRotation = realRotation;
        lerpSpeed -= decrementLerpSpeed;
        
        realFieldOfView = 50;
        realPosition = new Vector3(-3.94880056f, 58.9099998f, -5.07163668f);
        realRotation = Quaternion.Euler(new Vector3(56.5750008f, 300f, 0f));
        await Task.Delay(1500);
    }
    
    public async Task SwitchTo3Enter()
    {
        savedPos = realPosition;
        savedZoom = realFieldOfView;
        savedRotation = realRotation;
        lerpSpeed -= decrementLerpSpeed;
        
        realFieldOfView = 50;
        realPosition = new Vector3(1.45255613f,58.9099998f,-6.43465853f);
        realRotation = Quaternion.Euler(new Vector3(56.5750008f, 180f, 0f));
        await Task.Delay(1500);
    }

    public void PageSound()
    {
        GetComponent<AudioSource>().PlayOneShot(pageSound);
    }
}