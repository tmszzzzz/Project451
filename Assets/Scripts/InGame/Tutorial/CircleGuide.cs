using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleGuide : GuideBase
{
    private float radius;       // 镂空区域的半径
    private float scaleR;       // 变化之后的半径
    private float startRadius;
    public override void Guide(Canvas canvas, RectTransform target, LastData lastData, TranslateType translateType = TranslateType.Direct, float moveTime = 1)
    {
        base.Guide(canvas, target, base.lastData, translateType, moveTime);
        
        // 计算半径
        float width = target.rect.width / 2;
        float height = target.rect.height / 2;
        radius = Mathf.Sqrt(width * width + height * height);
        material.SetFloat("_Slider", radius);
        
        switch (translateType)
        {
            case TranslateType.Slow:
                startRadius = lastData.circleRadius;
                break;
            case TranslateType.Direct:
                break;
        }
    }

    public override void Guide(Canvas canvas, RectTransform target, LastData lastData, float scale, float scaleTime, TranslateType translateType = TranslateType.Direct, float moveTime = 1)
    {
        this.Guide(canvas, target, lastData, translateType, moveTime);
        
        scaleR = radius * scale;
        this.material.SetFloat("_Slider", scaleR);
        this.scaleTime = scaleTime;
        isScaling = true;
        scaleTimer = 0;
    }

    protected override void Update()
    {
        base.Update();
        if (isScaling)
        {
            this.material.SetFloat("_Slider", Mathf.Lerp(scaleR, radius, scaleTimer));
        }
        if (isMoving)
        {
            material.SetFloat("_Slider", Mathf.Lerp(startRadius, radius, centerTimer));
        }
    }
}
