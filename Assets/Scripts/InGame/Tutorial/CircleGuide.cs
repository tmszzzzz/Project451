using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleGuide : GuideBase
{
    private float radius;       // 镂空区域的半径
    private float scaleR;       // 变化之后的半径
    private float startRadius;
    public override void Guide(Canvas canvas, RectTransform target, LastData lastData, RenderType renderType = RenderType.Screen, TranslateType translateType = TranslateType.Direct, float moveTime = 1)
    {
        base.Guide(canvas, target, lastData, renderType, translateType, moveTime);
        
        // 计算半径
        float width = (targetCorners[3].x - targetCorners[0].x) / 2;
        float height = (targetCorners[1].y - targetCorners[0].y) / 2;
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

    public override void Guide(Canvas canvas, RectTransform target, LastData lastData, float scale, float scaleTime, RenderType renderType = RenderType.Screen, TranslateType translateType = TranslateType.Direct, float moveTime = 1)
    {
        this.Guide(canvas, target, lastData, renderType, translateType, moveTime);
        
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
