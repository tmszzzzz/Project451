using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBar : MonoBehaviour
{
    private int record = 0;
    [SerializeField] private GameObject _1ui;
    [SerializeField] private GameObject _2ui;
    [SerializeField] private float maxHeight = 166.7621f; //资源槽高度
    [SerializeField] private float width = 11.7f; //资源点宽度
    [SerializeField] private float diamondHeight = 4f; //资源1点显示为菱形时高度为diamondHeight * 2
    [SerializeField] private float sideHeight; //侧边高度（根据n实时计算）；资源1点的实际高度是sideHeight + diamondHeight * 2；资源单点的实际高度是sideHeight + diamondHeight
    [SerializeField] private float visuallySpacing = 2f; //视觉上的点数间隔
    [SerializeField] private List<GameObject> points;
    //资源点上限n，满足等式：maxHeight = n * sideHeight + (n - 1) * visuallySpacing + diamondHeight * 2;

    private void Start()
    {
        if (GlobalVar.instance.maxResourcePoint == 0) GlobalVar.instance.maxResourcePoint = 10;
        var n = GlobalVar.instance.maxResourcePoint;
        sideHeight = (maxHeight - (n - 1) * visuallySpacing - 2 * diamondHeight) / n;
        var _1height = sideHeight + diamondHeight * 2;
        var _2height = sideHeight + diamondHeight;
        
        points.Add(Instantiate(_1ui,transform));
        var rect1 = points[0].GetComponent<RectTransform>();
        rect1.sizeDelta = new Vector2(width,_1height);
        points[0].SetActive(false);
        
        for (int i = 1; i < n; i++)
        {
            points.Add(Instantiate(_2ui,transform));
            var rect2 = points[i].GetComponent<RectTransform>();
            rect2.sizeDelta = new Vector2(width,_2height);
            rect2.anchoredPosition = new Vector2(0, diamondHeight + i * (visuallySpacing + sideHeight));
            points[i].SetActive(false);
        }
    }

    void Update()
    {
        if (record == GlobalVar.instance.resourcePoint) return;
        if (record > GlobalVar.instance.resourcePoint)
        {
            for (int i = record - 1; i >= GlobalVar.instance.resourcePoint; i--)
            {
                if(i>=0&&i<GlobalVar.instance.maxResourcePoint)points[i].SetActive(false);
            }
        }
        else
        {
            for (int i = record; i < GlobalVar.instance.resourcePoint; i++)
            {
                if(i>=0&&i<GlobalVar.instance.maxResourcePoint)points[i].SetActive(true);
            }
        }

        record = GlobalVar.instance.resourcePoint;
    }
}
