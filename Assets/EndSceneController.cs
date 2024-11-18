using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    public Animator loading;
    public Animator bg;
    public AudioClip win;
    public AudioClip fail;

    private void Awake()
    {
        Camera.main.GetComponent<AudioSource>().clip = GameLoader.instance.winGame ? win : fail;
    }

    private void Start()
    {
        Camera.main.GetComponent<AudioSource>().Play();
    }

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
            string pth = "Assets/Saves/save.json";
            if (File.Exists(pth))
            {
                File.Delete(pth);
            }
            ExitToMain();   
        }
    }

    public void ExitToMain()
    {
        StartCoroutine(Fade(1));
        StartCoroutine(LoadSceneCoroutine(0));
    }
    
    private IEnumerator LoadSceneCoroutine(int sceneName)
    {
        bg.gameObject.SetActive(true);
        
        bg.Play("begin");

        // 获取当前动画状态信息
        AnimatorStateInfo stateInfo = bg.GetCurrentAnimatorStateInfo(0);

        // 等待动画片段长度的时间
        yield return new WaitForSeconds(stateInfo.length);
        loading.gameObject.SetActive(true);
        
        
        // 启动异步加载操作
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName,LoadSceneMode.Single);

        // 禁用场景切换完成后自动激活（如果需要在完成后进行额外操作）
        asyncLoad.allowSceneActivation = false;

        // 在加载进行时，可以添加加载界面或进度条显示
        while (!asyncLoad.isDone)
        {
            // 获取加载进度（0-0.9 表示加载中，1 表示加载完成）
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            Debug.Log($"Loading progress: {progress * 100}%");

            // 检查加载是否即将完成
            if (asyncLoad.progress >= 0.9f)
            {
                // 自定义逻辑可以放在此处，例如等待玩家点击继续
                // 等待用户操作，完成加载
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        Debug.Log("Scene loaded!");
    }
    
    private IEnumerator Fade(float fadeDuration)
    {
        AudioSource aus = Camera.main.GetComponent<AudioSource>();
        if (aus.isPlaying)
        {
            float startVolume = aus.volume;
            while (aus.volume > 0)
            {
                aus.volume -= startVolume * Time.deltaTime / fadeDuration;
                yield return null;
            }
            aus.Stop();
            aus.volume = startVolume; // 重置音量
        }
    }
}
