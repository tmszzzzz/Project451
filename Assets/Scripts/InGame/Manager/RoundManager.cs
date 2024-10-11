using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoundManager : MonoBehaviour
{
    // 单例实例
    public static RoundManager Instance { get; private set; }
    private Camera mainCamera;
    public GameObject textPrefab; // 指向TextMeshProUI预制体
    public MessageBar messageBar;
    public int roundNum = 1;
    public int allocated = 0;
    public int held = 0;
    public CanvasBehavior canvas;
    public Canvas uiCanvas; // 这是你的UI Canvas
    public Dictionary<GameObject, int> bookAllocationMap; //<node,value>
    private Dictionary<GameObject, GameObject> activeTextMap = new Dictionary<GameObject, GameObject>();

    //以下是事件
    public event Action OnRoundChange;

    // 场景初始化时调用
    private void Awake()
    {
        // 如果已有实例且不是当前实例，销毁当前实例，确保单例唯一性
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 将当前实例设为单例实例
        Instance = this;

        // 选择保留这个对象，使其在场景切换时不会被销毁
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        mainCamera = Camera.main;
        bookAllocationMap = new Dictionary<GameObject, int>();
        foreach (var i in canvas.GetNodeList())
        {
            bookAllocationMap.Add(i, 0);
        }
    }
    private void Update()
    {
        // 检测左键点击
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            BookAllocation(0);
        }
        // 检测右键点击
        if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
        {
            BookAllocation(1);
        }
        BookTexts();
    }
    void BookAllocation(int mouseButton)
    {
        // 从鼠标位置创建射线
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 如果射线击中了物体
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null)
            {
                NodeBehavior nb = hit.collider.GetComponent<NodeBehavior>();
                if (nb != null && mouseButton == 0)
                {
                    if ((int)nb.properties.state >= 1 && nb.properties.numOfBooks + bookAllocationMap[hit.collider.gameObject] < nb.properties.maximumNumOfBooks && held > 0)
                    {
                        bookAllocationMap[hit.collider.gameObject]++;
                        held--;
                    }
                }
                else if (nb != null && mouseButton == 1)
                {
                    if ((int)nb.properties.state >= 1 && nb.properties.numOfBooks + bookAllocationMap[hit.collider.gameObject] > 0 && !(GetNeedToAllocate() >= GlobalVar.Instance.allocationLimit && bookAllocationMap[hit.collider.gameObject] <= 0))
                    {
                        bookAllocationMap[hit.collider.gameObject]--;
                        held++;
                    }
                }
            }
        }
    }
    public int GetNeedToAllocate()
    {
        int v = 0;
        var keys = new List<GameObject>(bookAllocationMap.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            if (bookAllocationMap[keys[i]] < 0) v -= bookAllocationMap[keys[i]];

        }
        return v;
    }

    public void NextRound()
    {
        if (held == 0)
        {
            //这一段代码精确地控制了一些逻辑的触发顺序，可调整
            OnRoundChange?.Invoke();
            var keys = new List<GameObject>(bookAllocationMap.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                canvas.AddNodeNumOfBooks(keys[i], bookAllocationMap[keys[i]]);
            }
            roundNum++;
            canvas.RefreshGlobalExposureValue();
            canvas.RefreshAllNodes();
            for (int i = 0; i < keys.Count; i++)
            {
                bookAllocationMap[keys[i]] = 0;
            }
            messageBar.AddMessage("NextRound");
        }
        else
        {
            messageBar.AddMessage("There are still books that have not been assigned.");
        }
    }
    public void LimitIncreaseBy(int i)
    {
        GlobalVar.Instance.allocationLimit += i;
    }
    public void BookNumOfMeIncreaseBy(int i)
    {
        canvas.Me.GetComponent<NodeBehavior>().properties.numOfBooks++;
    }

    private void BookTexts()
    {
        // 遍历bookAllocationMap的所有项
        foreach (var entry in bookAllocationMap)
        {
            GameObject node = entry.Key;
            int bookCount = entry.Value;

            if (bookCount != 0)
            {
                // 如果该Node的文本尚未生成，则生成之
                if (!activeTextMap.ContainsKey(node))
                {
                    // 创建并显示文本
                    GameObject textObj = Instantiate(textPrefab, node.transform.position, Quaternion.LookRotation(node.transform.position - Camera.main.transform.position), node.transform);
                    TextMeshPro textComponent = textObj.GetComponent<TextMeshPro>();
                    if (textComponent != null)
                    {
                        textComponent.text = bookCount.ToString();
                    }

                    // 存储当前文本对象
                    activeTextMap[node] = textObj;
                }
                else
                {
                    // 如果已经有文本，更新文本内容
                    TextMeshPro textComponent = activeTextMap[node].GetComponent<TextMeshPro>();
                    if (textComponent != null)
                    {
                        textComponent.text = bookCount.ToString();
                    }
                }
            }
            else
            {
                // 如果当前的bookCount为0且文本正在显示，则销毁文本
                if (activeTextMap.ContainsKey(node))
                {
                    Destroy(activeTextMap[node]);
                    activeTextMap.Remove(node);
                }
            }
        }
    }
}
