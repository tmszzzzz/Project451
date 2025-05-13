using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GuidePanel : MonoBehaviour
{
    private GuideController guideController;
    private Canvas canvas;
    private Step[] steps;
    private LastData lastData;
    public bool completed = false;
    private void Awake()
    {
        canvas = transform.GetComponentInParent<Canvas>();
        // guideController = transform.parent.GetComponent<GuideController>();  //实际到canvas上
        guideController = transform.GetComponent<GuideController>();        // 测试用
        InitTasks();
    }

    private void InitTasks()
    {
        steps = new Step[transform.childCount - 1];
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            steps[i] = transform.GetChild(i).GetComponent<Step>();
        }

        if (steps.Length > 0)
        {
            lastData = getLastData(steps[0].target);
        }

        // var v = CanvasBehavior.instance.gameObject;
        // steps[0].target = CanvasBehavior.instance.GetNodeList().First().GetComponent<NodeBehavior>().GetBookMarkList()[0].GetComponent<Image>().GetComponent<RectTransform>();
    }
    
    // 执行某一个任务
    public void ExecuteTask(int index)
    {
        Debug.Log("现在的task");
        Debug.Log(GlobalVar.instance.currentTask);
        if (index == 0)
        {
            CameraBehavior.instance.FixedCamera1();
        }
        this.gameObject.SetActive(true);
        HideAllTasks();
        GlobalVar.instance.currentTask = index;
        if (index >= 0 && index < steps.Length)
        {
            if (steps[index].name == "消防员")
            {
                CameraBehavior.instance.FixedCamera2();
            }
            else if(steps[index].name == "任务面板")
            {
                CameraBehavior.instance.FixedCamera1();
            }else if (steps[index].name == "取消遮罩并解锁相机")
            {
                CameraBehavior.instance.isCameraFixed = false;
            }
            steps[index].Execute(guideController, canvas, this.lastData);
            if (steps[index].eventName != "取消按钮")
            {
                Invoke("SetButtonOn",1);
            }
        }
        else
        {
            Debug.Log("结束了.");
            Finish();
        }
    }

    public void NextTask()
    {
        if (steps[GlobalVar.instance.currentTask].target != null)
        {
            this.lastData = getLastData(steps[GlobalVar.instance.currentTask].target);
        }
        GlobalVar.instance.currentTask++;
        ExecuteTask(GlobalVar.instance.currentTask);
    }
    
    // 隐藏所有任务和点击按钮
    private void HideAllTasks()
    {
        for (int i = 0; i < steps.Length; i++)
        {
            steps[i].gameObject.SetActive(false);
        }
        this.transform.GetChild(this.transform.childCount - 1).gameObject.SetActive(false);
    }
    
    public LastData getLastData(RectTransform target)
    {
        LastData lastData = new LastData();
        lastData.rectHeight = target.rect.height / 2;
        lastData.rectWidth = target.rect.width / 2;
        lastData.circleRadius =  Mathf.Sqrt(lastData.rectHeight * lastData.rectHeight + lastData.rectWidth * lastData.rectWidth);
        return lastData;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        ExecuteTask(GlobalVar.instance.currentTask);
        StartCoroutine(WaitForFirstSelectBookMark());
    }
    
    private IEnumerator WaitForFirstSelectBookMark()
    {
        while (!GlobalVar.instance.firstSelectBookMark)
        {
            yield return null; // 等待一帧
        }
        if (GlobalVar.instance.currentTask == 3)
        {
            NextTask();
        }
        BookController.instance.addnodesTutorialPage();
        BookController.instance.addBookmarkTutorialPage();
        BookController.instance.TurnPageTo(1);
        StartCoroutine(WaitForFirstAllocation());
    }
    private IEnumerator WaitForFirstAllocation()
    {
        while (!GlobalVar.instance.firstAllocation)
        {
            yield return null; // 等待一帧
        }
        if (GlobalVar.instance.currentTask == 4)
        {
            NextTask();
        }
        StartCoroutine(WaitForFirstNext());
    }
    
    private IEnumerator WaitForFirstNext()
    {
        while (!GlobalVar.instance.firstNext)
        {
            yield return null; // 等待一帧
        }
        CameraBehavior.instance.isCameraFixed = false;
        if (GlobalVar.instance.currentTask == 5)
        {
            NextTask();     // 取消遮罩
        }
        Invoke("OpenInfoPanel",1);
    }
    
    private void OpenInfoPanel()
    {
        GlobalVar.instance.openInfoPanel = true;
        if (GlobalVar.instance.currentTask == 6)
        {
            NextTask(); //  关闭信息面板
        }
        StartCoroutine(WaitForCloseInfoPanel());
    }
    
    private IEnumerator WaitForCloseInfoPanel()
    {
        while (!GlobalVar.instance.closeInfoPanel)
        {
            yield return null; // 等待一帧
        }
        CameraBehavior.instance.FixedCamera1();
        if (GlobalVar.instance.currentTask == 8)
        {
            NextTask(); //  关键节点
        }
        BookController.instance.addNodeTypeTutorialPage();
        StartCoroutine(WaitForSecondAllocationSuccess());
    }
    
    private IEnumerator WaitForSecondAllocationSuccess()
    {
        while (GlobalVar.instance.allocationSuccess < 2)
        {
            yield return null; // 等待一帧
        }
        if (GlobalVar.instance.currentTask == 14)
        {
            NextTask(); // 取消分配
        }
        StartCoroutine(WaitForFirstCancellAllocation());
    }
    
    private IEnumerator WaitForFirstCancellAllocation()
    {
        while (!GlobalVar.instance.firstCancellAllocation)
        {
            yield return null; // 等待一帧
        }
        if (GlobalVar.instance.currentTask == 15)
        {
            NextTask(); // 取消遮罩
        }
        StartCoroutine(WaitForChapter1());
    }
    
    private IEnumerator WaitForChapter1()
    {
        while (!GlobalVar.instance.chapter1)
        {
            yield return null; // 等待一帧
        }
        CameraBehavior.instance.isCameraFixed = false;
        StartCoroutine(WaitForChapter1End());
    }
    
    private IEnumerator WaitForChapter1End()
    {
        while (RoundManager.instance.switchingPanel1.activeSelf)
        {
            yield return null; // 等待一帧
        }
        CameraBehavior.instance.FixedCamera1();
        if (GlobalVar.instance.currentTask == 16)
        {
            NextTask(); // 获取资源点
        }
        StartCoroutine(WaitForFirstGetResourcePoint());
    }
    
    private IEnumerator WaitForFirstGetResourcePoint()
    {
        while (!GlobalVar.instance.firstGetResourcePoint)
        {
            yield return null; // 等待一帧
        }
        if (GlobalVar.instance.currentTask == 17)
        {
            NextTask(); // 主信息面板
        }
        StartCoroutine(WaitForFirstOpenPointUsage());
    }
    
    private IEnumerator WaitForFirstOpenPointUsage()
    {
        while (!GlobalVar.instance.firstOpenPointUsage)
        {
            yield return null; // 等待一帧
        }
        if (GlobalVar.instance.currentTask == 19)
        {
            NextTask(); // 情报点使用
        }
        StartCoroutine(WaitForFirstUseResourcePoint());
    }
    
    private IEnumerator WaitForFirstUseResourcePoint()
    {
        while (!GlobalVar.instance.firstUseResourcePoint)
        {
            yield return null; // 等待一帧
        }
        if (GlobalVar.instance.currentTask == 20)
        {
            NextTask(); // 取消遮罩
        }
        CameraBehavior.instance.isCameraFixed = false;
        StartCoroutine(WaitForFirstPreviewExpose());
    }
    
    private IEnumerator WaitForFirstPreviewExpose()
    {
        while (!GlobalVar.instance.firstPreviewExpose)
        {
            yield return null; // 等待一帧
        }
        if (GlobalVar.instance.currentTask == 21)
        {
            NextTask(); // 暴露节点
        }
        BookController.instance.addExposureTutorialPage();
        CameraBehavior.instance.FixedCamera3();
        StartCoroutine(WaitForDetective());
        StartCoroutine(WaitForNodeInfoPanel());
    }
    
    private IEnumerator WaitForDetective()
    {
        while (!GlobalVar.instance.detective)
        {
            yield return null; // 等待一帧
        }
        if (GlobalVar.instance.currentTask == 24)
        {
            NextTask(); // 看火者介绍
        }
        BookController.instance.addFirewatcherTutorialPage();
        CameraBehavior.instance.FixedCamera4();
    }
    private IEnumerator WaitForNodeInfoPanel()
    {
        while (!GlobalVar.instance.everLearnedAboutNodeInfoPanel)
        {
            yield return null; // 等待一帧
        }
        Invoke("NodeInfoPanel",5);
    }

    private void NodeInfoPanel()
    {
        Debug.Log("NodeInfoPanel");
        CameraBehavior.instance.FixedCamera4();
        GlobalVar.instance.allowNodeInfoPanel = true;
        // 先加载一次防止加载错误
        PanelController.instance.EnableNodeInfoPanel(CanvasBehavior.instance.GetNodeByID(5).GetComponent<NodeBehavior>());
        PanelController.instance.NodeInfoPanel.SetActive(false);
        PanelController.instance.EnableNodeInfoPanel(CanvasBehavior.instance.GetNodeByName(GlobalVar.instance.nodeName).GetComponent<NodeBehavior>());
        BookController.instance.addIndexTutorialPage();
        
        Debug.Log(GlobalVar.instance.currentTask);
        while (GlobalVar.instance.currentTask < 28){}
        if (GlobalVar.instance.currentTask == 28)
        {
            NextTask(); // 节点信息面板介绍
        }
    }
    
    private void SetButtonOn()
    {
        this.transform.GetChild(this.transform.childCount - 1).gameObject.SetActive(true);
    }

    private void Finish()
    {
        this.gameObject.SetActive(false);
        completed = true;
        CameraBehavior.instance.isCameraFixed = false;
        GlobalVar.instance.allowPlot = true;
        GlobalVar.instance.NodeInfoPanelIntroductionFinished = true;
    }
}
