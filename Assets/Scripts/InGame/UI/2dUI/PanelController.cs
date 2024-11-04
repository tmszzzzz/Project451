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
    public GameObject infoTextGo; // 在 Inspector 中指定的 UI Text 用于显示信息
    public GameObject currentNode; // 当前选中的物体
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
        // 初始化文本为空
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

        // 检查物体是否有 CubeBehavior 脚本
        NodeBehavior node = hoveredObject.GetComponent<NodeBehavior>();
        if (node != null)
        {
            currentNode = hoveredObject;
            // 获取 Properties 数据并展示
            Properties properties = node.properties;
            if (properties != null && NodeInfoPanel.activeSelf)
            {
                // 显示属性值
                infoText.text = $"姓名: {hoveredObject.name}\n" +
                                $"状态: {properties.type}\n" +
                                $"觉醒阈值: {properties.awakeThreshold}\n" +
                                $"暴露阈值: {properties.exposeThreshold}\n" +
                                $"持有书籍: {properties.numOfBooks}/{properties.maximumNumOfBooks}";
            }
        }

    }
}
