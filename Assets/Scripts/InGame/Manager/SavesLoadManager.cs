using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SavesLoadManager : MonoBehaviour
{
    public static SavesLoadManager instance;
    private CanvasBehavior canvas;
    private DetectiveBehavior detective;
    [SerializeField] private Journal book;
    [SerializeField] private List<Sprite> SpriteList;


    [System.Serializable]
    public class SerializableGlobalVar
    {
        public int globalExposureValue;
        public int roundNum;
        public int maxGlobalExposureValue;
        public int exposureValueAdditionOfExposedNode;
        public int exposureValueAdditionOfDetective;
        public int exposureValueAccelerationOfDetective;
        public int exposureValueReductionOfNoExposedNode;
        public int allocationLimit;
        public int numOfBibliophileGiveBooks;
        public int numOfFirefighterGiveBooks;
        public int numOfMaximumBookDeliverRange;
        public int numOfDetectiveOnStart;
        public int probabilityOfNodesInspectingDetective;
        public int resourcePoint;
        public float previewExposureValue;
        public int maxResourcePoint;
        public int infoIncreaseBy;
        public int distanceIncreaseBy;
        public int allocationLimitIncreaseBy;
        public int exposureValuePerResource;
        public bool everReachedPoliceStation;
        public bool everReachedFirehouse;
        public bool everLearnedAboutDetectiveAndInfo;
        public bool everLearnedAboutNodeInfoPanel;
        public bool everAwakeAllNodes;
        public bool noStartingPlot;
        public bool everReachingMaxExposureValue;
        public List<int> nodesAwakendOnce;
        public bool skipCameraOverview;
        public int dealStartRound;
        public int nowPlaying;
        public List<int> allBooks;
        public bool showBookMark;
        public string bookName;
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
        public string nodeName;
        public int currentTask;
        public float exposureCoefficient;
        public BookManager.BookRandomConfig bookRandomConfig;
        public List<BookManager.BookRandomConfig> _remainedBookRandomConfig;
        
        // 构造函数，用于从 GlobalVar 初始化
        public SerializableGlobalVar(GlobalVar globalVar)
        {
            this.globalExposureValue = globalVar.globalExposureValue;
            this.roundNum = globalVar.roundNum;
            this.maxGlobalExposureValue = globalVar.maxGlobalExposureValue;
            this.exposureValueAdditionOfExposedNode = globalVar.exposureValueAdditionOfExposedNode;
            this.exposureValueAdditionOfDetective = globalVar.exposureValueAdditionOfDetective;
            this.exposureValueAccelerationOfDetective = globalVar.exposureValueAccelerationOfDetective;
            this.exposureValueReductionOfNoExposedNode = globalVar.exposureValueReductionOfNoExposedNode;
            this.allocationLimit = globalVar.allocationLimit;
            this.numOfBibliophileGiveBooks = globalVar.numOfBibliophileGiveBooks;
            this.numOfFirefighterGiveBooks = globalVar.numOfFirefighterGiveBooks;
            this.numOfMaximumBookDeliverRange = globalVar.numOfMaximumBookDeliverRange;
            this.numOfDetectiveOnStart = globalVar.numOfDetectiveOnStart;
            this.probabilityOfNodesInspectingDetective = globalVar.probabilityOfNodesInspectingDetective;
            this.resourcePoint = globalVar.resourcePoint;
            this.previewExposureValue = globalVar.previewExposureValue;
            this.maxResourcePoint = globalVar.maxResourcePoint;
            this.infoIncreaseBy = globalVar.infoIncreaseBy;
            this.distanceIncreaseBy = globalVar.distanceIncreaseBy;
            this.allocationLimitIncreaseBy = globalVar.allocationLimitIncreaseBy;
            this.exposureValuePerResource = globalVar.exposureValuePerResource;
            this.everReachedPoliceStation = globalVar.everReachedPoliceStation;
            this.everReachedFirehouse = globalVar.everReachedFirehouse;
            this.everLearnedAboutDetectiveAndInfo = globalVar.everLearnedAboutDetectiveAndInfo;
            this.everLearnedAboutNodeInfoPanel = globalVar.everLearnedAboutNodeInfoPanel;
            this.everAwakeAllNodes = globalVar.everAwakeAllNodes;
            this.noStartingPlot = globalVar.noStartingPlot;
            this.everReachingMaxExposureValue = globalVar.everReachingMaxExposureValue;
            this.nodesAwakendOnce = new List<int>(globalVar.nodesAwakendOnce);
            this.skipCameraOverview = globalVar.skipCameraOverview;
            this.dealStartRound = globalVar.dealStartRound;
            this.nowPlaying = globalVar.nowPlaying;
            this.allBooks = new List<int>(globalVar.allBooks);
            this.showBookMark = globalVar.showBookMark;
            this.bookName = globalVar.bookName;
            this.firstSelectBookMark = globalVar.firstSelectBookMark;
            this.firstAllocation = globalVar.firstAllocation;
            this.firstNext = globalVar.firstNext;
            this.openInfoPanel = globalVar.openInfoPanel;
            this.closeInfoPanel = globalVar.closeInfoPanel;
            this.allocationSuccess = globalVar.allocationSuccess;
            this.firstCancellAllocation = globalVar.firstCancellAllocation;
            this.firstGetResourcePoint = globalVar.firstGetResourcePoint;
            this.firstOpenPointUsage = globalVar.firstOpenPointUsage;
            this. firstUseResourcePoint = globalVar.firstUseResourcePoint;
            this.chapter1 = globalVar.chapter1;
            this.firstPreviewExpose = globalVar.firstPreviewExpose;
            this.detective = globalVar.detective;
            this.allowNodeInfoPanel = globalVar.allowNodeInfoPanel;
            this.NodeInfoPanelIntroductionFinished = globalVar.NodeInfoPanelIntroductionFinished;
            this.allowPlot = globalVar.allowPlot;
            this.nodeName = globalVar.nodeName;
            this.currentTask = globalVar.currentTask;
            this.exposureCoefficient = globalVar.exposureCoefficient;
            this.bookRandomConfig = new BookManager.BookRandomConfig(globalVar.bookRandomConfig);
            this._remainedBookRandomConfig = new List<BookManager.BookRandomConfig>(globalVar._remainedBookRandomConfig);
        }
    }


    [System.Serializable]
    public class SerializableNodeBehavior
    {
        public Properties.StateEnum state;
        // public int numOfBooks;
        public List<BookManager.Book> books;
        public bool hadAwakenedBefore;
        public bool isPaging;

        public SerializableNodeBehavior(NodeBehavior nodeBehavior)
        {
            this.state = nodeBehavior.properties.state;
            this.books = new(nodeBehavior.properties.books);
            for (int i = 0; i < this.books.Count; i++)
            {
                if (this.books[i].isPreallocatedIn)
                {
                    this.books.Remove(this.books[i]);
                    continue;
                }
                if (this.books[i].isPreallocatedOut)
                {
                    this.books[i].isPreallocatedOut = false;
                }
            }
            this.hadAwakenedBefore = nodeBehavior.hadAwakenedBefore;
            this.isPaging = nodeBehavior.plotAndPageHandler.isPaging;
        }
    }


    [System.Serializable]
    public class SerializableConnectionBehavior
    {
        public int unlockTag;
        public bool available;
        public int unlockState;
        public bool isDisplayingInfo;

        public SerializableConnectionBehavior(ConnectionBehavior connectionBehavior)
        {
            unlockTag = connectionBehavior.unlockTag;
            isDisplayingInfo = connectionBehavior.isDisplayingInfo;
            if (connectionBehavior is UnlockableConnectionBehavior unlockableConnectionBehavior)
            {
                available = unlockableConnectionBehavior.available;
                unlockState = unlockableConnectionBehavior.unlockState;
            }
            else
            {
                available = false;
                unlockState = 0;
            }
        }
    }


    [System.Serializable]
    public class SerializableDetectiveBehavior
    {
        public List<int> focusOnNodes;

        public SerializableDetectiveBehavior(DetectiveBehavior detectiveBehavior)
        {
            var nodes = new List<int>();
            foreach (var node in detectiveBehavior.focusOnNodes)
            {
                nodes.Add(int.Parse(node.name.Substring(5)));
            }

            focusOnNodes = nodes;
        }
    }
    
    [System.Serializable]
    public class SerializableQuests
    {
        public bool q1;
        public bool q2;
        public bool q3;
        public bool q4;
        public bool q5;
        public bool q6;

        public SerializableQuests(QuestPanel questPanel)
        {
            q1 = false;
            q2 = false;
            q3 = false;
            q4 = false;
            q5 = false;
            q6 = false;
            foreach (int i in questPanel.questIds)
            {
                switch (i)
                {
                    case 0:
                        q1 = true;
                        break;
                    case 1:
                        q2 = true;
                        break;
                    case 2:
                        q3 = true;
                        break;
                    case 3:
                        q4 = true;
                        break;
                    case 4:
                        q5 = true;
                        break;
                    case 5:
                        q6 = true;
                        break;
                }
            }
        }
    }

    [System.Serializable]
    public class SerializableBookPages
    {
        public List<int> PageIds;

        public SerializableBookPages(Journal book,List<Sprite> sprites)
        {
            PageIds = new List<int>();
            foreach (var page in book.bookPages)
            {
                int p = sprites.IndexOf(page);
                if (p != -1)PageIds.Add(p);
            }
        }
    }


    [System.Serializable]
    public class SerializableAll
    {
        public SerializableGlobalVar globalVar;
        public List<SerializableNodeBehavior> nodeBehaviors;
        public List<SerializableConnectionBehavior> connectionBehaviors;
        public SerializableDetectiveBehavior detectiveBehavior;
        public SerializableQuests quests;
        public SerializableBookPages bookPages;
        public SerializableAll(GlobalVar globalVar, CanvasBehavior canvas, DetectiveBehavior detective,QuestPanel quest,Journal book,List<Sprite> sprites)
        {
            this.globalVar = new SerializableGlobalVar(globalVar);
            var sNodeL = new List<SerializableNodeBehavior>();
            var nodeL = canvas.GetNodeList();
            foreach (var node in nodeL)
            {
                sNodeL.Add(new SerializableNodeBehavior(node.GetComponent<NodeBehavior>()));
            }

            nodeBehaviors = sNodeL;
            var sConL = new List<SerializableConnectionBehavior>();
            var conL = canvas.GetConnectionList();
            foreach (var con in conL)
            {
                sConL.Add(new SerializableConnectionBehavior(con.GetComponent<ConnectionBehavior>()));
            }

            connectionBehaviors = sConL;
            detectiveBehavior = new SerializableDetectiveBehavior(detective);
            quests = new SerializableQuests(quest);
            bookPages = new SerializableBookPages(book, sprites);
        }
    }

    public void SerializeAll()
    {
        // 使用包装类创建一个可序列化的对象
        SerializableAll serializableData =
            new SerializableAll(GlobalVar.instance, canvas, detective, QuestPanel.instance, book, SpriteList);

        // 使用 JsonUtility 序列化为 JSON 字符串
        string json = JsonUtility.ToJson(serializableData);

        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string saveFilePath;
        if (GameLoader.instance == null)
        {
            saveFilePath = "Assets/Saves/save.json";
        }
        else
        {
            saveFilePath = $"{GameLoader.instance.loadFilePath}"; // 添加时间戳到文件名
        }

        System.IO.File.WriteAllText(saveFilePath, json);
    }
    
    public void AutoSerializeAll(int round)
    {
        Debug.Log("AutoSerializeAll");
        // 使用包装类创建一个可序列化的对象
        SerializableAll serializableData =
            new SerializableAll(GlobalVar.instance, canvas, detective, QuestPanel.instance, book, SpriteList);

        // 使用 JsonUtility 序列化为 JSON 字符串
        string json = JsonUtility.ToJson(serializableData);

        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string saveFilePath;
        
        saveFilePath = $"Assets/Saves/autosaves/save_{round}_{timestamp}.json";

        System.IO.File.WriteAllText(saveFilePath, json);
        string directoryPath = "Assets/Saves/autosaves";
        // 检查目录是否存在
        if (!Directory.Exists(directoryPath))
        {
            return;
        }

        // 获取目录下的所有文件
        FileInfo[] files = new DirectoryInfo(directoryPath).GetFiles();

        // 检查文件数量是否超限
        if (files.Length > 5)
        {
            // 按文件创建时间升序排序
            FileInfo oldestFile = files.OrderBy(f => f.CreationTime).First();

            Console.WriteLine($"删除文件: {oldestFile.FullName}");
            try
            {
                oldestFile.Delete(); // 删除最早的文件
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }
    }

    public void DeserializeAll(string filePath)
    {
        Debug.Log(filePath);
        if (System.IO.File.Exists(filePath))
        {
            string json = System.IO.File.ReadAllText(filePath);
            SerializableAll deserializedData = JsonUtility.FromJson<SerializableAll>(json);


            SerializableGlobalVar serializableGlobalVar = deserializedData.globalVar;
            GlobalVar.instance.globalExposureValue = serializableGlobalVar.globalExposureValue;
            GlobalVar.instance.roundNum = serializableGlobalVar.roundNum;
            GlobalVar.instance.maxGlobalExposureValue = serializableGlobalVar.maxGlobalExposureValue;
            GlobalVar.instance.exposureValueAdditionOfExposedNode =
                serializableGlobalVar.exposureValueAdditionOfExposedNode;
            GlobalVar.instance.exposureValueAdditionOfDetective =
                serializableGlobalVar.exposureValueAdditionOfDetective;
            GlobalVar.instance.exposureValueAccelerationOfDetective =
                serializableGlobalVar.exposureValueAccelerationOfDetective;
            GlobalVar.instance.exposureValueReductionOfNoExposedNode =
                serializableGlobalVar.exposureValueReductionOfNoExposedNode;
            GlobalVar.instance.allocationLimit = serializableGlobalVar.allocationLimit;
            GlobalVar.instance.numOfBibliophileGiveBooks = serializableGlobalVar.numOfBibliophileGiveBooks;
            GlobalVar.instance.numOfFirefighterGiveBooks = serializableGlobalVar.numOfFirefighterGiveBooks;
            GlobalVar.instance.numOfMaximumBookDeliverRange = serializableGlobalVar.numOfMaximumBookDeliverRange;
            GlobalVar.instance.numOfDetectiveOnStart = serializableGlobalVar.numOfDetectiveOnStart;
            GlobalVar.instance.probabilityOfNodesInspectingDetective =
                serializableGlobalVar.probabilityOfNodesInspectingDetective;
            GlobalVar.instance.resourcePoint = serializableGlobalVar.resourcePoint;
            GlobalVar.instance.previewExposureValue = serializableGlobalVar.previewExposureValue;
            GlobalVar.instance.maxResourcePoint = serializableGlobalVar.maxResourcePoint;
            GlobalVar.instance.infoIncreaseBy = serializableGlobalVar.infoIncreaseBy;
            GlobalVar.instance.distanceIncreaseBy = serializableGlobalVar.distanceIncreaseBy;
            GlobalVar.instance.allocationLimitIncreaseBy = serializableGlobalVar.allocationLimitIncreaseBy;
            GlobalVar.instance.exposureValuePerResource = serializableGlobalVar.exposureValuePerResource;
            GlobalVar.instance.everReachedPoliceStation = serializableGlobalVar.everReachedPoliceStation;
            GlobalVar.instance.everReachedFirehouse = serializableGlobalVar.everReachedFirehouse;
            GlobalVar.instance.everLearnedAboutDetectiveAndInfo =
                serializableGlobalVar.everLearnedAboutDetectiveAndInfo;
            GlobalVar.instance.everLearnedAboutNodeInfoPanel =
                serializableGlobalVar.everLearnedAboutNodeInfoPanel;
            GlobalVar.instance.everAwakeAllNodes = serializableGlobalVar.everAwakeAllNodes;
            GlobalVar.instance.noStartingPlot = serializableGlobalVar.noStartingPlot;
            GlobalVar.instance.everReachingMaxExposureValue = serializableGlobalVar.everReachingMaxExposureValue;
            GlobalVar.instance.nodesAwakendOnce = new List<int>(serializableGlobalVar.nodesAwakendOnce);
            GlobalVar.instance.skipCameraOverview = serializableGlobalVar.skipCameraOverview;
            GlobalVar.instance.dealStartRound = serializableGlobalVar.dealStartRound;
            GlobalVar.instance.nowPlaying = serializableGlobalVar.nowPlaying;
            GlobalVar.instance.allBooks = new List<int>(serializableGlobalVar.allBooks);
            GlobalVar.instance.showBookMark = serializableGlobalVar.showBookMark;
            GlobalVar.instance.bookName = serializableGlobalVar.bookName;
            GlobalVar.instance.currentTask = serializableGlobalVar.currentTask;
            GlobalVar.instance.firstSelectBookMark = serializableGlobalVar.firstSelectBookMark;
            GlobalVar.instance.firstAllocation = serializableGlobalVar.firstAllocation;
            GlobalVar.instance.firstNext = serializableGlobalVar.firstNext;
            GlobalVar.instance.openInfoPanel = serializableGlobalVar.openInfoPanel;
            GlobalVar.instance.closeInfoPanel = serializableGlobalVar.closeInfoPanel;
            GlobalVar.instance.allocationSuccess = serializableGlobalVar.allocationSuccess;
            GlobalVar.instance.firstCancellAllocation = serializableGlobalVar.firstCancellAllocation;
            GlobalVar.instance.firstGetResourcePoint = serializableGlobalVar.firstGetResourcePoint;
            GlobalVar.instance.firstOpenPointUsage = serializableGlobalVar.firstOpenPointUsage;
            GlobalVar.instance.firstUseResourcePoint = serializableGlobalVar.firstUseResourcePoint;
            GlobalVar.instance.chapter1 = serializableGlobalVar.chapter1;
            GlobalVar.instance.firstPreviewExpose = serializableGlobalVar.firstPreviewExpose;
            GlobalVar.instance.detective = serializableGlobalVar.detective;
            GlobalVar.instance.allowNodeInfoPanel = serializableGlobalVar.allowNodeInfoPanel;
            GlobalVar.instance.NodeInfoPanelIntroductionFinished = serializableGlobalVar.NodeInfoPanelIntroductionFinished;
            GlobalVar.instance.allowPlot = serializableGlobalVar.allowPlot;
            GlobalVar.instance.nodeName = serializableGlobalVar.nodeName;
            GlobalVar.instance.exposureCoefficient = serializableGlobalVar.exposureCoefficient;
            GlobalVar.instance.bookRandomConfig = new BookManager.BookRandomConfig(serializableGlobalVar.bookRandomConfig);
            GlobalVar.instance._remainedBookRandomConfig = new List<BookManager.BookRandomConfig>(serializableGlobalVar._remainedBookRandomConfig);
            
            List<SerializableNodeBehavior> serializableNodeBehaviors = deserializedData.nodeBehaviors;
            var nodeL = canvas.GetNodeList();
            int l = nodeL.Count;
            int minL = Mathf.Min(l, deserializedData.nodeBehaviors.Count);
            for (int i = 0; i < minL; i++)
            {
                var nb = nodeL[i].GetComponent<NodeBehavior>();
                nb.properties.state = serializableNodeBehaviors[i].state;
                // nb.properties.numOfBooks = serializableNodeBehaviors[i].numOfBooks;
                nb.properties.books = serializableNodeBehaviors[i].books;
                nb.hadAwakenedBefore = serializableNodeBehaviors[i].hadAwakenedBefore;
                if(serializableNodeBehaviors[i].isPaging) nb.plotAndPageHandler.OnLoadShowButtons();
            }
            
            SerializableDetectiveBehavior serializableDetectiveBehavior = deserializedData.detectiveBehavior;
            var detIntL = serializableDetectiveBehavior.focusOnNodes;
            detective.focusOnNodes.Clear();
            foreach (var focusPointer in detective.focusPointers)
            {
                Destroy(focusPointer);
            }

            detective.focusPointers.Clear();
            foreach (var i in detIntL)
            {
                detective.focusOnNodes.Add(nodeL[i]);
                detective.focusPointers.Add(Instantiate(detective.pointerPrefab, nodeL[i].transform.position,
                    Quaternion.Euler(0, 0, 0)));
            }

            List<SerializableConnectionBehavior> serializableConnectionBehaviors = deserializedData.connectionBehaviors;
            var conL = canvas.GetConnectionList();
            int cl = conL.Count;
            for (int i = 0; i < Mathf.Min(cl,serializableConnectionBehaviors.Count); i++)
            {
                var cb = conL[i].GetComponent<ConnectionBehavior>();
                cb.unlockTag = serializableConnectionBehaviors[i].unlockTag;
                if (cb is UnlockableConnectionBehavior ucb)
                {
                    ucb.available = serializableConnectionBehaviors[i].available;
                    ucb.unlockState = serializableConnectionBehaviors[i].unlockState;
                }
                if (serializableConnectionBehaviors[i].isDisplayingInfo)
                {
                    cb.InfoColor(detective);
                }
                else
                {
                    cb.NonInfoColor();
                }
                
            }

            

            SerializableQuests serializableQuests = deserializedData.quests;
            if(serializableQuests.q1) QuestPanel.instance.AddQuest("Zero");
            if(serializableQuests.q2) QuestPanel.instance.AddQuest("Office");
            if(serializableQuests.q3) QuestPanel.instance.AddQuest("PoliceStation");
            if(serializableQuests.q4) QuestPanel.instance.AddQuest("FireHouse");
            if(serializableQuests.q5) QuestPanel.instance.AddQuest("Deal");
            if(serializableQuests.q6) QuestPanel.instance.AddQuest("Final");

            SerializableBookPages serializableBookPages = deserializedData.bookPages;
            book.bookPages = new Sprite[serializableBookPages.PageIds.Count];
            for (int i = 0; i < book.bookPages.Length; i++)
            {
                book.bookPages[i] = SpriteList[serializableBookPages.PageIds[i]];
            }
        }
    }



    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        
        
        canvas = RoundManager.instance.canvas;
        detective = RoundManager.instance.detective;
        if (GameLoader.instance == null)
        {
            DeserializeAll("Assets/Saves/save7.json");
        }
        if (GameLoader.instance != null && GameLoader.instance.loadingAnExistingGame)
        {
            DeserializeAll(GameLoader.instance.loadFilePath);
        }
    }
}
