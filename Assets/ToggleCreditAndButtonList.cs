using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.UI;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ToggleCreditAndButtonList : MonoBehaviour
{
    public GameObject creditText;
    public GameObject buttonList;
    public TextMeshProUGUI toggleText;
    // Start is called before the first frame update
    void Start()
    {
        creditText.SetActive(false);
        toggleText.text = "制作组名单/致谢";
    }

    public void ToggleCreditAndList()
    {
        if(creditText.activeSelf)
        {
            creditText.SetActive(false);
            buttonList.SetActive(true);
            toggleText.text = "制作组名单/致谢";
        }
        else
        {
            creditText.SetActive(true);
            buttonList.SetActive(false);
            toggleText.text = "返回主界面";
        }
    }
}
