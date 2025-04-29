using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlobalExposureBarBehavior : MonoBehaviour
{
    public Slider globalExposureBar;
    public Slider easeGlobalExposureBar;
    public Slider previewGlobalExposureBar;
    public GlobalVar globalVar;
    [SerializeField] private Image easeImage; 
    private float targetValue;
    [SerializeField] private float lerpSpeed = 0.001f;
    private float previewValue;
    // Start is called before the first frame update
    void Start()
    {
        globalExposureBar.maxValue = GlobalVar.instance.maxGlobalExposureValue;
        easeGlobalExposureBar.maxValue = GlobalVar.instance.maxGlobalExposureValue;
        previewGlobalExposureBar.maxValue = GlobalVar.instance.maxGlobalExposureValue;
    }

    void HandleGlobalExposureBar() 
    {
        targetValue = globalVar.globalExposureValue;
        previewValue = globalVar.previewExposureValue;
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
        
        if (previewGlobalExposureBar.value < targetValue + previewValue)
        {
            previewGlobalExposureBar.value = Mathf.Min(previewGlobalExposureBar.value + lerpSpeed, targetValue + previewValue);
        }
        else if (previewGlobalExposureBar.value > targetValue + previewValue)
        {
            previewGlobalExposureBar.value = Mathf.Max(previewGlobalExposureBar.value - lerpSpeed, targetValue + previewValue);
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

    public void GlobalExposureBarEnter()
    {
        transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(true);
    }
    
    public void GlobalExposureBarExit()
    {
        transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(false);
    }

    public void PreviewGlobalExposureBarChange()
    {
        previewGlobalExposureBar.gameObject.SetActive(!previewGlobalExposureBar.gameObject.activeSelf);
    }
}
