using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UI;
using UnityEngine.UI;

public class ResourceBar : MonoBehaviour
{
    private int record = 0;
    [SerializeField] private GameObject _1ui;
    [SerializeField] private GameObject _2ui;

    private void Start()
    {
        
    }

    void Update()
    {
        if (record != GlobalVar.instance.resourcePoint)
        {
        }
    }
}
