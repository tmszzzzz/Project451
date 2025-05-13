using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Image = UnityEngine.UI.Image;

public class GameLoader : MonoBehaviour
{
    public static GameLoader instance;
    public Animator loading;
    public Animator bg;
    public GameObject player;
    public Image image;
    public bool loadingAnExistingGame = false;
    public string loadFilePath = "Assets/Saves/save8.json";
    // public string loadFilePath = "Assets/Saves/save.json";
    
    //Used For end scene
    public int usedDays = 0;
    public int transformedNodes = 0;
    public int transformedBib = 0;
    public int transformedFire = 0;
    public int transformedKey = 0;
    public bool winGame = false;
    public int gainBooks = 0;

    public void SaveDataForEndScene(bool win)
    {
        usedDays = GlobalVar.instance.roundNum;
        winGame = win;
        gainBooks = 0;
        transformedBib = 0;
        transformedFire = 0;
        transformedKey = 0;
        transformedNodes = 0;
        
        foreach(GameObject node in RoundManager.instance.canvas.GetNodeList())
        {
            NodeBehavior nodeBehavior = node.GetComponent<NodeBehavior>();

            gainBooks += nodeBehavior.properties.books.Count;
            
            if (nodeBehavior.properties.state >= Properties.StateEnum.AWAKENED)
            {
                transformedNodes++;
                if (nodeBehavior.properties.type == Properties.typeEnum.BIBLIOFHILE)
                {
                    transformedBib++;
                }
                else if (nodeBehavior.properties.type == Properties.typeEnum.FIREFIGHTER)
                {
                    transformedFire++;
                }
                else if (nodeBehavior.properties.type == Properties.typeEnum.KEYNODE)
                {
                    transformedKey++;
                }
            }
        }
    }
    
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

    public void newGame()
    {
        loadingAnExistingGame = false;
        StartCoroutine(Fade(1));
        StartCoroutine(LoadScene());
    }

    public void openAGame()
    {
        loadingAnExistingGame = true;
        StartCoroutine(Fade(1));
        LoadSceneAsync(1);
    }
    
    public void LoadSceneAsync(int scene)
    {
        StartCoroutine(LoadSceneCoroutine(scene));
    }
    
    public IEnumerator LoadScene()
    {
        bg.gameObject.SetActive(true);
        
        bg.Play("begin");

        // 获取当前动画状态信息
        AnimatorStateInfo stateInfo = bg.GetCurrentAnimatorStateInfo(0);

        // 等待动画片段长度的时间
        yield return new WaitForSeconds(stateInfo.length);
        loading.gameObject.SetActive(true);
        
        player.SetActive(true);
        while (!ImagePlayController.instance.finished)
        {
            yield return null;
        }
        image.gameObject.SetActive(true);
        StartCoroutine(LoadSceneCoroutine(1));
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
