using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PanelController : MonoBehaviour
{
    public GameObject NodeInfoPanel;
    private Camera mainCamera;
    public GameObject infoTextGo; // �� Inspector ��ָ���� UI Text ������ʾ��Ϣ
    public GameObject currentNode; // ��ǰѡ�е�����
    private TextMeshProUGUI infoText;

    // Start is called before the first frame update
    void Awake()
    {
        NodeInfoPanel.SetActive(false);
        mainCamera = Camera.main;
    }

    public void NodePanelControl(RaycastHit hit)
    {
        GameObject hoveredObject = hit.collider.gameObject;
        NodeBehavior node = hoveredObject.GetComponent<NodeBehavior>();
        if (node != null)
        {
            EnableNodeInfoPanel();
        }
    }
    public void EnableNodeInfoPanel()
    {   
        if (NodeInfoPanel == null)
        {
            Debug.LogWarning("NodeInfoPanel is not assigned in the inspector.");
            return;
        }

        NodeInfoPanel.SetActive(true);
        infoText = infoTextGo.GetComponent<TextMeshProUGUI>();
        // ��ʼ���ı�Ϊ��
        infoText.text = "";
        currentNode = null;
    }
    public void DisableNodeInfoPanel()
    {
        if (NodeInfoPanel == null)
        {
            Debug.LogWarning("NodeInfoPanel is not assigned in the inspector.");
            return;
        }
        NodeInfoPanel.SetActive(false);
    }
    public void UpdateInfo(RaycastHit hit)
    {
        GameObject hoveredObject = hit.collider.gameObject;

        // ��������Ƿ��� CubeBehavior �ű�
        NodeBehavior node = hoveredObject.GetComponent<NodeBehavior>();
        if (node != null)
        {
            currentNode = hoveredObject;
            // ��ȡ Properties ���ݲ�չʾ
            Properties properties = node.properties;
            if (properties != null && NodeInfoPanel.activeSelf)
            {
                // ��ʾ����ֵ
                infoText.text = $"����: {hoveredObject.name}\n" +
                                $"״̬: {properties.type}\n" +
                                $"������ֵ: {properties.awakeThreshold}\n" +
                                $"��¶��ֵ: {properties.exposeThreshold}\n" +
                                $"�����鼮: {properties.numOfBooks}/{properties.maximumNumOfBooks}";
            }
        }

    }
}
