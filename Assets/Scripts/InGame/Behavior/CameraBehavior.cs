using UnityEngine;
using UnityEngine.AI;

public class CameraBehavior : MonoBehaviour
{
    public float zoomSpeed = 5000f;  
    public float minZoom = 2f;     
    public float maxZoom = 1000f;
    public float overviewZoom = 90f;

    private Camera cam;
    public float rotationSpeed = 50f; 
    public float moveSpeed = 50f;

    public GameObject circleCenter;
    public float cameraCircleRadius = 10f;
    
    Vector3 HandleOutBound(Vector3 newPosition, Vector3 oldPosition) {
        Vector3 move = newPosition - oldPosition;

        if (circleCenter != null)
        {
            // 创建一个指向 y=0 平面的射线
            Ray ray = cam.ScreenPointToRay(new(Screen.width/2, (Screen.height/2),0));
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            // 计算摄像机到 y=0 平面的交点
            if (plane.Raycast(ray, out float enter))
            {
                Vector3 cameraGroundPosition = ray.GetPoint(enter);

                // 计算从交点到中心的距离
                Vector3 toCenter = circleCenter.transform.position - cameraGroundPosition;
                toCenter.y = 0; // 确保水平距离
                float distance = toCenter.magnitude;

                // 如果交点超出半径范围
                if (distance > cameraCircleRadius)
                {
                    Vector3 toCenterDirection = toCenter.normalized;
                    Vector3 moveDirection = move.normalized;
                    float angle = Vector3.SignedAngle(toCenterDirection, moveDirection, Vector3.up);

                    // 修正角度，使其保持在边界内
                    if (angle > 90 || angle < -90)
                    {
                        move = Quaternion.AngleAxis(-angle, Vector3.up) * move;
                    }
                }
            }
        }

        return oldPosition + move;
    }

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

            Vector3 f = new(transform.forward.x, 0, transform.forward.z);
            Vector3 r = new(transform.right.x, 0, transform.right.z);

            Vector3 move = moveSpeed * Time.deltaTime * (vertical * f.normalized + horizontal * r.normalized);
            Vector3 direction = transform.position - circleCenter.transform.position;
            direction.y = 0;
            direction = direction.normalized;
            Vector3 originalMove = move;
            transform.position += move;
            cameraGroundPosition += move;

            Vector3 toCenter = circleCenter.transform.position - cameraGroundPosition;
            toCenter.y = 0; // 确保水平距离
            float distance = toCenter.magnitude;
            
            if (distance > cameraCircleRadius)
            {
                transform.position += (distance - cameraCircleRadius) * 0.1f * toCenter.normalized;
            }
            
            //if (distance > cameraCircleRadius&&Vector3.Angle(direction, move) < 90) move -= (Vector3.Dot(move, direction)) * direction;
            //transform.position += move;
            //cameraGroundPosition += move;
            //Vector3 newDist = cameraGroundPosition - circleCenter.transform.position; //目标点位置向量
            //newDist.y = 0;
            //if (newDist.magnitude > cameraCircleRadius)
            //{
            //    var delta = cameraCircleRadius * newDist.normalized - cameraGroundPosition;
            //    cameraGroundPosition += delta;
            //    transform.position += delta;
            //    //出最大范围修正
            //    if (Vector3.Cross(originalMove, direction).normalized + Vector3.Cross(originalMove, newDist).normalized == Vector3.zero)
            //    {
            //        var delta2 = cameraCircleRadius * originalMove.normalized - cameraGroundPosition;
            //        cameraGroundPosition += delta2;
            //        transform.position += delta2;
            //        //超出死点修正
            //    }
            //}

            
        }
        
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
        if (!RoundManager.instance.operationForbidden)
        {
            //Vector3 oldPosition = transform.position;
            HandleMovement();
            HandleRotation();
            //transform.position = HandleOutBound(transform.position, oldPosition);

            HandleZoom();
        }
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