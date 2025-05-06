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
    private int currentTask;
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
        this.gameObject.SetActive(true);
        HideAllTasks();
        currentTask = index;
        if (index >= 0 && index < steps.Length)
        {
            if (steps[index].name == "消防员")
            {
                CameraBehavior.instance.FixedCamera2();
            }
            else if(steps[index].name == "任务面板")
            {
                CameraBehavior.instance.FixedCamera1();
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
        if (steps[this.currentTask].target != null)
        {
            this.lastData = getLastData(steps[this.currentTask].target);
        }
        this.currentTask++;
        ExecuteTask(this.currentTask);
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
        CameraBehavior.instance.FixedCamera1();
        ExecuteTask(0);
        StartCoroutine(WaitForFirstSelectBookMark());
        
        // 快速测试
    }
    
    private IEnumerator WaitForFirstSelectBookMark()
    {
        while (!GlobalVar.instance.firstSelectBookMark)
        {
            yield return null; // 等待一帧
        }
        NextTask();
        StartCoroutine(WaitForFirstAllocation());
    }
    private IEnumerator WaitForFirstAllocation()
    {
        while (!GlobalVar.instance.firstAllocation)
        {
            yield return null; // 等待一帧
        }
        NextTask();
        StartCoroutine(WaitForFirstNext());
    }
    
    private IEnumerator WaitForFirstNext()
    {
        while (!GlobalVar.instance.firstNext)
        {
            yield return null; // 等待一帧
        }
        CameraBehavior.instance.isCameraFixed = false;
        NextTask();
        Invoke("OpenInfoPanel",3);
    }
    
    private void OpenInfoPanel()
    {
        GlobalVar.instance.openInfoPanel = true;
        NextTask();
        StartCoroutine(WaitForCloseInfoPanel());
    }
    
    private IEnumerator WaitForCloseInfoPanel()
    {
        while (!GlobalVar.instance.closeInfoPanel)
        {
            yield return null; // 等待一帧
        }
        Debug.Log("CloseInfoPanel");
        CameraBehavior.instance.FixedCamera1();
        NextTask();
        StartCoroutine(WaitForSecondAllocationSuccess());
    }
    
    private IEnumerator WaitForSecondAllocationSuccess()
    {
        while (GlobalVar.instance.allocationSuccess < 2)
        {
            yield return null; // 等待一帧
        }
        NextTask();
        StartCoroutine(WaitForFirstCancellAllocation());
    }
    
    private IEnumerator WaitForFirstCancellAllocation()
    {
        while (!GlobalVar.instance.firstCancellAllocation)
        {
            yield return null; // 等待一帧
        }
        NextTask();
        StartCoroutine(WaitForFirstGetResourcePoint());
    }
    
    private IEnumerator WaitForFirstGetResourcePoint()
    {
        while (!GlobalVar.instance.firstGetResourcePoint)
        {
            yield return null; // 等待一帧
        }
        NextTask();
        StartCoroutine(WaitForFirstOpenPointUsage());
    }
    
    private IEnumerator WaitForFirstOpenPointUsage()
    {
        while (!GlobalVar.instance.firstOpenPointUsage)
        {
            yield return null; // 等待一帧
        }
        NextTask();
        StartCoroutine(WaitForFirstUseResourcePoint());
    }
    
    private IEnumerator WaitForFirstUseResourcePoint()
    {
        while (!GlobalVar.instance.firstUseResourcePoint)
        {
            yield return null; // 等待一帧
        }
        // TODO
        // 轮询暴露值和看火者
        // NextTask();
    }
    
    private void SetButtonOn()
    {
        this.transform.GetChild(this.transform.childCount - 1).gameObject.SetActive(true);
    }

    private void Finish()
    {
        this.gameObject.SetActive(false);
        completed = true;
    }
}
