using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalExposureBarBehavior : MonoBehaviour
{
    public Slider globalExposureBar;
    public Slider easeGlobalExposureBar;
    public GlobalVar globalVar;
    private float targetValue;
    [SerializeField] private float lerpSpeed = 0.5f;

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
        if (globalExposureBar.value == targetValue) 
            return;
        globalExposureBar.value = Mathf.Lerp(globalExposureBar.value, targetValue, lerpSpeed);
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
