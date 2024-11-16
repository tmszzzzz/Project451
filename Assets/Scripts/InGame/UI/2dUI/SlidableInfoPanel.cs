using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;
public class SlidableInfoPanel : MonoBehaviour
{
    public List<GameObject> panelUnits;
    public Slider slider;
    private List<float> _panelUnitActivatedSliderValues;

    void FillPanelUnitActivatedSliderValues()
    {
        float fullWidth = transform.GetComponent<RectTransform>().rect.width;
        _panelUnitActivatedSliderValues = new List<float>();
        foreach (GameObject panel in panelUnits)
        {
            Transform panelTransform = panel.GetComponent<Transform>();
            float x = panelTransform.GetComponent<RectTransform>().localPosition.x;
            float sliderValue = (x + fullWidth / 2) / fullWidth;
            _panelUnitActivatedSliderValues.Add(sliderValue);
        }

        //Debug.Log(_panelUnitActivatedSliderValues);
    }
    
    
    void Start()
    {
        FillPanelUnitActivatedSliderValues();
    }

    void LerpPanelUnitActivatedSliderValues(float targetValue, int i)
    {
        Transform unitTransform = panelUnits[i].GetComponent<Transform>();
        Vector3 targetScale = new Vector3(targetValue, targetValue, targetValue);
        Vector3 tempV = Vector3.Lerp(unitTransform.localScale, targetScale, Time.deltaTime * 20f);
        unitTransform.localScale = tempV;
    }

    private void LerpSliderValueToTargetValue(float targetValue)
    {
        slider.value = Mathf.Lerp(slider.value, targetValue, Time.deltaTime * 5f);
    }

    private bool _isExpanding = true;
    public void Expand()
    {
        //LerpSliderValueToTargetValue(1);
        _isExpanding = true;
    }

    public void Shrink()
    {
        //LerpSliderValueToTargetValue(0);
        _isExpanding = false;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject pUnit in panelUnits)
        {
            if (slider.value > _panelUnitActivatedSliderValues[panelUnits.IndexOf(pUnit)])
            {
                LerpPanelUnitActivatedSliderValues(1, panelUnits.IndexOf(pUnit));
            }
            else
            {
               LerpPanelUnitActivatedSliderValues(0, panelUnits.IndexOf(pUnit));
            }
        }

        if (_isExpanding)
        {
            LerpSliderValueToTargetValue(1);
        }
        else
        {
            LerpSliderValueToTargetValue(0);
        }
    }
}