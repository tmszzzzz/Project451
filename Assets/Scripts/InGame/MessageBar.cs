using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageBar : MonoBehaviour
{
    public TextMeshProUGUI messageText; // 用于显示消息
    private Queue<string> messageQueue = new Queue<string>(); // 消息队列

    private void Start()
    {
        UpdateMessageDisplay(); // 初始更新
    }

    private void Update()
    {
        // 更新消息显示，如果队列不为空，显示最新消息
        if (messageQueue.Count > 0)
        {
            UpdateMessageDisplay();
        }
    }

    public void AddMessage(string message)
    {
        messageQueue.Enqueue(message); // 添加消息到队列
        StartCoroutine(RemoveMessageAfterDelay(5f)); // 启动协程，5秒后移除消息
    }

    private IEnumerator RemoveMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (messageQueue.Count > 0)
        {
            messageQueue.Dequeue(); // 移除队列中的第一个消息
            UpdateMessageDisplay(); // 更新显示
        }
    }

    private void UpdateMessageDisplay()
    {
        // 显示队列中的所有消息
        messageText.text = string.Join("\n", messageQueue.ToArray());
    }
}
