using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AllocatingDisplay : MonoBehaviour
{
    void Update()
    {
        gameObject.GetComponent<TextMeshProUGUI>().text = $"ÿ�غ����Ĵ�����Ŀ��{RoundManager.instance.GetNeedToAllocate()}";
        // {RoundManager.instance.held}
    }
}
