using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class NodeInfoEditorDisplay : MonoBehaviour
{
    public static NodeInfoEditorDisplay instance;
    private Camera mainCamera;
    private CubeEditorBehavior selectedCB;
    public TextMeshProUGUI NodeName;
    public Slider AwakeThresholdSlider;
    public TextMeshProUGUI AwakeThresholdText;
    public Slider ExposedThresholdSlider;
    public TextMeshProUGUI ExposedThresholdText;
    public Slider FallThresholdSlider;
    public TextMeshProUGUI FallThresholdText;
    public Slider TypeSlider;
    public TextMeshProUGUI TypeText;
    public Slider RegionSlider;
    public TextMeshProUGUI RegionText;

    public BooksData BookListSO;
    public GameObject BookEditorItemPrefab;
    public GameObject BookListScrollViewContent;
    public Dictionary<GameObject,BookManager.Book> BookEditorItemDictionary = new Dictionary<GameObject, BookManager.Book>();
    public TextMeshProUGUI BorrowBooksText;
    
    private bool changing;

    void Start()
    {
        instance = this;
        mainCamera = Camera.main;
        changing = false;
        foreach (var book in BookListSO.books)
        {
            var b = Instantiate(BookEditorItemPrefab, BookListScrollViewContent.transform);
            var t = b.transform.Find("BookText").GetComponent<TextMeshProUGUI>();
            t.text = $"ID - {book.id}\n{book.name}\n{book.type}";
            BookEditorItemDictionary.Add(b,book);
        }
    }

    void Update()
    {
        // 射线检测鼠标指向的物体
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit))
        {
            GameObject hoveredObject = hit.collider.gameObject;

            // 检查物体是否有 CubeBehavior 脚本
            CubeEditorBehavior cubeBehavior = hoveredObject.GetComponent<CubeEditorBehavior>();
            if (cubeBehavior != null)
            {
                // 获取 Properties 数据并展示
                Properties properties = cubeBehavior.properties;
                if (properties != null)
                {
                    changing = true;
                    cubeBehavior.selected = true;
                    if (selectedCB != null && selectedCB != cubeBehavior) selectedCB.selected = false;
                    selectedCB = cubeBehavior;
                    
                    NodeName.text = cubeBehavior.name;
                    AwakeThresholdSlider.value = properties.awakeThreshold;
                    AwakeThresholdText.text = $"觉醒阈值 - {properties.awakeThreshold}";
                    ExposedThresholdSlider.value = properties.exposeThreshold;
                    ExposedThresholdText.text = $"暴露阈值 - {properties.exposeThreshold}";
                    FallThresholdSlider.value = properties.fallThreshold;
                    FallThresholdText.text = $"维持阈值 - {properties.fallThreshold}";
                    TypeSlider.value = (int)(properties.type);
                    TypeText.text = $"节点类型 - {properties.type}";
                    RegionSlider.value = properties.region;
                    RegionText.text = $"所处区域 - {properties.region}";
                    UpdateBookInfo();

                    changing = false;
                }
            }
        }
    }

    public void UpdateInfo()
    {
        if (!changing && selectedCB != null)
        {
            var properties = selectedCB.properties;
            properties.awakeThreshold = (int)AwakeThresholdSlider.value;
            properties.exposeThreshold = (int)ExposedThresholdSlider.value;
            properties.fallThreshold = (int)FallThresholdSlider.value;
            properties.type = (Properties.typeEnum)TypeSlider.value;
            properties.region = (int)RegionSlider.value;
            
            NodeName.text = selectedCB.name;
            AwakeThresholdSlider.value = properties.awakeThreshold;
            AwakeThresholdText.text = $"觉醒阈值 - {properties.awakeThreshold}";
            ExposedThresholdSlider.value = properties.exposeThreshold;
            ExposedThresholdText.text = $"暴露阈值 - {properties.exposeThreshold}";
            FallThresholdSlider.value = properties.fallThreshold;
            FallThresholdText.text = $"维持阈值 - {properties.fallThreshold}";
            TypeSlider.value = (int)(properties.type);
            TypeText.text = $"节点类型 - {properties.type}";
            RegionSlider.value = properties.region;
            RegionText.text = $"所处区域 - {properties.region}";
        }
    }

    public void Select(GameObject bookEI)
    {
        if (selectedCB != null)
        {
            var book = BookEditorItemDictionary[bookEI];
            var beib = bookEI.GetComponent<BookEditorItemBehavior>();
            if (beib.Tag)
            {
                selectedCB.properties.borrowBooks.Remove(book);
            }
            else
            {
                selectedCB.properties.borrowBooks.Add(book);
            }
            UpdateBookInfo();
        }
    }

    void UpdateBookInfo()
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (var book in selectedCB.properties.borrowBooks)
        {
            stringBuilder.Append(book.name + " - ").Append(book.type).Append("\n");
        }

        BorrowBooksText.text = stringBuilder.ToString();
        foreach (var kv in BookEditorItemDictionary)
        {
            if (selectedCB.properties.borrowBooks.Contains(kv.Value))
            {
                kv.Key.GetComponent<BookEditorItemBehavior>().Tag = true;
            }else kv.Key.GetComponent<BookEditorItemBehavior>().Tag = false;
        }
    }
}