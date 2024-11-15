using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonRebind : MonoBehaviour
{
    public Button b1;
    public Button b2;
    public TextMeshProUGUI b2text;
    void Start()
    {
        b1.onClick.AddListener(GameLoader.instance.newGame);
        b2.onClick.AddListener(GameLoader.instance.openAGame);
        if (!System.IO.File.Exists(GameLoader.instance.loadFilePath))
        {
            b2.enabled = false;
            b2text.text = "暂无存档";
        }
    }
}
