using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using DG.Tweening.Plugins;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    public static PanelController instance;
    public GameObject NodeInfoPanel;
    private Camera mainCamera;
    public GameObject currentNode; // 当前选中的物体
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private TextMeshProUGUI identityText;
    [SerializeField] private TextMeshProUGUI awakeThresholdText;
    [SerializeField] private TextMeshProUGUI exposeThresholdText;
    [SerializeField] private TextMeshProUGUI numOfBooksText;
    [SerializeField] private TextMeshProUGUI influenceText;
    [SerializeField] private Slider BookSlider;
    [SerializeField] private Slider InfluenceSlider;
    [SerializeField] private TextMeshProUGUI fallThresholdText;

    // Start is called before the first frame update
    void Awake()
    {
        NodeInfoPanel.SetActive(false);
        mainCamera = Camera.main;
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        RoundManager.instance.BookAllocationChange += OnBookAllocationChange;
    }

    public void NodePanelControl(RaycastHit hit)
    {
        GameObject hoveredObject = hit.collider.gameObject;
        NodeBehavior node = hoveredObject.GetComponent<NodeBehavior>();
        if (node != null)
        {
            EnableNodeInfoPanel(node);
        }
        else
        {
            DisableNodeInfoPanel();
        }
    }
    public void EnableNodeInfoPanel(NodeBehavior node)
    {   
        if (NodeInfoPanel == null)
        {
            Debug.LogWarning("NodeInfoPanel is not assigned in the inspector.");
            return;
        }

        NodeInfoPanel.SetActive(true);
        currentNode = node.gameObject;
        Properties properties = node.properties;
        if (properties != null && NodeInfoPanel.activeSelf)
        {
            nameText.text = NameManager.instance.ConvertNodeNameToName(currentNode.name);
            stateText.text = properties.stateNameToCNString(properties.state);
            identityText.text = properties.typeNameToCNString(properties.type);
            awakeThresholdText.text = "转变阈值: " + properties.awakeThreshold;
            exposeThresholdText.text = "暴露阈值: " + properties.exposeThreshold;
            numOfBooksText.text = "持有书籍: " + properties.numOfBooks + "/" + properties.maximumNumOfBooks;
            influenceText.text = "当前受影响: " + node.NowState().influence;
            fallThresholdText.text = "维持阈值：" + properties.fallThreshold; 

            BookSlider.value = properties.numOfBooks/(float)properties.maximumNumOfBooks;
            InfluenceSlider.value = node.NowState().influence / (float)properties.exposeThreshold;
            //Debug.Log(InfluenceSlider.value);
        }
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
                nameText.text = NameManager.instance.ConvertNodeNameToName(hoveredObject.name);
                stateText.text = properties.stateNameToCNString(properties.state);
                identityText.text = properties.typeNameToCNString(properties.type);
                awakeThresholdText.text = "转变阈值: " + properties.awakeThreshold;
                exposeThresholdText.text = "暴露阈值: " + properties.exposeThreshold;
                numOfBooksText.text = "持有书籍: " + properties.numOfBooks + "/" + properties.maximumNumOfBooks;
                influenceText.text = "当前受影响: " + node.NowState().influence;
                fallThresholdText.text = "维持阈值：" + properties.fallThreshold; 

                BookSlider.value = properties.numOfBooks/(float)properties.maximumNumOfBooks;
                InfluenceSlider.value = node.NowState().influence / (float)properties.exposeThreshold;
                //Debug.Log(InfluenceSlider.value);
            }
        }

    }

    private void OnBookAllocationChange()
    {
        //Debug.Log(1);
        if (currentNode != null)
        {
            //Debug.Log(2);
            var node = currentNode.GetComponent<NodeBehavior>();
            Properties properties = node.properties;
            if (properties != null && NodeInfoPanel.activeSelf)
            {
                nameText.text = NameManager.instance.ConvertNodeNameToName(currentNode.name);
                stateText.text = properties.stateNameToCNString(properties.state);
                identityText.text = properties.typeNameToCNString(properties.type);
                awakeThresholdText.text = "转变阈值: " + properties.awakeThreshold;
                exposeThresholdText.text = "暴露阈值: " + properties.exposeThreshold;
                numOfBooksText.text = "持有书籍: " + (properties.numOfBooks + RoundManager.instance.BookAllocationMap[currentNode]) + "/" + properties.maximumNumOfBooks;
                influenceText.text = "当前受影响: " + node.PredictState().influence;
                fallThresholdText.text = "维持阈值：" + properties.fallThreshold; 

                BookSlider.value = (float)(properties.numOfBooks + RoundManager.instance.BookAllocationMap[currentNode])/(float)properties.maximumNumOfBooks;
                InfluenceSlider.value = (float) node.PredictState().influence / (float)properties.exposeThreshold;
            }
        }
    }
}
