using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ExposedInfo : MonoBehaviour
{
    void Update()
    {
        gameObject.GetComponent<TextMeshProUGUI>().text = $"{GlobalVar.instance.globalExposureValue} / 100";
    }
}
