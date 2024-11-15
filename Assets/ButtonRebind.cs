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
    public GameObject loading;
    public GameObject bg;
    void Start()
    {
        b1.onClick.AddListener(GameLoader.instance.newGame);
        b2.onClick.AddListener(GameLoader.instance.openAGame);
        if (!System.IO.File.Exists(GameLoader.instance.loadFilePath))
        {
            b2.enabled = false;
            b2text.text = "暂无存档";
        }

        GameLoader.instance.loading = loading.GetComponent<Animator>();
        GameLoader.instance.bg = bg.GetComponent<Animator>();
    }

    public void Exit()
    {
        Application.Quit();
    }
}
