using UnityEngine;

public class DraggableEditor : MonoBehaviour
{
    private Vector3 offset;
    private Camera cam;
    private Plane plane; // 用于定义水平面

    void Start()
    {
        cam = Camera.main;
        // 定义水平面，通常是 y = 0 的平面
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
        return Vector3.zero; // 如果没有命中平面，返回零向量
    }
}
