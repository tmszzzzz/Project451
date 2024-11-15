using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoundManager : MonoBehaviour
{
    // 单例实例
    public static RoundManager instance;
    public CameraBehavior mainCamera;
    public GameObject textPrefab; // 指向TextMeshProUI预制体
    public GameObject selectedPointerPrefab;
    private GameObject _currentSelectedPointer;
    public MessageBar messageBar;
    public int allocated = 0;
    public int held = 0;
    public CanvasBehavior canvas;
    public DetectiveBehavior detective;
    public Canvas uiCanvas; // 这是你的UI Canvas
    public Dictionary<GameObject, int> BookAllocationMap; //<node,value>
    [SerializeField] private GameObject startNode = null;
    public List<BookAllocationItem> BookAllocationItems = new List<BookAllocationItem>(); // 存储分配项的列表
    public GameObject bookAllocationArrow;
    public GameObject DownFx;
    public GameObject ActiveFx;
    public GameObject ExposeFx;
    [SerializeField] private GameObject forbidden;
    public bool operationForbidden = false;
    private int forbidTag = 0;


    //以下是事件
    public event Action RoundChange;
    public event Action BookAllocationChange;

    // 场景初始化时调用
    private void Awake()
    {
        // 如果已有实例且不是当前实例，销毁当前实例，确保单例唯一性
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 将当前实例设为单例实例
        instance = this;

        // 选择保留这个对象，使其在场景切换时不会被销毁
        BookAllocationMap = new Dictionary<GameObject, int>();
        foreach (var i in canvas.GetNodeList())
        {
            BookAllocationMap.Add(i, 0);
        }
    }
    
    public struct BookAllocationItem
    {
        public GameObject Begin;
        public GameObject End;
        public GameObject Arrow;

    }
    void RestartFirstSelection()
    {
        startNode = null;
        Destroy(_currentSelectedPointer);
    }

    public void BookAllocation(int mouseButton, RaycastHit hit)
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
                    if ((int)nb.properties.state >= 1 && nb.properties.numOfBooks + BookAllocationMap[hit.collider.gameObject] > 0)
                    {
                        startNode = hit.collider.gameObject; // 记录起始节点
                        _currentSelectedPointer = Instantiate(selectedPointerPrefab, startNode.transform.position + new Vector3(0, 5, 0), Quaternion.identity);
                    }
                    else
                    {
                        messageBar.AddMessage("This node is not permitted to be a starting point.");
                        // 起始节点无书，清除选择状态
                        RestartFirstSelection();
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
                        BookAllocationArrow reverseArrowScript = reverseItem.Value.Arrow.GetComponent<BookAllocationArrow>();
                        reverseArrowScript.allocationNum--;  // 减少反向分配线的数量
                        BookAllocationMap[startNode]--;
                        BookAllocationMap[targetNode]++;
                        // 如果数量为0，则删除该反向分配线
                        if (reverseArrowScript.allocationNum <= 0)
                        {
                            reverseItem.Value.Arrow.GetComponent<BookAllocationArrow>().Cancel();
                            BookAllocationItems.Remove(reverseItem.Value); // 从列表中移除
                        }

                        List<BookAllocationItem> toBeDeleted = new List<BookAllocationItem>();
                        //在此操作后，检查所有分配线是否合法
                        foreach (var i in BookAllocationItems)
                        {
                            NodeBehavior bnb = i.Begin.GetComponent<NodeBehavior>();
                            NodeBehavior enb = i.End.GetComponent<NodeBehavior>();
                            if (bnb.properties.numOfBooks + BookAllocationMap[i.Begin] < 0
                                || enb.properties.numOfBooks + BookAllocationMap[i.End] > enb.properties.maximumNumOfBooks)
                            {
                                toBeDeleted.Add(i);
                            }
                        }
                        foreach (var i in toBeDeleted)
                        {
                            BookAllocationItems.Remove(i);
                            var allocationArrow = i.Arrow.GetComponent<BookAllocationArrow>();
                            int val = allocationArrow.allocationNum;
                            BookAllocationMap[i.Begin] += val;
                            BookAllocationMap[i.End] -= val;


                            allocationArrow.Cancel();
                        }
                        // 重置选择状态
                        RestartFirstSelection();

                        // 触发分配变化事件
                        BookAllocationChange?.Invoke();

                    }

                    // 检查目标节点是否满足接收书的条件
                    else if (targetNode != startNode //节点不可重
                        && canvas.CanConnectNodes(startNode, targetNode, GlobalVar.instance.numOfMaximumBookDeliverRange) //不可超距离
                        && (int)nb.properties.state >= 1 //需已觉醒
                        && nb.properties.numOfBooks + BookAllocationMap[targetNode] < nb.properties.maximumNumOfBooks //不可达上限
                        && BookAllocationNum() < GlobalVar.instance.allocationLimit) //分配不可达上限
                    {
                        // 执行书籍的转移
                        BookAllocationMap[startNode]--;
                        BookAllocationMap[targetNode]++;

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
                            BookAllocationItems.Add(new BookAllocationItem
                            {
                                Begin = startNode,
                                End = targetNode,
                                Arrow = arrowInstance
                            });
                        }
                        else
                        {
                            // 如果已经存在箭头，更新其书籍数量
                            BookAllocationArrow arrowScript = existingItem.Value.Arrow.GetComponent<BookAllocationArrow>();
                            arrowScript.allocationNum++;
                        }


                        // 重置选择状态
                        RestartFirstSelection();
                    }
                    else
                    {
                        messageBar.AddMessage("This movement is not permitted.");
                        // 目标节点不满足条件，清除选择状态
                        RestartFirstSelection();
                    }
                }
            }
        }

    }

    public int BookAllocationNum()
    {
        int sum = 0;
        foreach(var i in BookAllocationItems)
        {
            sum += i.Arrow.GetComponent<BookAllocationArrow>().allocationNum;
        }
        return sum;
    }

    private BookAllocationItem? FindAllocationItem(GameObject begin, GameObject end)
    {
        // 遍历allocationItems列表，查找是否存在对应的分配箭头
        foreach (var item in BookAllocationItems)
        {
            if (item.Begin == begin && item.End == end)
            {
                return item;
            }
        }
        return null; // 如果未找到匹配项，返回null
    }

    public int GetNeedToAllocate()
    {
        int v = 0;
        var keys = new List<GameObject>(BookAllocationMap.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            if (BookAllocationMap[keys[i]] < 0) v -= BookAllocationMap[keys[i]];

        }
        return v;
    }

    //public bool skipCameraOverview = true;

    public async void NextRound()
    {
        PanelController.instance.DisableNodeInfoPanel();
        OperationForbidden();//屏蔽所有操作
        forbidden.SetActive(true);
        
        //这一段代码精确地控制了一些逻辑的触发顺序，可调整
        RoundChange?.Invoke();
        bool skipCameraOverviewTemp = GlobalVar.instance.skipCameraOverview;
        if (!skipCameraOverviewTemp)
            await mainCamera.OverviewEnter();


        foreach (var i in BookAllocationItems)
        {
            var arrow = i.Arrow.GetComponent<BookAllocationArrow>();
            arrow.Confirm();
            arrow.displayNum = false;
        }
        //执行分配动画


        await canvas.RefreshAllNodes();//更新节点状态


        BookAllocationChange?.Invoke();//分配情况变更事件


        await detective.AddGlobalExposureValue();//侦探依据预分配数据判定增加暴露值


        canvas.RefreshGlobalExposureValue();//依据当前节点状态更新全局暴露值


        detective.DetectiveMove();//侦探移动


        canvas.RefreshAllConnections();//连接数据更新


        var keys = new List<GameObject>(BookAllocationMap.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            canvas.AddNodeNumOfBooks(keys[i], BookAllocationMap[keys[i]]);
        }//执行分配
         //由于更新状态时已经考虑了预分配的书，所以这里先更新后分配书。这里的分配书实际上没有逻辑上的影响。


        GlobalVar.instance.roundNum++;//更新回合数


        for (int i = 0; i < keys.Count; i++)
        {
            BookAllocationMap[keys[i]] = 0;
        }
        foreach (var i in BookAllocationItems)
        {
            Destroy(i.Arrow.gameObject);
        }
        BookAllocationItems.Clear();//清除预分配数据


        messageBar.AddMessage($"第{GlobalVar.instance.roundNum-1}天过去了.");//消息提示


        await Task.Delay(1000);
        
        if (!skipCameraOverviewTemp)
            await mainCamera.OverviewExit();
        
        await PlotManager.instance.ReadPlotQueue();
        
        OperationRelease();//释放操作屏蔽
        forbidden.SetActive(false);
    }

    public void OperationForbidden()
    {
        forbidTag++;
        if (forbidTag > 0)
        {
            operationForbidden = true;
        }
    }
    
    public void OperationRelease()
    {
        forbidTag--;
        if (forbidTag <= 0)
        {
            operationForbidden = false;
        }
    }
    
    
    public void LimitIncreaseBy(int i)
    {
        GlobalVar.instance.allocationLimit += i;
    }
    public void BookNumOfMeIncreaseBy(int i)
    {
        canvas.Me.GetComponent<NodeBehavior>().properties.numOfBooks++;
    }
}
