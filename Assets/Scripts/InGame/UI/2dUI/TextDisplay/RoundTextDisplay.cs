using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundTextDisplay : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<TextMeshProUGUI>().text = $"第{RoundManager.instance.roundNum}天";
    }
}
