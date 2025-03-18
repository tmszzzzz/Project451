using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AllocationRangeText : MonoBehaviour
{
    private GlobalVar globalVar;

    void Start()
    {
        globalVar = GlobalVar.instance;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<TextMeshProUGUI>().text = $"×î´ó´«²¥·¶Î§£º{globalVar.numOfMaximumBookDeliverRange}";
        //Debug.Log(globalVar.numOfMaximumBookDeliverRange);
    }
}
