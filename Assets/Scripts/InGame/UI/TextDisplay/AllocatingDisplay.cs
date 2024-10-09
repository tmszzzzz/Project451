using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AllocatingDisplay : MonoBehaviour
{
    void Update()
    {
        gameObject.GetComponent<TextMeshProUGUI>().text = $"Allocating: {RoundManager.Instance.held}/{RoundManager.Instance.GetNeedToAllocate()}";
    }
}
