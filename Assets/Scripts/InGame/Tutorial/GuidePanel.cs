using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
        steps = new Step[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            steps[i] = transform.GetChild(i).GetComponent<Step>();
        }

        if (steps.Length > 0)
        {
            lastData = getLastData(steps[0].target);
        }
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
        }
    }

    public void NextTask(string eventName)
    {
        if (eventName == steps[this.currentTask].eventName)
        {
            this.lastData = getLastData(steps[this.currentTask].target);
            this.currentTask++;
            ExecuteTask(this.currentTask);
        }
        
    }
    
    // 隐藏所有任务
    private void HideAllTasks()
    {
        for (int i = 0; i < steps.Length; i++)
        {
            steps[i].gameObject.SetActive(false);
        }
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
        ExecuteTask(0);
        Invoke("Test",3);
    }

    void Test()
    {
        NextTask(steps[currentTask].eventName);
    }

    
    // Update is called once per frame
    void Update()
    {
        
    }
}
