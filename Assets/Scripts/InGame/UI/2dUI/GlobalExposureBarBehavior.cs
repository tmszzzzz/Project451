using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalExposureBarBehavior : MonoBehaviour
{
    public Slider globalExposureBar;
    public Slider easeGlobalExposureBar;
    public GlobalVar globalVar;
    [SerializeField] private Image easeImage; 
    private float targetValue;
    [SerializeField] private float lerpSpeed = 0.001f;

    // Start is called before the first frame update
    void Start()
    {
        globalExposureBar.maxValue = GlobalVar.instance.maxGlobalExposureValue;
        easeGlobalExposureBar.maxValue = GlobalVar.instance.maxGlobalExposureValue;
    }

    void HandleGlobalExposureBar() 
    {
        targetValue = globalVar.globalExposureValue;
        // globalExposureBar.value = newValue;
    }

    void LerpingGlobalExposureBar()
    {
        if (globalExposureBar.value > targetValue)
        {
            globalExposureBar.value = targetValue;
        }
        else if (globalExposureBar.value < targetValue)
        {
            globalExposureBar.value = Mathf.Min(globalExposureBar.value + lerpSpeed, targetValue);
            easeImage.color = new Color(1,0.85f,0.63f,1);
        }

        if (easeGlobalExposureBar.value < targetValue)
        {
            easeGlobalExposureBar.value = targetValue;
        }
        else if (easeGlobalExposureBar.value > targetValue)
        {
            easeGlobalExposureBar.value = Mathf.Max(easeGlobalExposureBar.value - lerpSpeed, targetValue);
            easeImage.color = new Color(0.63f,1,0.68f,1);
        }
    }

    void IncrementGlobalExposureBar() 
    {
       // globalExposureBar.value = newValue;
    }

    void DecreaseGloabExposureBar()
    {
        //globalExposureBar.value = newValue;
    }   
    void HandleGlobalEaseExposureBarChange()
    {
        if (globalExposureBar.value == easeGlobalExposureBar.value) 
            return;
        easeGlobalExposureBar.value = Mathf.Lerp(easeGlobalExposureBar.value, globalVar.globalExposureValue, lerpSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        HandleGlobalExposureBar();
        LerpingGlobalExposureBar();
        //HandleGlobalEaseExposureBarChange();
    }
}
