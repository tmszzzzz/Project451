using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AllocationDisplay : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<TextMeshProUGUI>().text = $"分配情况: {RoundManager.Instance.GetNeedToAllocate()}/{GlobalVar.Instance.allocationLimit}";
    }
}
