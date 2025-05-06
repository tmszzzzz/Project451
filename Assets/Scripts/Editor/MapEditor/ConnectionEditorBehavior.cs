using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionEditorBehavior : MonoBehaviour
{
    public GameObject startNode;
    public GameObject endNode;
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;  // 连接线的起始宽度
        lineRenderer.endWidth = 0.1f;    // 连接线的结束宽度
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // 使用默认材质
        lineRenderer.startColor = Color.white; // 连接线的起始颜色
        lineRenderer.endColor = Color.white;   // 连接线的结束颜色
    }

    void Update()
    {
        if (startNode != null && endNode != null)
        {
            UpdateLine();
        }
    }

    private void UpdateLine()
    {
        lineRenderer.positionCount = 2; // 连接线有两个点
        lineRenderer.SetPosition(0, startNode.transform.position); // 设置起始点
        lineRenderer.SetPosition(1, endNode.transform.position);   // 设置结束点
    }
}
