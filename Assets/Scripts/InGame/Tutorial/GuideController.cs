using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum GuideType
{
    Rect,
    Circle
}

[RequireComponent(typeof(CircleGuide))]
[RequireComponent(typeof(RectGuide))]

public class GuideController : MonoBehaviour, ICanvasRaycastFilter
{
    
    private CircleGuide circleGuide;
    private RectGuide rectGuide;
    
    public Material rectMat;
    public Material circleMat;
    
    private Image mask;

    private RectTransform target;
    private RenderType renderType;
    private GuideType guideType;
    public Vector3 Center
    {
        get
        {
            switch (this.guideType)
            {
               case GuideType.Rect:
                   return rectGuide.Center;
               case GuideType.Circle:
                   return circleGuide.Center;
            }
            return rectGuide.Center;
        }
    }
    private void Awake()
    {
        mask = transform.GetComponent<Image>();
        if (mask == null)
        {
            throw new System.Exception("Mask 初始化失败！");
        }

        if (rectMat == null || circleMat == null)
        {
            throw new System.Exception("材质未赋值！");
        }
        
        circleGuide = transform.GetComponent<CircleGuide>();
        rectGuide = transform.GetComponent<RectGuide>();
    }

    private void Guide(RectTransform target, GuideType guideType, RenderType renderType)
    {
        this.target = target;
        this.guideType = guideType;
        this.renderType = renderType;
        switch (guideType)
        {
            case GuideType.Rect:
                mask.material = rectMat;
                break;
            case GuideType.Circle:
                mask.material = circleMat;
                break;
        }
    }
    
    // 大小不变化
    public void Guide(Canvas canvas, RectTransform target, LastData lastData, GuideType guideType, RenderType renderType = RenderType.Screen, TranslateType translateType = TranslateType.Direct, float moveTime = 1)
    {
        Guide(target, guideType, renderType);
        switch (guideType)
        {
            case GuideType.Rect:
                rectGuide.Guide(canvas, target, lastData, renderType, translateType, moveTime);
                break;
            case GuideType.Circle:
                circleGuide.Guide(canvas, target, lastData, renderType, translateType, moveTime);
                break;
        }
    }

    // 大小变化
    public void Guide(Canvas canvas, RectTransform target, LastData lastData, GuideType guideType, float scale, float scaleTime, RenderType renderType = RenderType.Screen, TranslateType translateType = TranslateType.Direct, float moveTime = 1)
    {
        Guide(target, guideType, renderType);
        switch (guideType)
        {
            case GuideType.Rect:
                rectGuide.Guide(canvas, target, lastData, scale, scaleTime, renderType, translateType, moveTime);
                break;
            case GuideType.Circle:
                circleGuide.Guide(canvas, target, lastData, scale, scaleTime, renderType, translateType, moveTime);
                break;
        }
    }
    
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        switch (renderType)
        {
            case RenderType.Screen:
                if (target == null)
                {
                    this.transform.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
                    return false;    // 事件不会渗透
                }
                this.transform.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.88f);
                return !RectTransformUtility.RectangleContainsScreenPoint(target, sp);
            case RenderType.World:
                return !RectTransformUtility.RectangleContainsScreenPoint(target, sp, Camera.main);
        }
        if (target == null)
        {
            return true;    // 事件不会渗透
        }
        return !RectTransformUtility.RectangleContainsScreenPoint(target, sp);
    }
}
