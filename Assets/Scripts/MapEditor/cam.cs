using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f; // ����ƶ��ٶ�
    public float zoomSpeed = 2f;  // ��������ٶ�
    public float minZoom = 2f;     // ��С����ֵ
    public float maxZoom = 10f;    // �������ֵ

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
        float horizontal = Input.GetAxis("Horizontal"); // A/D �� ��/�Ҽ�ͷ
        float vertical = Input.GetAxis("Vertical");     // W/S �� ��/�¼�ͷ
        

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
            cam.orthographicSize -= scroll * zoomSpeed; // �޸������ orthographicSize ��ʵ������
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom); // �������ŷ�Χ
        }
    }
    void HandleRotation()
    {
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePosition = Input.mousePosition;  // ��¼��갴��ʱ��λ��
        }

        if (Input.GetMouseButton(1))  // �Ҽ���ס
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            float rotationX = delta.x * 10 * Time.deltaTime;
            float rotationY = -delta.y * 10 * Time.deltaTime;

            // ��ת���
            transform.eulerAngles += new Vector3(rotationY, rotationX, 0);

            lastMousePosition = Input.mousePosition;  // �������λ��
        }
    }
}
