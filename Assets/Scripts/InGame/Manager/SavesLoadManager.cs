using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SavesLoadManager : MonoBehaviour
{
    public static SavesLoadManager instance;
    private CanvasBehavior canvas;
    private DetectiveBehavior detective;
    [SerializeField] private Book book;
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
        public float probabilityOfNodesInspectingDetective;
        public int resourcePoint;
        public int resourcePointPerInfoIncrement;
        public float infoIncreaseBy;
        public int resourcePointPerDistanceIncrement;
        public int distanceIncreaseBy;
        public int resourcePointPerAllocationLimitIncrement;
        public int allocationLimitIncreaseBy;
        public int exposureValuePerResource;
        public bool everReachedPoliceStation;
        public bool everReachedFirehouse;
        public bool everLearnedAboutDetectiveAndInfo;
        public bool everLearnedAboutKeepNodesDontFall;
        public bool everAwakeAllNodes;
        public bool noStartingPlot;
        public bool everReachingMaxExposureValue;
        public List<int> nodesAwakendOnce;
        public bool skipCameraOverview;
        public int dealStartRound;

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
            this.resourcePointPerInfoIncrement = globalVar.resourcePointPerInfoIncrement;
            this.infoIncreaseBy = globalVar.infoIncreaseBy;
            this.resourcePointPerDistanceIncrement = globalVar.resourcePointPerDistanceIncrement;
            this.distanceIncreaseBy = globalVar.distanceIncreaseBy;
            this.resourcePointPerAllocationLimitIncrement = globalVar.resourcePointPerAllocationLimitIncrement;
            this.allocationLimitIncreaseBy = globalVar.allocationLimitIncreaseBy;
            this.exposureValuePerResource = globalVar.exposureValuePerResource;
            this.everReachedPoliceStation = globalVar.everReachedPoliceStation;
            this.everReachedFirehouse = globalVar.everReachedFirehouse;
            this.everLearnedAboutDetectiveAndInfo = globalVar.everLearnedAboutDetectiveAndInfo;
            this.everLearnedAboutKeepNodesDontFall = globalVar.everLearnedAboutKeepNodesDontFall;
            this.everAwakeAllNodes = globalVar.everAwakeAllNodes;
            this.noStartingPlot = globalVar.noStartingPlot;
            this.everReachingMaxExposureValue = globalVar.everReachingMaxExposureValue;
            this.nodesAwakendOnce = new List<int>(globalVar.nodesAwakendOnce);
            this.skipCameraOverview = globalVar.skipCameraOverview;
            this.dealStartRound = globalVar.dealStartRound;
        }
    }


    [System.Serializable]
    public class SerializableNodeBehavior
    {
        public Properties.StateEnum state;
        public int numOfBooks;
        public bool hadAwakenedBefore;
        public bool isPaging;

        public SerializableNodeBehavior(NodeBehavior nodeBehavior)
        {
            this.state = nodeBehavior.properties.state;
            this.numOfBooks = nodeBehavior.properties.numOfBooks;
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

        public SerializableQuests(QuestPanel questPanel)
        {
            q1 = false;
            q2 = false;
            q3 = false;
            q4 = false;
            q5 = false;
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
                }
            }
        }
    }

    [System.Serializable]
    public class SerializableBookPages
    {
        public List<int> PageIds;

        public SerializableBookPages(Book book,List<Sprite> sprites)
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

        public SerializableAll(GlobalVar globalVar, CanvasBehavior canvas, DetectiveBehavior detective,QuestPanel quest,Book book,List<Sprite> sprites)
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

    public void DeserializeAll(string filePath)
    {
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
            GlobalVar.instance.resourcePointPerInfoIncrement = serializableGlobalVar.resourcePointPerInfoIncrement;
            GlobalVar.instance.infoIncreaseBy = serializableGlobalVar.infoIncreaseBy;
            GlobalVar.instance.resourcePointPerDistanceIncrement =
                serializableGlobalVar.resourcePointPerDistanceIncrement;
            GlobalVar.instance.distanceIncreaseBy = serializableGlobalVar.distanceIncreaseBy;
            GlobalVar.instance.resourcePointPerAllocationLimitIncrement =
                serializableGlobalVar.resourcePointPerAllocationLimitIncrement;
            GlobalVar.instance.allocationLimitIncreaseBy = serializableGlobalVar.allocationLimitIncreaseBy;
            GlobalVar.instance.exposureValuePerResource = serializableGlobalVar.exposureValuePerResource;
            GlobalVar.instance.everReachedPoliceStation = serializableGlobalVar.everReachedPoliceStation;
            GlobalVar.instance.everReachedFirehouse = serializableGlobalVar.everReachedFirehouse;
            GlobalVar.instance.everLearnedAboutDetectiveAndInfo =
                serializableGlobalVar.everLearnedAboutDetectiveAndInfo;
            if (GlobalVar.instance.everLearnedAboutDetectiveAndInfo)
            {
                GameProcessManager.instance.probabilityOfInfoPanel.SetActive(true);
            }
            GlobalVar.instance.everLearnedAboutKeepNodesDontFall =
                serializableGlobalVar.everLearnedAboutKeepNodesDontFall;
            GlobalVar.instance.everAwakeAllNodes = serializableGlobalVar.everAwakeAllNodes;
            GlobalVar.instance.noStartingPlot = serializableGlobalVar.noStartingPlot;
            GlobalVar.instance.everReachingMaxExposureValue = serializableGlobalVar.everReachingMaxExposureValue;
            GlobalVar.instance.nodesAwakendOnce = new List<int>(serializableGlobalVar.nodesAwakendOnce);
            GlobalVar.instance.skipCameraOverview = serializableGlobalVar.skipCameraOverview;
            GlobalVar.instance.dealStartRound = serializableGlobalVar.dealStartRound;
            

            List<SerializableNodeBehavior> serializableNodeBehaviors = deserializedData.nodeBehaviors;
            var nodeL = canvas.GetNodeList();
            int l = nodeL.Count;
            for (int i = 0; i < l; i++)
            {
                var nb = nodeL[i].GetComponent<NodeBehavior>();
                nb.properties.state = serializableNodeBehaviors[i].state;
                nb.properties.numOfBooks = serializableNodeBehaviors[i].numOfBooks;
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
            for (int i = 0; i < cl; i++)
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
            string[] strs = new[] { "Zero", "Office","PoliceStation","FireHouse","Deal" };
            if(serializableQuests.q1) QuestPanel.instance.AddQuest(strs[0]);
            if(serializableQuests.q2) QuestPanel.instance.AddQuest(strs[1]);
            if(serializableQuests.q3) QuestPanel.instance.AddQuest(strs[2]);
            if(serializableQuests.q4) QuestPanel.instance.AddQuest(strs[3]);
            if(serializableQuests.q5) QuestPanel.instance.AddQuest(strs[4]);

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
            DeserializeAll("Assets/Saves/save.json");
        }
        if (GameLoader.instance != null && GameLoader.instance.loadingAnExistingGame)
        {
            DeserializeAll(GameLoader.instance.loadFilePath);
        }
    }
}
