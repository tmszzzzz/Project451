using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 中心点移动的类型
public enum TranslateType
{
    Direct,
    Slow
}

public enum RenderType
{
    World,
    Screen
}
public struct LastData
{
    public float circleRadius;
    public float rectWidth;
    public float rectHeight;
};

[RequireComponent(typeof(Image))]

public class GuideBase : MonoBehaviour
{
    protected Material material;  // 材质
    protected Vector3 center;     // 中心

    protected RectTransform target;   // 要显示的目标

    protected Vector3[] targetCorners = new Vector3[4];   // 要引导的目标的边界
    
    protected float scaleTimer;
    protected float scaleTime;
    protected bool isScaling;

    private Vector3 startCenter;
    protected float centerTimer;
    protected float centerTime;
    protected bool isMoving;
    
    protected LastData lastData;

    public Vector3 Center
    {
        get
        {
            return center;
        }
    }
    
    public virtual void Guide(Canvas canvas, RectTransform target, LastData lastData, RenderType renderType = RenderType.Screen, TranslateType translateType = TranslateType.Direct, float time = 1.0f)
    {
        // 初始化材质
        material = transform.GetComponent<Image>().material;
        this.lastData = lastData;
        this.target = target;
        // 获取中心点
        target.GetWorldCorners(targetCorners);
        
        
        for (int i = 0; i < targetCorners.Length; i++)
        {
            switch (renderType)
            {
                case RenderType.Screen:
                    targetCorners[i]  = WorldToScreenPointScreenMode(canvas, targetCorners[i]);
                    break;
                case RenderType.World:
                    targetCorners[i]  = WorldToScreenPointWorldMode(canvas, targetCorners[i]);
                    break;
            }
        }
        // 计算中心点
        center.x = targetCorners[0].x + (targetCorners[3].x - targetCorners[0].x) / 2;
        center.y = targetCorners[0].y + (targetCorners[1].y - targetCorners[0].y) / 2;
        
        switch (translateType)
        {
            case TranslateType.Slow:
                startCenter = material.GetVector("_Center");
                isMoving = true;
                centerTimer = 0;
                centerTime = time;
                break;
            case TranslateType.Direct:
                material.SetVector("_Center", center);
                break;
        }
    }

    public virtual void Guide(Canvas canvas, RectTransform target,LastData lastData, float scale, float scaleTime, RenderType renderType = RenderType.Screen, TranslateType translateType = TranslateType.Direct, float moveTime = 1.0f) { }
    
    public Vector2 WorldToScreenPointScreenMode(Canvas canvas, Vector3 worldPoint)
    {
        // 世界坐标转屏幕坐标
        Vector2 screenPoint  = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, worldPoint);
        Vector2 localPoint;
        // 屏幕坐标转局部坐标
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPoint, canvas.worldCamera, out localPoint);
        return localPoint;
    }
    
    public Vector2 WorldToScreenPointWorldMode(Canvas canvas, Vector3 worldPoint)
    {
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPoint);
        Vector2 localPoint;
        // 屏幕坐标转局部坐标
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPoint, canvas.worldCamera, out localPoint);
        return localPoint;
    }
    
    protected virtual void Update()
    {
        if (isScaling)
        {
            scaleTimer += Time.deltaTime * 1 / scaleTime;
            if (scaleTimer >= 1)
            {
                isScaling = false;
                scaleTimer = 0;
            }
        }

        if (isMoving)
        {
            centerTimer += Time.deltaTime * 1 / centerTime;
            // 设置中心点
            material.SetVector("_Center", Vector3.Lerp(startCenter, center, centerTimer));
            if (centerTimer >= 1)
            {
                isMoving = false;
                centerTimer = 0;
            }
        }
    }
    
}
