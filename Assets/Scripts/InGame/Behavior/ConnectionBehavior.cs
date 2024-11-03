using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionBehavior : MonoBehaviour
{
    public GameObject startNode;
    public GameObject endNode;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] DetectiveBehavior detective;


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
