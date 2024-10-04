using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public float zoomSpeed = 2f;  // ��������ٶ�
    public float minZoom = 2f;     // ��С����ֵ
    public float maxZoom = 10f;    // �������ֵ

    private Camera cam;
    public float rotationSpeed = 50f; // ��ת�ٶ�

    void HandleMovement()
    {
        // ��ȡˮƽ���루A �� D ����
        float horizontalInput = Input.GetAxis("Horizontal");

        // ������ת�Ƕ�
        float rotationAmount = horizontalInput * rotationSpeed * Time.deltaTime;

        // Χ������ռ��е�ԭ��(0, 0, 0)������ת��Χ��y�ᣨVector3.up��
        transform.RotateAround(Vector3.zero, Vector3.up, rotationAmount);

        // ʼ�ձ����������ԭ��
        transform.LookAt(Vector3.zero);
    }

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