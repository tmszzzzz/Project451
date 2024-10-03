using UnityEngine;

public class DraggableEditor : MonoBehaviour
{
    private Vector3 offset;
    private Camera cam;
    private Plane plane; // ���ڶ���ˮƽ��

    void Start()
    {
        cam = Camera.main;
        // ����ˮƽ�棬ͨ���� y = 0 ��ƽ��
        plane = new Plane(Vector3.up, Vector3.zero);
    }

    void OnMouseDown()
    {
        offset = transform.position - GetMouseWorldPos();
    }

    void OnMouseDrag()
    {
        transform.position = GetMouseWorldPos() + offset;
    }

    private Vector3 GetMouseWorldPos()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        float enter;
        if (plane.Raycast(ray, out enter))
        {
            return ray.GetPoint(enter);
        }
        return Vector3.zero; // ���û������ƽ�棬����������
    }
}
