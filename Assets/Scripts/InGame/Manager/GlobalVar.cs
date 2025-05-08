using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

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
    public string bookName = "";
    // 需要序列化
    public int maxResourcePoint = 9;
    
    public int infoIncreaseBy = 5;
    public int distanceIncreaseBy = 1;
    public int allocationLimitIncreaseBy = 1;
    public int exposureValuePerResource = 30;
    public bool everReachedPoliceStation = false;
    public bool everReachedFirehouse = false;
    public bool everLearnedAboutDetectiveAndInfo = false;
    public bool everLearnedAboutKeepNodesDontFall = false;
    public bool everAwakeAllNodes = false;
    public bool noStartingPlot = false;
    public bool everReachingMaxExposureValue = false;
    public List<int> nodesAwakendOnce = new List<int>();
    public bool skipCameraOverview = true;
    public int dealStartRound = 0;
    public int nowPlaying = 0;
    public HashSet<int> allBooks = new HashSet<int>();

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
    
    public void AddResourcePoint(int value)
    {
        resourcePoint += value;
        resourcePoint = Math.Min(resourcePoint, maxResourcePoint);
    }
    [SerializeField] private int infoResourcePoint = 0;
    public void InfoResourcePointIncrement(int v)
    {
        if (resourcePoint <= 1)
        {
            Debug.Log("资源点不足");
            return;
        }
        if (!firstUseResourcePoint)
        {
            firstUseResourcePoint = true;
        }
        resourcePoint -= 2;
        probabilityOfNodesInspectingDetective += v;
    }
    
    [SerializeField] private int distanceResourcePoint = 0;
    public void DistanceResourcePointIncrement(int v)
    {
        if (resourcePoint <= 1)
        {
            Debug.Log("资源点不足");
            return;
        }
        if (!firstUseResourcePoint)
        {
            firstUseResourcePoint = true;
        }

        resourcePoint -= 2;
        numOfMaximumBookDeliverRange += v;
    }
    [SerializeField] private int allocationLimitResourcePoint = 0;

    public void AllocationLimitResourcePointIncrement(int v)
    {
        if (resourcePoint <= 1)
        {
            Debug.Log("资源点不足");
            return;
        }
        if (!firstUseResourcePoint)
        {
            firstUseResourcePoint = true;
        }

        resourcePoint -= 2;
        allocationLimit += v;
    }

    public void DecreaseExposureValueByResource()
    {
        if (resourcePoint <= 0)
        {
            Debug.Log("资源点不足");
            return;
        }
        if (!firstUseResourcePoint)
        {
            firstUseResourcePoint = true;
        }

        resourcePoint--;
        globalExposureValue = Math.Max(globalExposureValue - exposureValuePerResource, 0);
    }
    
    public void levelUpBookRandomConfig()
    {
        if (resourcePoint <= 3)
        {
            Debug.Log("资源点不足");
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
    }

    public void getAShipmentOfBooks()
    {
        if (resourcePoint <= 0)
        {
            Debug.Log("");
            return;
        }

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
