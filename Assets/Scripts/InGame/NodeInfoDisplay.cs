using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NodeInfoDisplay : MonoBehaviour
{
    public GameObject infoTextGo; // �� Inspector ��ָ���� UI Text ������ʾ��Ϣ
    private TextMeshProUGUI infoText;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        infoText = infoTextGo.GetComponent<TextMeshProUGUI>();
        // ��ʼ���ı�Ϊ��
        infoText.text = "";
    }

    void Update()
    {
        // ���߼�����ָ�������
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hoveredObject = hit.collider.gameObject;

            // ��������Ƿ��� CubeBehavior �ű�
            NodeBehavior node = hoveredObject.GetComponent<NodeBehavior>();
            if (node != null)
            {
                // ��ȡ Properties ���ݲ�չʾ
                Properties properties = node.properties;
                if (properties != null)
                {
                    // ��ʾ����ֵ
                    infoText.text = $"Name: {hoveredObject.name}\n" +
                                    $"Identity: {properties.type}\n" +
                                    $"Awake Threshold: {properties.awakeThreshold}\n" +
                                    $"Expose Threshold: {properties.exposeThreshold}\n" +
                                    $"Books: {properties.numOfBooks}/{properties.maximumNumOfBooks}";
                }
            }
        }
    }
}
