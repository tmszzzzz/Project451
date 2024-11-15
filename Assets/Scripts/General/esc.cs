using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class esc : MonoBehaviour
{
    [SerializeField] private GameObject QuitPanel;
    public Animator animator;        // 动画器
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
}
