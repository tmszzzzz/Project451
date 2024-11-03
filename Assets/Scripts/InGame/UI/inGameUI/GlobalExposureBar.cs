using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalExposureBarBehavior : MonoBehaviour
{
    public Slider globalExposureBar;
    public Slider easeGlobalExposureBar;
    public GlobalVar globalVar;
    private float newValue;
    [SerializeField] private float lerpSpeed = 0.005f;

    // Start is called before the first frame update
    void Start()
    {
        globalExposureBar.maxValue = GlobalVar.Instance.maxGlobalExposureValue;
        easeGlobalExposureBar.maxValue = GlobalVar.Instance.maxGlobalExposureValue;

        globalExposureBar.value = globalVar.globalExposureValue;
        easeGlobalExposureBar.value = globalVar.globalExposureValue;
    }

    void HandleGlobalExposureBar() 
    {
        newValue = globalVar.globalExposureValue;
        globalExposureBar.value = newValue;
    }
    void IncrementGlobalExposureBar() 
    {
        globalExposureBar.value = newValue;
    }

    void DecreaseGloabExposureBar()
    {
        globalExposureBar.value = newValue;
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
        HandleGlobalEaseExposureBarChange();
    }
}
