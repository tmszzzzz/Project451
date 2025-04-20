using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RectGuide : GuideBase
{
    protected float width;        // 镂空区域的宽
    protected float height;       // 镂空区域的高

    private float scaleWidth;
    private float scaleHeight;
    private float startWidth;
    private float startHeight;
    public override void Guide(Canvas canvas, RectTransform target, LastData lastData, TranslateType translateType = TranslateType.Direct, float moveTime = 1)
    {
        base.Guide(canvas, target, lastData, translateType, moveTime);
        
        // 计算宽和高
        //width = targetCorners[3].x - targetCorners[0].x;
        //height = targetCorners[1].y - targetCorners[0].y;
        width = target.rect.width / 2;
        height = target.rect.height / 2;
        material.SetFloat("_SliderX", width);
        material.SetFloat("_SliderY", height);
        switch (translateType)
        {
            case TranslateType.Slow:
                startWidth = lastData.rectWidth;
                startHeight = lastData.rectHeight;
                break;
            case TranslateType.Direct:
                break;
        }
    }

    public override void Guide(Canvas canvas, RectTransform target, LastData lastData, float scale, float scaleTime, TranslateType translateType = TranslateType.Direct, float moveTime = 1)
    {
        this.Guide(canvas, target, lastData, translateType, moveTime);
        
        scaleWidth = scale * width;
        scaleHeight = scale * height;
        isScaling = true;
        scaleTimer = 0;
        this.scaleTime = scaleTime;
    }

    protected override void Update()
    {
        base.Update();
        if (isScaling)
        {
            material.SetFloat("_SliderX", Mathf.Lerp(scaleWidth, width, scaleTimer));
            material.SetFloat("_SliderY", Mathf.Lerp(scaleHeight, height, scaleTimer));
        }
        if (isMoving)
        {
            material.SetFloat("_SliderX", Mathf.Lerp(startWidth, width, centerTimer));
            material.SetFloat("_SliderY", Mathf.Lerp(startHeight, height, centerTimer));
        }
    }
}
