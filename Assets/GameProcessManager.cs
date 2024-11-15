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

        //设置成false稍后再打开
        probabilityOfInfoPanel.SetActive(false);
    }

    public void NewRoundIsEntered()
    {

    }

    void FirstReachFireHouse()
    {
        GlobalVar.instance.numOfBibliophileGiveBooks = 4;
    }

    void FirstReachPoliceStation()
    {
        GlobalVar.instance.numOfBibliophileGiveBooks = 2;
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
        GlobalVar.instance.probabilityOfNodesInspectingDetective = initialProbabilityOfInfo;
        GlobalVar.instance.everLearnedAboutDetectiveAndInfo = true;
    }

    public void NodeAwakend(GameObject thisnode)
    {
        int id = int.Parse(thisnode.name.Substring(5));
        if (!GlobalVar.instance.nodesAwakendOnce.Contains(id))
        {
            GlobalVar.instance.nodesAwakendOnce.Add(id);
        }
        else return;

        if (!GlobalVar.instance.everReachedFirehouse && thisnode.GetComponent<NodeBehavior>().properties.region == 2)
        {
            GlobalVar.instance.everReachedFirehouse = true;
            FirstReachFireHouse();
        }

        if (!GlobalVar.instance.everReachedPoliceStation && thisnode.GetComponent<NodeBehavior>().properties.region == 1)
        {
            GlobalVar.instance.everReachedPoliceStation = true;
            FirstReachPoliceStation();
        }

        if (GlobalVar.instance.nodesAwakendOnce.Count >= 12  && !GlobalVar.instance.everLearnedAboutDetectiveAndInfo)
        {
            PresentDetectiveAndInofSystem();
        }
        

        if (!GlobalVar.instance.everLearnedAboutKeepNodesDontFall && thisnode.GetComponent<NodeBehavior>().properties.fallThreshold != 0)
        {
            GlobalVar.instance.everLearnedAboutKeepNodesDontFall = true;
        }

        if (!GlobalVar.instance.everAwakeAllNodes && GlobalVar.instance.nodesAwakendOnce.Count == canvasBehavior.GetNodeList().Count)
        {
            GlobalVar.instance.everAwakeAllNodes = true;
        }
    }
}
