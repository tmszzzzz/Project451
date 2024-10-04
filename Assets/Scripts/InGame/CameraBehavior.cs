using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public float zoomSpeed = 2f;  // 相机缩放速度
    public float minZoom = 2f;     // 最小缩放值
    public float maxZoom = 10f;    // 最大缩放值

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
        cam.orthographic = true; // 确保相机为正交模式
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
            cam.orthographicSize -= scroll * zoomSpeed; // 修改相机的 orthographicSize 来实现缩放
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom); // 限制缩放范围
        }
    }
}