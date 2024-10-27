using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GlobalVar : MonoBehaviour
{
    public static GlobalVar Instance { get; set; }
    //此处存放各类全局需要用到的常量或变量，视游戏进程可以做一定的修改。
    //请注意为了简便，此处的值由成员变量初始化得到，这将导致在此脚本内修改这些参数无效，需要在inspector内修改。
    public int globalExposureValue = 0;

    public void AddGlobalExposureValue(int v)
    {
        globalExposureValue = Math.Min(globalExposureValue + v, maxGlobalExposureValue);
    }
    public void RuduceGlobalExposureValue(int v)
    {
        globalExposureValue = Math.Max(globalExposureValue - v, 0);
    }

    public const int maxGlobalExposureValue = 100;
    public int exposureValueAdditionOfExposedNode = 10;
    public int exposureValueAdditionOfDetective = 2;
    public int exposureValueAccelerationOfDetective = 1;
    public int exposureValueReductionOfNoExposedNode = 5;
    public int allocationLimit = 1;
    public int NumOfBibliophileGiveBooks = 1;
    public int NumOfFirefighterGiveBooks = 1;
    public int NumOfMaximumBookDeliverRange = 3;

    //采用单例模式，任意代码段可通过类名的静态变量Instance引用此唯一实例。
    private void Awake()
    {
        // 如果已有实例且不是当前实例，销毁当前实例，确保单例唯一性
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 将当前实例设为单例实例
        Instance = this;

        // 选择保留这个对象，使其在场景切换时不会被销毁
        DontDestroyOnLoad(gameObject);
    }

    private void KeepLimitEqualToHalfOfBookNum()
    {
        allocationLimit = RoundManager.Instance.canvas.GetTotalBookNum();
    }

    void Update()
    {
        KeepLimitEqualToHalfOfBookNum();
    }
}
