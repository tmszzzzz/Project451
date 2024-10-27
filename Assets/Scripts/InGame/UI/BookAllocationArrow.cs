using TMPro;
using UnityEngine;

public class BookAllocationArrow : MonoBehaviour
{
    public Transform pointA;         // 起点
    public Transform pointB;         // 终点
    public LineRenderer lineRenderer; // LineRenderer 组件
    public TextMeshPro text; //text
    public int allocationNum = 0;
    public int curveResolution = 20; // 曲线的分辨率，越高越平滑
    public float nodeHeight = 2.0f; // 断点浮起的高度
    public float curveHeight = 2.0f; // 中段凸起的高度
    public float deltaYText = 2.0f;
    public float arrowHeadLength = 0.2f; // 箭头头部长度
    public float arrowHeadAngle = 20.0f; // 箭头角度

    private void Start()
    {
        transform.position = (pointA.position + pointB.position) / 2 + Vector3.up * (curveHeight + nodeHeight + deltaYText);
    }

    void Update()
    {
        if (pointA != null && pointB != null)
        {
            // 设置线条的点数，曲线的分辨率+箭头的点数
            lineRenderer.positionCount = curveResolution;

            DrawSmoothArrow();
            transform.position = (pointA.position + pointB.position) / 2 + Vector3.up * (curveHeight + nodeHeight + deltaYText);
            text.text = $"{allocationNum}";
        }
    }

    void DrawSmoothArrow()
    {
        Vector3 start = pointA.position + Vector3.up * nodeHeight;
        Vector3 end = pointB.position + Vector3.up * nodeHeight;
        Vector3 middle = (start + end) / 2 + Vector3.up * (curveHeight + nodeHeight);

        // 生成曲线的点
        for (int i = 0; i < curveResolution; i++)
        {
            float t = i / (float)curveResolution;
            Vector3 pointOnCurve = CalculateQuadraticBezierPoint(t, start, middle, end);
            lineRenderer.SetPosition(i, pointOnCurve);
        }
    }

    // 使用二次贝塞尔曲线计算中间的点
    Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        return uu * p0 + 2 * u * t * p1 + tt * p2;
    }

}
