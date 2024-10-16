using UnityEngine;

public class BookAllocationArrow : MonoBehaviour
{
    public Vector3 pointA; // 第一个点
    public Vector3 pointB; // 第二个点
    public LineRenderer lineRenderer; // 线渲染器
    public float height = 1.0f; // 中段凸起的高度
    public float arrowHeadLength = 0.2f; // 箭头头部长度
    public float arrowHeadAngle = 20.0f; // 箭头角度

    void Start()
    {
        // 设置 LineRenderer 属性
        lineRenderer.positionCount = 3; // 4个点，前后各一个
        lineRenderer.useWorldSpace = true; // 使用世界坐标

        
    }
    private void Update()
    {
        DrawArrow();
    }

    void DrawArrow()
    {
        // 计算中点和箭头的方向
        Vector3 midPoint = (pointA + pointB) / 2;
        Vector3 direction = (pointB - pointA).normalized;

        // 计算向上的偏移
        Vector3 upOffset = Vector3.up * height;

        // 设置 LineRenderer 的位置
        lineRenderer.SetPosition(0, pointA);
        lineRenderer.SetPosition(1, midPoint + upOffset); // 中段向上凸起
        lineRenderer.SetPosition(2, pointB);

        // 设置箭头
        DrawArrowHead(midPoint, direction);
    }

    void DrawArrowHead(Vector3 position, Vector3 direction)
    {
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, arrowHeadAngle, 0) * Vector3.forward;
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -arrowHeadAngle, 0) * Vector3.forward;

        // 计算箭头的顶端位置
        Vector3 arrowHeadBase = position + direction * arrowHeadLength;
        //lineRenderer.SetPosition(3, arrowHeadBase + right); // 右边的箭头
        //lineRenderer.SetPosition(4, arrowHeadBase + left);  // 左边的箭头
    }
}
