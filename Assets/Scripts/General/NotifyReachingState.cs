using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotifyReachingState : StateMachineBehaviour
{
    // 可以在 Inspector 中设置目标状态名
    public string triggerNotificationMethod = ""; // 用于通知的方法名称

    // 当动画进入某个状态时，检查是否匹配目标状态
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 获取组件并调用指定的通知方法
        var controller = AnimatorStateReachManager.instance;
        if (controller != null)
        {
            var methodInfo = controller.GetType().GetMethod(triggerNotificationMethod);
            if (methodInfo != null)
            {
                methodInfo.Invoke(controller, null); // 调用通知方法
            }
        }

    }
}
