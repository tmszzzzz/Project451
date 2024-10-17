using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoundManager : MonoBehaviour
{
    // 单例实例
    public static RoundManager Instance { get; private set; }
    private Camera mainCamera;
    public GameObject textPrefab; // 指向TextMeshProUI预制体
    public GameObject selectedPointerPrefab;
    private GameObject currentelectedPointer;
    public MessageBar messageBar;
    public int roundNum = 1;
    public int allocated = 0;
    public int held = 0;
    public CanvasBehavior canvas;
    public DetectiveBehavior detective;
    public Canvas uiCanvas; // 这是你的UI Canvas
    public Dictionary<GameObject, int> bookAllocationMap; //<node,value>
    [SerializeField] private GameObject startNode = null;
    [SerializeField] private List<BookAllocationItem> allocationItems = new List<BookAllocationItem>(); // 存储分配项的列表
    public GameObject bookAllocationArrow;


    //以下是事件
    public event Action RoundChange;
    public event Action BookAllocationChange;

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
        if (Input.GetMouseButtonDown(0)/* && !EventSystem.current.IsPointerOverGameObject()*/)
        {
            BookAllocation(0);
        }
        // 检测右键点击
        if (Input.GetMouseButtonDown(1)/* && !EventSystem.current.IsPointerOverGameObject()*/)
        {
            BookAllocation(1);
        }
        //BookTexts();
    }
    /*
    void BookAllocation(int mouseButton)
    {
        // 从鼠标位置创建射线
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //Debug.Log(1);
        // 如果射线击中了物体
        if (Physics.Raycast(ray, out hit))
        {
            //Debug.Log(2);
            if (hit.collider != null)
            {
                //Debug.Log(3);
                NodeBehavior nb = hit.collider.GetComponent<NodeBehavior>();
                if (nb != null && mouseButton == 0)
                {
                    //Debug.Log(4);
                    if ((int)nb.properties.state >= 1 && nb.properties.numOfBooks + bookAllocationMap[hit.collider.gameObject] < nb.properties.maximumNumOfBooks && held > 0)
                    {
                        bookAllocationMap[hit.collider.gameObject]++;
                        held--;
                        BookAllocationChange?.Invoke();
                    }
                }
                else if (nb != null && mouseButton == 1)
                {
                    //Debug.Log(5);
                    if ((int)nb.properties.state >= 1 && nb.properties.numOfBooks + bookAllocationMap[hit.collider.gameObject] > 0 && !(GetNeedToAllocate() >= GlobalVar.Instance.allocationLimit && bookAllocationMap[hit.collider.gameObject] <= 0))
                    {
                        bookAllocationMap[hit.collider.gameObject]--;
                        held++;
                        BookAllocationChange?.Invoke();
                    }
                }
            }
        }
    }*/
    struct BookAllocationItem
    {
        public GameObject begin;
        public GameObject end;
        public GameObject arrow;
    }
    void reStartFirstSelection()
    {
        startNode = null;
        Destroy(currentelectedPointer);
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

                // 仅处理右键点击（即书籍的转移）
                if (nb != null && mouseButton == 1)
                {
                    // 处理第一次点击，选择起始节点
                    if (startNode == null)
                    {
                        // 检查起始节点是否有书可以移出
                        if ((int)nb.properties.state >= 1 && nb.properties.numOfBooks + bookAllocationMap[hit.collider.gameObject] > 0)
                        {
                            startNode = hit.collider.gameObject; // 记录起始节点
                            currentelectedPointer = Instantiate(selectedPointerPrefab, startNode.transform.position + new Vector3(0, 5, 0), Quaternion.identity);
                        }
                        else
                        {
                            messageBar.AddMessage("This node is not permitted to be a starting point.");
                            // 起始节点无书，清除选择状态
                            reStartFirstSelection();
                        }
                    }
                    else
                    {
                        // 第二次点击，选择目标节点
                        GameObject targetNode = hit.collider.gameObject;

                        // 先检查反向分配是否存在
                        BookAllocationItem? reverseItem = FindAllocationItem(targetNode, startNode);
                        if (reverseItem != null)
                        {
                            BookAllocationArrow reverseArrowScript = reverseItem.Value.arrow.GetComponent<BookAllocationArrow>();
                            reverseArrowScript.allocationNum--;  // 减少反向分配线的数量
                            bookAllocationMap[startNode]--;
                            bookAllocationMap[targetNode]++;
                            // 如果数量为0，则删除该反向分配线
                            if (reverseArrowScript.allocationNum <= 0)
                            {
                                Destroy(reverseItem.Value.arrow); // 删除箭头对象
                                allocationItems.Remove(reverseItem.Value); // 从列表中移除
                            }

                            List<BookAllocationItem> toBeDeleted = new List<BookAllocationItem>();
                            //在此操作后，检查所有分配线是否合法
                            foreach(var i in allocationItems)
                            {
                                NodeBehavior bnb = i.begin.GetComponent<NodeBehavior>();
                                NodeBehavior enb = i.end.GetComponent<NodeBehavior>();
                                if (bnb.properties.numOfBooks + bookAllocationMap[i.begin] < 0 
                                    || enb.properties.numOfBooks + bookAllocationMap[i.end] > enb.properties.maximumNumOfBooks)
                                {
                                    toBeDeleted.Add(i);
                                }
                            }
                            foreach(var i in toBeDeleted)
                            {
                                allocationItems.Remove(i);
                                int val = i.arrow.GetComponent<BookAllocationArrow>().allocationNum;
                                bookAllocationMap[i.begin] += val;
                                bookAllocationMap[i.end] -= val;


                                Destroy(i.arrow); // 删除箭头对象
                            }
                            // 重置选择状态
                            reStartFirstSelection();

                            // 触发分配变化事件
                            BookAllocationChange?.Invoke();

                        }

                        // 检查目标节点是否满足接收书的条件
                        else if (targetNode != startNode //节点不可重
                            && canvas.CanConnectNodes(startNode,targetNode,GlobalVar.Instance.NumOfMaximumBookDeliverRange) //不可超距离
                            && (int)nb.properties.state >= 1 //需已觉醒
                            && nb.properties.numOfBooks + bookAllocationMap[targetNode] < nb.properties.maximumNumOfBooks //不可达上限
                            && BookAllocationNum() < GlobalVar.Instance.allocationLimit) //分配不可达上限
                        {
                            // 执行书籍的转移
                            bookAllocationMap[startNode]--;
                            bookAllocationMap[targetNode]++;

                            // 查找是否已有对应的分配箭头
                            BookAllocationItem? existingItem = FindAllocationItem(startNode, targetNode);

                            if (existingItem == null)
                            {
                                // 如果不存在对应的箭头，创建新箭头
                                GameObject arrowInstance = Instantiate(bookAllocationArrow);
                                BookAllocationArrow arrowScript = arrowInstance.GetComponent<BookAllocationArrow>();
                                arrowScript.pointA = startNode.transform;
                                arrowScript.pointB = targetNode.transform;
                                arrowScript.allocationNum = 1;  // 初始分配书数量

                                // 添加到allocationItems列表
                                allocationItems.Add(new BookAllocationItem
                                {
                                    begin = startNode,
                                    end = targetNode,
                                    arrow = arrowInstance
                                });
                            }
                            else
                            {
                                // 如果已经存在箭头，更新其书籍数量
                                BookAllocationArrow arrowScript = existingItem.Value.arrow.GetComponent<BookAllocationArrow>();
                                arrowScript.allocationNum++;
                            }


                            // 重置选择状态
                            reStartFirstSelection();

                            // 触发分配变化事件
                            BookAllocationChange?.Invoke();
                        }
                        else
                        {
                            messageBar.AddMessage("This movement is not permitted.");
                            // 目标节点不满足条件，清除选择状态
                            reStartFirstSelection();
                        }
                    }
                }
            }
        }
    }

    private int BookAllocationNum()
    {
        int sum = 0;
        foreach(var i in allocationItems)
        {
            sum += i.arrow.GetComponent<BookAllocationArrow>().allocationNum;
        }
        return sum;
    }

    private BookAllocationItem? FindAllocationItem(GameObject begin, GameObject end)
    {
        // 遍历allocationItems列表，查找是否存在对应的分配箭头
        foreach (var item in allocationItems)
        {
            if (item.begin == begin && item.end == end)
            {
                return item;
            }
        }
        return null; // 如果未找到匹配项，返回null
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
            RoundChange?.Invoke();//回合变更事件
            canvas.RefreshAllNodes();//更新节点状态
            var keys = new List<GameObject>(bookAllocationMap.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                canvas.AddNodeNumOfBooks(keys[i], bookAllocationMap[keys[i]]);
            }//重新分配节点书数量
            //由于更新状态时已经考虑了预分配的书，所以此时先更新后分配书
            roundNum++;//更新回合数
            canvas.RefreshGlobalExposureValue();//更新全局暴露值
            detective.AddGlobalExposureValue();
            detective.DetectiveMove();
            for (int i = 0; i < keys.Count; i++)
            {
                bookAllocationMap[keys[i]] = 0;
            }//清除预分配数据
            foreach(var i in allocationItems)
            {
                Destroy(i.arrow);
            }
            allocationItems.Clear();//清除预分配链
            BookAllocationChange?.Invoke();//分配情况变更事件（暂未使用）
            messageBar.AddMessage("NextRound");//消息提示
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
}
