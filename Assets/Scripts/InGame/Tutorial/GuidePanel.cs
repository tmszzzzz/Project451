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
            steps[index].Execute(guideController, canvas, this.lastData);
            Invoke("SetButtonOn",2);
        }
        else
        {
            Finish();
        }
    }

    public void NextTask()
    {
        this.lastData = getLastData(steps[this.currentTask].target);
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
        CameraBehavior.instance.TestCamera();
        ExecuteTask(0);
    }

    public void SetButtonOn()
    {
        this.transform.GetChild(this.transform.childCount - 1).gameObject.SetActive(true);
    }

    public void Finish()
    {
        this.gameObject.SetActive(false);
    }
}
