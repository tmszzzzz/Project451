using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidePanel : MonoBehaviour
{
    GuideController guideController;
    Canvas canvas;

    private void Awake()
    {
        canvas = transform.GetComponentInParent<Canvas>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // guideController = transform.parent.GetComponent<GuideController>();  //实际到canvas上
        guideController = transform.GetComponent<GuideController>();        // 测试用
        RectTransform target = GameObject.Find("调试面板").GetComponent<RectTransform>();
        guideController.Guide(canvas, target, getLastData(target), GuideType.Circle);    // 输入要寻找的目标
        Invoke("Test", 2);
    }

    void Test()
    {
        RectTransform target = GameObject.Find("调试面板").GetComponent<RectTransform>();
        guideController.Guide(canvas, GameObject.Find("消息栏").GetComponent<RectTransform>(), getLastData(target), GuideType.Circle,TranslateType.Slow);
    }


    public LastData getLastData(RectTransform target)
    {
        LastData lastData = new LastData();
        lastData.rectHeight = target.rect.height / 2;
        lastData.rectWidth = target.rect.width / 2;
        lastData.circleRadius =  Mathf.Sqrt(lastData.rectHeight * lastData.rectHeight + lastData.rectWidth * lastData.rectWidth);
        return lastData;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
