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
    public GameObject lastNode;     // 上次选中的物体
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
    [SerializeField] private GameObject borrowBooksContent;
    [SerializeField] private GameObject borrowBooksItemPrefab;
    
    private Vector3 _nodeInfoPanelOffset = new Vector3(20, -20, 0);
    [SerializeField] private GameObject _ResourceUsagePanel;
    [SerializeField] private Image _ImageThatSeperateResourceUsagePanel;
    
    public GameObject _tutorialPanel;
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
        LockOrganizeIndex();
        LockContactInformant();
        LockInsertInformant();
    }

    void Start()
    {
        RoundManager.instance.BookAllocationChange += OnBookAllocationChange;
        if (!_tutorialPanel.activeSelf && GlobalVar.instance.currentTask != 0 && GlobalVar.instance.currentTask < 28)
        {
            _tutorialPanel.SetActive(true);
        }

        RefreshBookIndex();
    }
    
    public void OpenResourceUsagePanel()
    {
        _ResourceUsagePanel.SetActive(true);
    }
    
    public void CloseResourceUsagePanel()
    {
        if (!GlobalVar.instance.firstUseResourcePoint)
        {
            return;
        }
        _ResourceUsagePanel.SetActive(false);
    }
    
    public void ToggleResourceUsagePanel()
    {
        _ResourceUsagePanel.SetActive(!_ResourceUsagePanel.activeSelf);
        if (!GlobalVar.instance.firstOpenPointUsage)
        {
            GlobalVar.instance.firstOpenPointUsage = true;
        }
    }
    
    public void UNLockInsertInformant()
    {
        MessageBar.instance.AddMessage("你现在可以使用情报点提升获取信息的概率.");
        var v = _ResourceUsagePanel.transform.GetChild(2);
        v.GetChild(0).GetChild(0).gameObject.SetActive(false);
        StartCoroutine(FadeImageColor(v.GetChild(0).GetComponent<Image>()));
        v.GetComponent<Button>().interactable = true;
    }
    
    public void LockInsertInformant()
    {
        _ResourceUsagePanel.transform.GetChild(2).GetComponent<Button>().interactable = false;
    }
    
    public void UNLockContactInformant()
    {
        MessageBar.instance.AddMessage("你现在可以使用情报点联系线人.");
        var v = _ResourceUsagePanel.transform.GetChild(4);
        v.GetChild(0).GetChild(0).gameObject.SetActive(false);
        StartCoroutine(FadeImageColor(v.GetChild(0).GetComponent<Image>()));
        v.GetComponent<Button>().interactable = true;
    }
    
    public void LockContactInformant()
    {
        _ResourceUsagePanel.transform.GetChild(4).GetComponent<Button>().interactable = false;
    }
    
    public void UNLockOrganizeIndex()
    {
        MessageBar.instance.AddMessage("你现在可以使用情报点整理索引.");
        var v = _ResourceUsagePanel.transform.GetChild(3);
        v.GetChild(0).GetChild(0).gameObject.SetActive(false);
        StartCoroutine(FadeImageColor(v.GetChild(0).GetComponent<Image>()));
        v.GetComponent<Button>().interactable = true;
    }
    
    public void LockOrganizeIndex()
    {
        _ResourceUsagePanel.transform.GetChild(3).GetComponent<Button>().interactable = false;
    }
    
    // 颜色渐变协程
    private IEnumerator FadeImageColor(Image image)
    {
        float duration = 1f; // 渐变持续时间（秒）
        float elapsed = 0f;
        Color startColor = image.color; // 当前颜色
        Color targetColor = new Color(0, 0, 0f, 0); // 目标颜色（完全不透明色）

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration); // 插值比例（0→1）
            image.color = Color.Lerp(startColor, targetColor, t);
            yield return null; // 等待下一帧
        }

        // 确保最终颜色准确
        image.color = targetColor;
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
        if (lastNode == null)
        {
            lastNode = currentNode;
        }
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
            if (lastNode != currentNode)
            {
                GenerateBorrowBooks(properties.borrowBooks);
            }
            lastNode = currentNode;
            BookSlider.value = 0.5f;
            InfluenceSlider.value = node.NowState().basicInfluence / (float)properties.exposeThreshold;
            //Debug.Log(InfluenceSlider.value);
        }
        NodeInfoPanel.transform.position = mainCamera.WorldToScreenPoint(node.transform.position) + _nodeInfoPanelOffset;
    }

    private void GenerateBorrowBooks(List<BookManager.Book> borrowBooks)
    {
        foreach (Transform child in borrowBooksContent.transform)
        {
            Destroy(child.gameObject);
        }
        if (borrowBooks == null)
        {
            Debug.LogWarning("该书不含有借书单.");
        }
        else
        {
            foreach (BookManager.Book book in borrowBooks)
            {
                GameObject bookItem = Instantiate(borrowBooksItemPrefab, borrowBooksContent.transform);
                var itemInfo = bookItem.GetComponent<BorrowBookItemInfo>();
                itemInfo._book = book;
                itemInfo.UpdateItem();
            }
        }
    }

    public void EnableNodeInfoPanel(NodeBehavior node)
    {   
        if (NodeInfoPanel == null)
        {
            Debug.LogWarning("NodeInfoPanel is not assigned in the inspector.");
            return;
        }

        if (GlobalVar.instance.allowNodeInfoPanel)
        {
            NodeInfoPanel.SetActive(true);
            _changeNodeInfoPanleContenAndPosition(node);
        }
    }
    public void DisableNodeInfoPanel()
    {
        if (NodeInfoPanel == null)
        {
            Debug.LogWarning("NodeInfoPanel is not assigned in the inspector.");
            return;
        }
        if (GlobalVar.instance.NodeInfoPanelIntroductionFinished)
        {
            currentNode = null;
            NodeInfoPanel.SetActive(false);
        }
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

    public void RefreshBookIndex()
    {
        Transform indexPanel = _ResourceUsagePanel.transform.GetChild(7);
        Transform level1 = indexPanel.GetChild(3);
        Transform level2 = indexPanel.GetChild(4);
        Transform level3 = indexPanel.GetChild(5);
        Transform fansi = indexPanel.GetChild(6);
        Transform huanxiang = indexPanel.GetChild(7);
        Transform fankang = indexPanel.GetChild(8);
        Transform shijiao = indexPanel.GetChild(9);
        Transform wangxi = indexPanel.GetChild(10);
        Transform yuyan = indexPanel.GetChild(11);
        Transform zhishi = indexPanel.GetChild(12);
        Color color = new Color(1f, 1f, 1f, 0.117f);
        // 等级
        if (GlobalVar.instance.bookRandomConfig.LevelWeights[0] > 0)
        {
            level1.GetComponent<Image>().color = Color.white;
        }
        else
        {
            level1.GetComponent<Image>().color = color;
        }
        if (GlobalVar.instance.bookRandomConfig.LevelWeights[1] > 0)
        {
            level2.GetComponent<Image>().color = Color.white;
        }
        else
        {
            level2.GetComponent<Image>().color = color;
        }
        if (GlobalVar.instance.bookRandomConfig.LevelWeights[2] > 0)
        {
            level3.GetComponent<Image>().color = Color.white;
        }
        else
        {
            level3.GetComponent<Image>().color = color;
        }
        // 类型
        if (GlobalVar.instance.bookRandomConfig.AllowedTypes.Contains(BookManager.Book.BookType.fansi))
        {
            fansi.GetComponent<Image>().color = Color.white;
        }
        else
        {
            fansi.GetComponent<Image>().color = color;
        }
        if (GlobalVar.instance.bookRandomConfig.AllowedTypes.Contains(BookManager.Book.BookType.huanxiang))
        {
            huanxiang.GetComponent<Image>().color = Color.white;
        }
        else
        {
            huanxiang.GetComponent<Image>().color = color;
        }
        if (GlobalVar.instance.bookRandomConfig.AllowedTypes.Contains(BookManager.Book.BookType.yuyan))
        {
            yuyan.GetComponent<Image>().color = Color.white;
        }
        else
        {
            yuyan.GetComponent<Image>().color = color;
        }
        if (GlobalVar.instance.bookRandomConfig.AllowedTypes.Contains(BookManager.Book.BookType.zhishi))
        {
            zhishi.GetComponent<Image>().color = Color.white;
        }
        else
        {
            zhishi.GetComponent<Image>().color = color;
        }
        if (GlobalVar.instance.bookRandomConfig.AllowedTypes.Contains(BookManager.Book.BookType.wangxi))
        {
            wangxi.GetComponent<Image>().color = Color.white;
        }
        else
        {
            wangxi.GetComponent<Image>().color = color;
        }
        if (GlobalVar.instance.bookRandomConfig.AllowedTypes.Contains(BookManager.Book.BookType.fankang))
        {
            fankang.GetComponent<Image>().color = Color.white;
        }
        else
        {
            fankang.GetComponent<Image>().color = color;
        }
        if (GlobalVar.instance.bookRandomConfig.AllowedTypes.Contains(BookManager.Book.BookType.shijiao))
        {
            shijiao.GetComponent<Image>().color = Color.white;
        }
        else
        {
            shijiao.GetComponent<Image>().color = color;
        }
    }
}

