using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionBehavior : MonoBehaviour
{
    public GameObject startNode;
    public GameObject endNode;
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;  // �����ߵ���ʼ���
        lineRenderer.endWidth = 0.1f;    // �����ߵĽ������
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // ʹ��Ĭ�ϲ���
        lineRenderer.startColor = Color.white; // �����ߵ���ʼ��ɫ
        lineRenderer.endColor = Color.white;   // �����ߵĽ�����ɫ
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
        lineRenderer.positionCount = 2; // ��������������
        lineRenderer.SetPosition(0, startNode.transform.position); // ������ʼ��
        lineRenderer.SetPosition(1, endNode.transform.position);   // ���ý�����
    }
}
