using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AllocatingDisplay : MonoBehaviour
{
    void Update()
    {
        gameObject.GetComponent<TextMeshProUGUI>().text = $"每回合最大的传递数目：{RoundManager.instance.GetNeedToAllocate()}";
        // {RoundManager.instance.held}
    }
}
