using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class esc : MonoBehaviour
{
    [SerializeField] private GameObject QuitPanel;
    public Animator animator;        // 动画器

    public Animator bg;

    public Animator loading;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)&&!RoundManager.instance.operationForbidden)
        {
            AppearAndWait();
        }
    }
    
    private void AppearAndWait()
    {
        QuitPanel.SetActive(true);
        RoundManager.instance.OperationForbidden();
        animator.Play("SaveAndQuitAppear");
    }

    public void DisappearAndWait()
    {
        StartCoroutine(DisappearAndWaitCoroutine());
    }
    
    private IEnumerator DisappearAndWaitCoroutine()
    {
        // 播放动画
        animator.Play("SaveAndQuitDisap");

        // 获取当前动画状态信息
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // 等待动画片段长度的时间
        yield return new WaitForSeconds(stateInfo.length);

        QuitPanel.SetActive(false);
        RoundManager.instance.OperationRelease();
    }

    public void Quit()
    {
        SavesLoadManager.instance.SerializeAll();
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
}
