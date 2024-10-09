using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageBar : MonoBehaviour
{
    public TextMeshProUGUI messageText; // ������ʾ��Ϣ
    private Queue<string> messageQueue = new Queue<string>(); // ��Ϣ����

    private void Start()
    {
        UpdateMessageDisplay(); // ��ʼ����
    }

    private void Update()
    {
        // ������Ϣ��ʾ��������в�Ϊ�գ���ʾ������Ϣ
        if (messageQueue.Count > 0)
        {
            UpdateMessageDisplay();
        }
    }

    public void AddMessage(string message)
    {
        messageQueue.Enqueue(message); // �����Ϣ������
        StartCoroutine(RemoveMessageAfterDelay(5f)); // ����Э�̣�5����Ƴ���Ϣ
    }

    private IEnumerator RemoveMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (messageQueue.Count > 0)
        {
            messageQueue.Dequeue(); // �Ƴ������еĵ�һ����Ϣ
            UpdateMessageDisplay(); // ������ʾ
        }
    }

    private void UpdateMessageDisplay()
    {
        // ��ʾ�����е�������Ϣ
        messageText.text = string.Join("\n", messageQueue.ToArray());
    }
}
