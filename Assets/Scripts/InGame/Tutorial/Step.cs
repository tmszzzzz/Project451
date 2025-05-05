using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Step : MonoBehaviour
{
    public string eventName;
    public RectTransform target;
    public GuideType guideType = GuideType.Rect;
    public float scale = 1;
    public float scaleTime = 0;
    public RenderType renderType = RenderType.Screen;
    public TranslateType translateType = TranslateType.Direct;
    public float transTime = 1;
    LastData lastData;
    
    public RectTransform targetPos;
    public void Execute(GuideController guideController, Canvas canvas, LastData lastData)
    {
        this.gameObject.SetActive(true);
        this.lastData = lastData;
        guideController.Guide(canvas, target, this.lastData, this.guideType, this.scale, this.scaleTime, this.renderType, this.translateType, this.transTime);
        if (targetPos != null)
        {
            targetPos.localPosition = guideController.Center;
            Invoke("SetInfoOn", 1);
        }
    }

    public void SetInfoOn()
    {
        targetPos.gameObject.SetActive(true);
    }
}
