using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GlobalVar : MonoBehaviour
{
    public static GlobalVar instance;
    //此处存放各类全局需要用到的常量或变量，视游戏进程可以做一定的修改。
    //请注意为了简便，此处的值由成员变量初始化得到，这将导致在此脚本内修改这些参数无效，需要在inspector内修改。
    

    public void AddGlobalExposureValue(int v)
    {
        globalExposureValue = Math.Min(globalExposureValue + v, maxGlobalExposureValue);
    }
    public void RuduceGlobalExposureValue(int v)
    {
        globalExposureValue = Math.Max(globalExposureValue - v, 0);
    }
    
    public int globalExposureValue = 0;
    public int roundNum = 1;
    public int maxGlobalExposureValue = 100;
    public int exposureValueAdditionOfExposedNode = 10;
    public int exposureValueAdditionOfDetective = 2;
    public int exposureValueAccelerationOfDetective = 1;
    public int exposureValueReductionOfNoExposedNode = 5;
    public int allocationLimit = 1;
    public int numOfBibliophileGiveBooks = 1;
    public int numOfFirefighterGiveBooks = 1;
    public int numOfMaximumBookDeliverRange = 2;
    public int numOfDetectiveOnStart = 4;
    public int probabilityOfNodesInspectingDetective = 0;
    public int resourcePoint = 0;
    public float previewExposureValue = 0;
    // 需要序列化
    public int maxResourcePoint = 9;
    public int infoIncreaseBy = 5;
    public int distanceIncreaseBy = 1;
    public int allocationLimitIncreaseBy = 1;
    public int exposureValuePerResource = 30;
    public bool everReachedPoliceStation = false;
    public bool everReachedFirehouse = false;
    public bool everLearnedAboutDetectiveAndInfo = false;
    public bool everLearnedAboutNodeInfoPanel = false;
    public bool everAwakeAllNodes = false;
    public bool noStartingPlot = false;
    public bool everReachingMaxExposureValue = false;
    public List<int> nodesAwakendOnce = new List<int>();
    public bool skipCameraOverview = true;
    public int dealStartRound = 0;
    public int nowPlaying = 0;
    public HashSet<int> allBooks = new HashSet<int>();
    public bool showBookMark;
    public string bookName = "";
    
    // 教程相关
    public bool firstSelectBookMark = false;
    public bool firstAllocation = false;
    public bool firstNext = false;
    public bool openInfoPanel = false;
    public bool closeInfoPanel = false;
    public int allocationSuccess = 0;
    public bool firstCancellAllocation = false;
    public bool firstGetResourcePoint = false;
    public bool firstOpenPointUsage = false;
    public bool firstUseResourcePoint = false;
    public bool chapter1 = false;
    public bool firstPreviewExpose = false;
    public bool detective = false;
    public bool allowNodeInfoPanel = false;
    public bool NodeInfoPanelIntroductionFinished = false;
    public bool allowPlot = true;
    public NodeBehavior nodeBehavior;
    
    
    public BookManager.BookRandomConfig bookRandomConfig = new BookManager.BookRandomConfig
    {
        LevelWeights = new float[] { 1f, 0f, 0f },
        AllowedTypes = new HashSet<BookManager.Book.BookType>()
        {
            BookManager.Book.BookType.fansi,
            BookManager.Book.BookType.huanxiang,
        }
    };
    
    [SerializeField] private List<BookManager.BookRandomConfig> _remainedBookRandomConfig = new List<BookManager.BookRandomConfig>()
    {
        new BookManager.BookRandomConfig
        {
            LevelWeights = new float[] { 0.8f, 0.2f, 0f },
            AllowedTypes = new HashSet<BookManager.Book.BookType>
            {
                BookManager.Book.BookType.fankang,
                BookManager.Book.BookType.fansi,
                BookManager.Book.BookType.huanxiang,
                BookManager.Book.BookType.shijiao,
            }
        },
        new BookManager.BookRandomConfig
        {
            LevelWeights = new float[] { 0.6f, 0.2f, 0.2f },
            AllowedTypes = new HashSet<BookManager.Book.BookType>
            {
                BookManager.Book.BookType.fankang,
                BookManager.Book.BookType.fansi,
                BookManager.Book.BookType.huanxiang,
                BookManager.Book.BookType.shijiao,
                BookManager.Book.BookType.wangxi,
                BookManager.Book.BookType.yuyan,
                BookManager.Book.BookType.zhishi
            }
        },
    };
    
    
    public void AddResourcePoint(int value)
    {
        resourcePoint += value;
        resourcePoint = Math.Min(resourcePoint, maxResourcePoint);
    }
    public void InfoResourcePointIncrement(int v)
    {
        if (resourcePoint <= 1)
        {
            Debug.Log("资源点不足");
            MessageBar.instance.AddMessage("资源点不足.");
            return;
        }
        if (!firstUseResourcePoint)
        {
            firstUseResourcePoint = true;
        }
        resourcePoint -= 2;
        probabilityOfNodesInspectingDetective += v;
        MessageBar.instance.AddMessage("情报覆盖率增加.");
    }
    
    public void DistanceResourcePointIncrement(int v)
    {
        if (resourcePoint <= 1)
        {
            Debug.Log("资源点不足");
            MessageBar.instance.AddMessage("资源点不足.");
            return;
        }
        if (!firstUseResourcePoint)
        {
            firstUseResourcePoint = true;
        }

        resourcePoint -= 2;
        numOfMaximumBookDeliverRange += v;
        
        MessageBar.instance.AddMessage("书籍最大传输距离增加.");
    }

    public void AllocationLimitResourcePointIncrement(int v)
    {
        if (resourcePoint <= 1)
        {
            Debug.Log("资源点不足");
            MessageBar.instance.AddMessage("资源点不足.");
            return;
        }
        if (!firstUseResourcePoint)
        {
            firstUseResourcePoint = true;
        }

        resourcePoint -= 2;
        allocationLimit += v;
        MessageBar.instance.AddMessage("每日可分配书籍上限增加.");
    }

    public void DecreaseExposureValueByResource()
    {
        if (resourcePoint <= 0)
        {
            Debug.Log("资源点不足");
            MessageBar.instance.AddMessage("资源点不足.");
            return;
        }
        if (!firstUseResourcePoint)
        {
            firstUseResourcePoint = true;
        }

        resourcePoint--;
        globalExposureValue = Math.Max(globalExposureValue - exposureValuePerResource, 0);
        
        MessageBar.instance.AddMessage("暴露值已减少.");
    }
    
    public void levelUpBookRandomConfig()
    {
        if (resourcePoint <= 3)
        {
            Debug.Log("资源点不足");
            MessageBar.instance.AddMessage("资源点不足.");
            return;
        }
        
        
        if (_remainedBookRandomConfig.Count == 0)
        {
            Debug.Log("没有更多的书籍随机配置可用");
            return;
        }
        
        bookRandomConfig = _remainedBookRandomConfig[0];
        
        // 从列表中移除已使用的配置
        _remainedBookRandomConfig.RemoveAt(0);
        MessageBar.instance.AddMessage("索引整理成功.");
    }

    public void getAShipmentOfBooks()
    {
        if (resourcePoint <= 0)
        {
            Debug.Log("资源点不足");
            MessageBar.instance.AddMessage("资源点不足.");
            return;
        }
        List<GameObject> nodes = new List<GameObject>();
        foreach (var n in CanvasBehavior.instance.GetNodeList())
        {
            if (n.GetComponent<NodeBehavior>().NowState().state > 0)
            {
                nodes.Add(n);
            }
        }
        if (nodes.Count < 3)
        {
            Debug.LogWarning("已转化节点少于三个");
            return;
        }
        int n1 = Random.Range(0, nodes.Count);
        nodes[n1].GetComponent<NodeBehavior>().AddABook(BookManager.instance.GetRandomBook());
        MessageBar.instance.AddMessage(NameManager.instance.ConvertNodeNameToName(nodes[n1].name) + "获得了1本书");
        int n2 = Random.Range(0, nodes.Count);
        nodes[n2].GetComponent<NodeBehavior>().AddABook(BookManager.instance.GetRandomBook());
        MessageBar.instance.AddMessage(NameManager.instance.ConvertNodeNameToName(nodes[n2].name) + "获得了1本书");
        int n3 = Random.Range(0, nodes.Count);
        nodes[n3].GetComponent<NodeBehavior>().AddABook(BookManager.instance.GetRandomBook());
        MessageBar.instance.AddMessage(NameManager.instance.ConvertNodeNameToName(nodes[n3].name) + "获得了1本书");
        resourcePoint -= 1;
    }
    

    //采用单例模式，任意代码段可通过类名的静态变量Instance引用此唯一实例。
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
    }

    public void Test1()
    {
        Debug.Log("Test1");
    }
    
}
