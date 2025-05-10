using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExposureRiskText : MonoBehaviour
{
    private GlobalVar globalVar;

    void Start()
    {
        globalVar = GlobalVar.instance;
    }

    // Update is called once per frame
    void Update()
    {
        switch (globalVar.exposureCoefficient)
        {
            case 0.8f:
                gameObject.GetComponent<TextMeshProUGUI>().text = $"低";
                break;
            case 1f:
                gameObject.GetComponent<TextMeshProUGUI>().text = $"中";
                break;
            case 1.2f:
                gameObject.GetComponent<TextMeshProUGUI>().text = $"高";
                break;
            default:
                gameObject.GetComponent<TextMeshProUGUI>().text = $"风险值出现错误";
                break;
        }
        //Debug.Log(globalVar.numOfMaximumBookDeliverRange);
    }
}
