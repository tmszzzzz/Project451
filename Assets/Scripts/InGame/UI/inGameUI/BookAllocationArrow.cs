using System;
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
    private Animator anim;
    public bool displayNum = true;
    public bool isDoubleDirection = false;
    public bool changedTextPosition = false;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        if (anim == null) Debug.LogWarning("No Animator in arrow!");
    }

    private void Start()
    {
        transform.position = (pointA.position + pointB.position) / 2 + Vector3.up * (curveHeight + nodeHeight + deltaYText);
        ChangeTextPosition(isDoubleDirection, pointA, pointB);
    }

    private void ChangeTextPosition(bool b, Transform pointA, Transform pointB)
    {
        if (isDoubleDirection && !changedTextPosition)
        {
            changedTextPosition = true;
            // 修改text的位置
            float k = 0.33f;         // 从中点变化到text位置变化的缩放比例（经验值
            Vector3 start = pointA.position + Vector3.up * nodeHeight;
            Vector3 end = pointB.position + Vector3.up * nodeHeight;
            Vector3 middle1, middle2;
            middle1 = new Vector3(start.x, end.y + curveHeight + nodeHeight, end.z);
            middle2 = (start + end) / 2 + Vector3.up * (curveHeight + nodeHeight);
            text.transform.position += k * (middle1 - middle2);
        }
        if (!isDoubleDirection && changedTextPosition)
        {
            float k = 0.33f;         // 从中点变化到text位置变化的缩放比例（经验值
            Vector3 start = pointA.position + Vector3.up * nodeHeight;
            Vector3 end = pointB.position + Vector3.up * nodeHeight;
            Vector3 middle1, middle2;
            middle1 = new Vector3(start.x, end.y + curveHeight + nodeHeight, end.z);
            middle2 = (start + end) / 2 + Vector3.up * (curveHeight + nodeHeight);
            text.transform.position -= k * (middle1 - middle2);
            changedTextPosition = false;
        }
    }

    void Update()
    {
        if (pointA != null && pointB != null)
        {
            ChangeTextPosition(isDoubleDirection, pointA, pointB);
            // 设置线条的点数，曲线的分辨率+箭头的点数
            lineRenderer.positionCount = curveResolution;

            DrawSmoothArrow();
            transform.position = (pointA.position + pointB.position) / 2 + Vector3.up * (curveHeight + nodeHeight + deltaYText);
            text.text = (!displayNum || allocationNum == 0)? "" : $"{allocationNum}";
        }
    }

    void DrawSmoothArrow()
    {
        Vector3 start = pointA.position + Vector3.up * nodeHeight;
        Vector3 end = pointB.position + Vector3.up * nodeHeight;
        Vector3 middle;
        if (isDoubleDirection)
        {
            middle = new Vector3(start.x, end.y + curveHeight + nodeHeight, end.z);
        }
        else
        {
            middle = (start + end) / 2 + Vector3.up * (curveHeight + nodeHeight);
        }
        
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
    public void Confirm()
    {
        anim.SetTrigger("Confirm");
    }
    public void Cancel()
    {
        anim.SetTrigger("Cancel");
    }
}
