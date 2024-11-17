using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndSceneController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI contentText;

    [SerializeField] private string winText = "你赢了";
    [SerializeField] private string loseText = "你输了";
    
    private void Update()
    {
        if (GameLoader.instance.winGame)
        {
            titleText.text = winText;
        }
        else
        {
            titleText.text = loseText;
        }
        
        contentText.text = "获得的书籍：" + GameLoader.instance.gainBooks + "本\n" +
            "加入你们的人：" + GameLoader.instance.transformedNodes + "人\n" +
            "加入你们的藏书家：" + GameLoader.instance.transformedBib + "人\n" +
            "加入你们的消防员：" + GameLoader.instance.transformedFire + "人\n" +
            "加入你们的关键人物：" + GameLoader.instance.transformedKey + "人\n" 
            + "用了" + GameLoader.instance.usedDays + "天"
            
            + "\n\n" +
            "按ESC键返回主菜单";

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitToMain();   
        }
    }

    public void ExitToMain()
    {
        SceneManager.LoadScene(0);
    }
}
