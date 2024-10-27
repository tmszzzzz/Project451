using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AllocationRangeText : MonoBehaviour
{
    private GlobalVar globalVar;

    void Start()
    {
        globalVar = GlobalVar.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<TextMeshProUGUI>().text = $"{globalVar.NumOfMaximumBookDeliverRange}";
    }
}
