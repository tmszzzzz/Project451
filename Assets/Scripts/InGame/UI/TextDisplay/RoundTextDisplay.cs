using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundTextDisplay : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<TextMeshProUGUI>().text = $"ROUND {RoundManager.Instance.roundNum}";
    }
}