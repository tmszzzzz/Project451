using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f; // 相机移动速度
    public float zoomSpeed = 2f;  // 相机缩放速度
    public float minZoom = 2f;     // 最小缩放值
    public float maxZoom = 10f;    // 最大缩放值

    private Camera cam;
    private Vector3 lastMousePosition;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        HandleMovement();
        HandleZoom();
        HandleRotation();
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal"); // A/D 或 左/右箭头
        float vertical = Input.GetAxis("Vertical");     // W/S 或 上/下箭头
        

        Vector3 f = new(transform.forward.x, 0, transform.forward.z);
        Vector3 r = new(transform.right.x, 0, transform.right.z);
        Vector3 v = Vector3.zero;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            v -= new Vector3(0, 1, 0);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            v += new Vector3(0, 1, 0);
        }

        Vector3 move = (vertical * f.normalized + horizontal * r.normalized + v) * moveSpeed * Time.deltaTime;
        transform.position += move;
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            cam.orthographicSize -= scroll * zoomSpeed; // 修改相机的 orthographicSize 来实现缩放
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom); // 限制缩放范围
        }
    }
    void HandleRotation()
    {
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePosition = Input.mousePosition;  // 记录鼠标按下时的位置
        }

        if (Input.GetMouseButton(1))  // 右键按住
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            float rotationX = delta.x * 10 * Time.deltaTime;
            float rotationY = -delta.y * 10 * Time.deltaTime;

            // 旋转相机
            transform.eulerAngles += new Vector3(rotationY, rotationX, 0);

            lastMousePosition = Input.mousePosition;  // 更新鼠标位置
        }
    }
}
