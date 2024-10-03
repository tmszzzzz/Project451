using UnityEngine;

public class OrthographicCameraController : MonoBehaviour
{
    public float moveSpeed = 5f; // 相机移动速度
    public float zoomSpeed = 2f;  // 相机缩放速度
    public float minZoom = 2f;     // 最小缩放值
    public float maxZoom = 10f;    // 最大缩放值

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true; // 确保相机为正交模式
    }

    void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal"); // A/D 或 左/右箭头
        float vertical = Input.GetAxis("Vertical");     // W/S 或 上/下箭头

        Vector3 f = new(transform.forward.x, 0, transform.forward.z);
        Vector3 r = new(transform.right.x, 0, transform.right.z);

        Vector3 move = (vertical * f.normalized + horizontal * r.normalized) * moveSpeed * Time.deltaTime;
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
}
