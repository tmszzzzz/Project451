using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorStateReachManager : MonoBehaviour
{
    public static AnimatorStateReachManager instance;
    
    [SerializeField] private DetectiveBehavior detectiveBehavior;
    private void Awake()
    {
        // 如果已有实例且不是当前实例，销毁当前实例，确保单例唯一性
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 将当前实例设为单例实例
        instance = this;

    }

    public void NotifyDetective1()
    {
        if(detectiveBehavior.Tcs1 != null && !detectiveBehavior.Tcs1.Task.IsCompleted) detectiveBehavior.Tcs1?.SetResult(true);
    }
    public void NotifyDetective2()
    {
        if(detectiveBehavior.Tcs1 != null && !detectiveBehavior.Tcs2.Task.IsCompleted) detectiveBehavior.Tcs2?.SetResult(true);
    }
}
