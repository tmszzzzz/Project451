using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AllocationDisplay : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<TextMeshProUGUI>().text = $"∑÷≈‰…œœﬁ£∫{RoundManager.instance.BookAllocationNum()}/{GlobalVar.instance.allocationLimit}";
    }
}
