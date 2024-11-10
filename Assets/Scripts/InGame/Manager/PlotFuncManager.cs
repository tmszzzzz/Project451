using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotFuncManager : MonoBehaviour
{
    public static PlotFuncManager instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void test()
    {
        Debug.Log("Function test called.");
    }
}
