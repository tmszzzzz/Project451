using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class GlobalVar : MonoBehaviour
{
    public static GlobalVar instance;
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

    public int maxGlobalExposureValue = 100;
    public int exposureValueAdditionOfExposedNode = 10;
    public int exposureValueAdditionOfDetective = 2;
    public int exposureValueAccelerationOfDetective = 1;
    public int exposureValueReductionOfNoExposedNode = 5;
    public int allocationLimit = 1;
    public int numOfBibliophileGiveBooks = 1;
    public int numOfFirefighterGiveBooks = 1;
    public int numOfMaximumBookDeliverRange = 2;
    public int numOfDetectiveOnStart = 4;
    public float probabilityOfNodesInspectingDetective = 0.1f;
    
    public Dictionary<string, float> increasementOfSkillsPerTime = new Dictionary<string, float>();

    public int resourcePoint = 0;
    public void AddResourcePoint(int value)
    {
        resourcePoint += value;
    }
    
    public int resourcePointPerInfoIncrement = 3;
    public float infoIncreaseBy = 0.01f;
    [SerializeField] private int infoResourcePoint = 0;
    public void InfoResourcePointIncrement(int v)
    {
        if (resourcePoint <= 0)
        {
            Debug.Log("no resource point");
            return;
        }

        resourcePoint--;
        infoResourcePoint += v;
        while (infoResourcePoint >= resourcePointPerInfoIncrement)
        {
            infoResourcePoint -= resourcePointPerInfoIncrement;
            probabilityOfNodesInspectingDetective+=infoIncreaseBy;
        }
    }
    
    public int resourcePointPerDistanceIncrement = 3;
    public int distanceIncreaseBy = 1;
    [SerializeField] private int distanceResourcePoint = 0;
    public void DistanceResourcePointIncrement(int v)
    {
        if (resourcePoint <= 0)
        {
            Debug.Log("no resource point");
            return;
        }

        resourcePoint--;
        distanceResourcePoint += v;
        while (distanceResourcePoint >= resourcePointPerDistanceIncrement)
        {
            distanceResourcePoint -= resourcePointPerDistanceIncrement;
            numOfMaximumBookDeliverRange+=distanceIncreaseBy;
        }
    }
    public int resourcePointPerAllocationLimitIncrement = 3;
    public int allocationLimitIncreaseBy = 1;
    [SerializeField] private int allocationLimitResourcePoint = 0;

    public void AllocationLimitResourcePointIncrement(int v)
    {
        if (resourcePoint <= 0)
        {
            Debug.Log("no resource point");
            return;
        }

        resourcePoint--;
        allocationLimitResourcePoint += v;
        while (allocationLimitResourcePoint >= resourcePointPerAllocationLimitIncrement)
        {
            allocationLimitResourcePoint -= resourcePointPerAllocationLimitIncrement;
            allocationLimit += allocationLimitIncreaseBy;
        }
    }

    public int exposureValuePerResource = 30;

    public void DecreaseExposureValueByResource()
    {
        if (resourcePoint <= 0)
        {
            Debug.Log("no resource point");
            return;
        }

        resourcePoint--;
        globalExposureValue = Math.Max(globalExposureValue - exposureValuePerResource, 0);
    }

    //采用单例模式，任意代码段可通过类名的静态变量Instance引用此唯一实例。
    private void Awake()
    {
        // 如果已有实例且不是当前实例，销毁当前实例，确保单例唯一性
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 将当前实例设为单例实例
        instance = this;

        // 选择保留这个对象，使其在场景切换时不会被销毁
        DontDestroyOnLoad(gameObject);
    }



    private void KeepLimitEqualToHalfOfBookNum()
    {
        allocationLimit = RoundManager.instance.canvas.GetTotalBookNum();
    }



    void Update()
    {
    }
}
