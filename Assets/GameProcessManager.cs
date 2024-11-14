using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameProcessManager : MonoBehaviour
{
    public static GameProcessManager instance;  // 单例实例

    //public List<int> nodesAwakendOnce = new List<int>();
    private RoundManager roundManager;
    private GlobalVar globalVar;
    public CanvasBehavior canvasBehavior;
    public DetectiveBehavior detectiveBehavior;
    [SerializeField] float initialProbabilityOfInfo = 0.4f;
    [SerializeField] GameObject probabilityOfInfoPanel;
    //private bool everReachedPoliceStation = false;
    //private bool everReachedFirehouse = false;
    //private bool everLearnedAboutDetectiveAndInfo = false;
    //private bool everLearnedAboutKeepNodesDontFall = false;
    //private bool everAwakeAllNodes = false;

    [SerializeField] private TutorialsController _tutorialsController;
    // Start is called before the first frame update
    

    private void Start()
    {
        if (!GlobalVar.instance.noStartingPlot)
        {
            PlotManager.instance.StartPlot("Assets/Resources/Plots/scene0.txt");
            GlobalVar.instance.noStartingPlot = true;
        }
    }

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

        roundManager = RoundManager.instance;
        globalVar = GlobalVar.instance;

        // 选择保留这个对象，使其在场景切换时不会被销毁
        DontDestroyOnLoad(gameObject);

        //设置成false稍后再打开
        probabilityOfInfoPanel.SetActive(false);
    }

    public void NewRoundIsEntered()
    {

    }

    void FirstReachFireHouse()
    {
        globalVar.numOfBibliophileGiveBooks = 4;
    }

    void FirstReachPoliceStation()
    {
        globalVar.numOfBibliophileGiveBooks = 2;
    }

    [SerializeField] private Sprite page1;
    [SerializeField] private Sprite page2;
    [SerializeField] private BookController _bookController;
    void PresentDetectiveAndInofSystem()
    {
        GlobalVar.instance.skipCameraOverview = false;   // 重新开启摄像机视角
        
        _tutorialsController.canShowTutorial4 = true;
        _bookController.subsititute(page1, 11);
        _bookController.subsititute(page2, 12);
        
        probabilityOfInfoPanel.SetActive(true);
        
        detectiveBehavior.AddDetectivesInRegion(0, 4);
        detectiveBehavior.AddDetectivesInRegion(1, 7);
        detectiveBehavior.AddDetectivesInRegion(2, 11);
        globalVar.probabilityOfNodesInspectingDetective = initialProbabilityOfInfo;
        globalVar.everLearnedAboutDetectiveAndInfo = true;
    }

    public void NodeAwakend(GameObject thisnode)
    {
        int id = int.Parse(thisnode.name.Substring(5));
        if (!globalVar.nodesAwakendOnce.Contains(id))
        {
            globalVar.nodesAwakendOnce.Add(id);
        }
        else return;

        if (!globalVar.everReachedFirehouse && thisnode.GetComponent<NodeBehavior>().properties.region == 2)
        {
            globalVar.everReachedFirehouse = true;
            FirstReachFireHouse();
        }

        if (!globalVar.everReachedPoliceStation && thisnode.GetComponent<NodeBehavior>().properties.region == 1)
        {
            globalVar.everReachedPoliceStation = true;
            FirstReachPoliceStation();
        }

        if (globalVar.nodesAwakendOnce.Count >= 12  && !globalVar.everLearnedAboutDetectiveAndInfo)
        {
            PresentDetectiveAndInofSystem();
        }
        

        if (!globalVar.everLearnedAboutKeepNodesDontFall && thisnode.GetComponent<NodeBehavior>().properties.fallThreshold != 0)
        {
            globalVar.everLearnedAboutKeepNodesDontFall = true;
        }

        if (!globalVar.everAwakeAllNodes && globalVar.nodesAwakendOnce.Count == canvasBehavior.GetNodeList().Count)
        {
            globalVar.everAwakeAllNodes = true;
        }
    }
}
