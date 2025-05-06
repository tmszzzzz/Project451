using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainInfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _roundText;
    [SerializeField] private TextMeshProUGUI _exposureRiskText;
    [SerializeField] private TextMeshProUGUI _allocationRangeText;
    [SerializeField] private TextMeshProUGUI _allocationNumText;
    [SerializeField] private TextMeshProUGUI _proberbilityOfInfo;

    public void RoungTextEnter()
    {
        _roundText.gameObject.SetActive(true);
    }

    public void RoungTextExit()
    {
        _roundText.gameObject.SetActive(false);
    }

    public void ExposureRiskTextEnter()
    {
        _exposureRiskText.gameObject.SetActive(true);
    }

    public void ExposureRiskTextExit()
    {
        _exposureRiskText.gameObject.SetActive(false);
    }

    public void AllocationRangeTextEnter()
    {
        _allocationRangeText.gameObject.SetActive(true);
    }

    public void AllocationRangeTextExit()
    {
        _allocationRangeText.gameObject.SetActive(false);
    }

    public void AllocationNumTextEnter()
    {
        _allocationNumText.gameObject.SetActive(true);
    }

    public void AllocationNumTextExit()
    {
        _allocationNumText.gameObject.SetActive(false);
    }

    public void ProberbilityOfInfoEnter()
    {
        _proberbilityOfInfo.gameObject.SetActive(true);
    }

    public void ProberbilityOfInfoExit()
    {
        _proberbilityOfInfo.gameObject.SetActive(false);
    }
}
