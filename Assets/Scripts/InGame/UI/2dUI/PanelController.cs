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
    
    private Vector3 _nodeInfoPanelOffset = new Vector3(20, -20, 0);
    [SerializeField] private GameObject _ResourceUsagePanel;
    [SerializeField] private Image _ImageThatSeperateResourceUsagePanel;
    
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
    
    public void OpenResourceUsagePanel()
    {
        _ResourceUsagePanel.SetActive(true);
    }
    
    public void CloseResourceUsagePanel()
    {
        _ResourceUsagePanel.SetActive(false);
    }
    
    public void ToggleResourceUsagePanel()
    {
        _ResourceUsagePanel.SetActive(!_ResourceUsagePanel.activeSelf);
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

    private void _changeNodeInfoPanleContenAndPosition(NodeBehavior node)
    {
        currentNode = node.gameObject;
        Properties properties = node.properties;
        if (properties != null && NodeInfoPanel.activeSelf)
        {
            nameText.text = NameManager.instance.ConvertNodeNameToName(currentNode.name);
            stateText.text = properties.stateNameToCNString(properties.state);
            identityText.text = properties.typeNameToCNString(properties.type);
            awakeThresholdText.text = "转变阈值: " + properties.awakeThreshold;
            exposeThresholdText.text = "暴露阈值: " + properties.exposeThreshold;
            numOfBooksText.text = "持有书籍: " + properties.books.Count;
            influenceText.text = "当前受影响: " + node.NowState().basicInfluence;
            fallThresholdText.text = "维持阈值：" + properties.fallThreshold; 

            BookSlider.value = 0.5f;
            InfluenceSlider.value = node.NowState().basicInfluence / (float)properties.exposeThreshold;
            //Debug.Log(InfluenceSlider.value);
        }
        NodeInfoPanel.transform.position = mainCamera.WorldToScreenPoint(node.transform.position) + _nodeInfoPanelOffset;
    }
    
    public void EnableNodeInfoPanel(NodeBehavior node)
    {   
        if (NodeInfoPanel == null)
        {
            Debug.LogWarning("NodeInfoPanel is not assigned in the inspector.");
            return;
        }

        NodeInfoPanel.SetActive(true);
        _changeNodeInfoPanleContenAndPosition(node);
    }
    public void DisableNodeInfoPanel()
    {
        if (NodeInfoPanel == null)
        {
            Debug.LogWarning("NodeInfoPanel is not assigned in the inspector.");
            return;
        }
        currentNode = null;
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
            _changeNodeInfoPanleContenAndPosition(node);
        }
        else
        {
            DisableNodeInfoPanel();
        }
    }

    private void OnBookAllocationChange()
    {
        //Debug.Log(1);
        if (currentNode != null)
        {
            //Debug.Log(2);
            var node = currentNode.GetComponent<NodeBehavior>();
            _changeNodeInfoPanleContenAndPosition(node);
        }
    }
}
