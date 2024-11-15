using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
    public static GameLoader instance;
    public Animator loading;
    public Animator bg;
    

    public bool loadingAnExistingGame = false;
    public string loadFilePath = "Assets/Saves/save.json";
    
    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void newGame()
    {
        loadingAnExistingGame = false;
        LoadSceneAsync(1);
    }

    public void openAGame()
    {
        loadingAnExistingGame = true;
        LoadSceneAsync(1);
    }
    
    public void LoadSceneAsync(int scene)
    {
        StartCoroutine(LoadSceneCoroutine(scene));
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
}
