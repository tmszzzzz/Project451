using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameProcessManager : MonoBehaviour
{
    public static GameProcessManager instance;  // 单例实例

    public List<GameObject> nodesAwakendOnce = new List<GameObject>();
    private RoundManager roundManager;
    private GlobalVar globalVar;
    public CanvasBehavior canvasBehavior;
    public DetectiveBehavior detectiveBehavior;
    [SerializeField] float initialProbabilityOfInfo = 0.4f;
    [SerializeField] GameObject probabilityOfInfoPanel;
    private bool everReachedPoliceStation = false;
    private bool everReachedFirehouse = false;
    private bool everLearnedAboutDetectiveAndInfo = false;
    private bool everLearnedAboutKeepNodesDontFall = false;
    private bool everAwakeAllNodes = false;
    // Start is called before the first frame update

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

    void PresentDetectiveAndInofSystem()
    {
        RoundManager.instance.skipCameraOverview = false;   // 重新开启摄像机视角
        probabilityOfInfoPanel.SetActive(true);
        detectiveBehavior.AddDetectivesInRegion(0, 4);
        detectiveBehavior.AddDetectivesInRegion(1, 7);
        detectiveBehavior.AddDetectivesInRegion(2, 11);
        globalVar.probabilityOfNodesInspectingDetective = initialProbabilityOfInfo;
        everLearnedAboutDetectiveAndInfo = true;
    }

    public void NodeAwakend(GameObject thisnode)
    {
        if (!nodesAwakendOnce.Contains(thisnode))
        {
            nodesAwakendOnce.Add(thisnode);
        }
        else return;

        if (!everReachedFirehouse && thisnode.GetComponent<NodeBehavior>().properties.region == 2)
        {
            everReachedFirehouse = true;
            FirstReachFireHouse();
        }

        if (!everReachedPoliceStation && thisnode.GetComponent<NodeBehavior>().properties.region == 1)
        {
            everReachedPoliceStation = true;
            FirstReachPoliceStation();
        }

        if (nodesAwakendOnce.Count >= 12  && !everLearnedAboutDetectiveAndInfo)
        {
            PresentDetectiveAndInofSystem();
        }
        

        if (!everLearnedAboutKeepNodesDontFall && thisnode.GetComponent<NodeBehavior>().properties.fallThreshold != 0)
        {
            everLearnedAboutKeepNodesDontFall = true;
        }

        if (!everAwakeAllNodes && nodesAwakendOnce.Count == canvasBehavior.GetNodeList().Count)
        {
            everAwakeAllNodes = true;
        }
    }
}
