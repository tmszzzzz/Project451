using UnityEngine;

public class OrthographicCameraController : MonoBehaviour
{
    public float moveSpeed = 5f; // ����ƶ��ٶ�
    public float zoomSpeed = 2f;  // ��������ٶ�
    public float minZoom = 2f;     // ��С����ֵ
    public float maxZoom = 10f;    // �������ֵ

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true; // ȷ�����Ϊ����ģʽ
    }

    void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal"); // A/D �� ��/�Ҽ�ͷ
        float vertical = Input.GetAxis("Vertical");     // W/S �� ��/�¼�ͷ

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
            cam.orthographicSize -= scroll * zoomSpeed; // �޸������ orthographicSize ��ʵ������
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom); // �������ŷ�Χ
        }
    }
}
