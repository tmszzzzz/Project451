using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public float zoomSpeed = 2f;  // 相机缩放速度
    public float minZoom = 2f;     // 最小缩放值
    public float maxZoom = 1000f;    // 最大缩放值

    private Camera cam;
    public float rotationSpeed = 50f; // 旋转速度

    void HandleMovement()
    {
        // 获取水平输入（A 和 D 键）
        float horizontalInput = Input.GetAxis("Horizontal");

        // 计算旋转角度
        float rotationAmount = horizontalInput * rotationSpeed * Time.deltaTime;

        // 围绕世界空间中的原点(0, 0, 0)进行旋转，围绕y轴（Vector3.up）
        transform.RotateAround(Vector3.zero, Vector3.up, rotationAmount);

        // 始终保持相机朝向原点
        transform.LookAt(Vector3.zero);
    }

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            // 相机在其前向方向（z轴）上移动
            Vector3 forward = transform.forward;
            Vector3 newPosition = transform.position + forward * scroll * zoomSpeed;

            // 计算相机与目标点之间的距离
            float distance = Vector3.Distance(Vector3.zero, transform.position);

            // 限制相机的缩放距离
            if (distance >= minZoom && distance <= maxZoom)
            {
                transform.position = newPosition;
            }
        }
    }
}