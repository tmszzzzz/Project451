using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayoutComponent : MonoBehaviour
{
    // 解决点开二级列表会卡住的问题
    private RectTransform rectTransform;
    private GameObject secondMenu;
    private Toggle firstMenuToggle;

    private void Awake()
    {
        firstMenuToggle = GetComponent<Toggle>();
        rectTransform = GetComponent<RectTransform>();
        secondMenu = rectTransform.GetChild(1).gameObject;
    }

    private void Start()
    {
        firstMenuToggle.onValueChanged.AddListener(canOpen => OpenSecondMenu(canOpen));
    }

    public void OpenSecondMenu(bool canOpen)
    {
        secondMenu.gameObject.SetActive(canOpen);
        StartCoroutine(UpdateLayout(rectTransform));
    }
    
    IEnumerator UpdateLayout(RectTransform rect)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        yield return new WaitForEndOfFrame();
    }
}
