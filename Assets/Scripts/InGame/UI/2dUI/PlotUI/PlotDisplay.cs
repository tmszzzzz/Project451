using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlotDisplay : MonoBehaviour
{
    // 引用 TextMeshProUGUI 组件
    [SerializeField] private TextMeshProUGUI plotText;

    // 引用继续按钮和选择项按钮的父容器
    [SerializeField] private GameObject continueButton;
    [SerializeField] private Transform selectionContainer;
    [SerializeField] private Button selectionButtonPrefab;

    private void Awake()
    {
        continueButton.GetComponent<Button>().onClick.AddListener(ContinuePlot);
        plotText.gameObject.SetActive(false);
    }


    // 开始新剧情时调用
    public void PushedStart()
    {
        plotText.gameObject.SetActive(true);
        plotText.text = ""; // 清空之前的剧情文本
        PlotManager.Instance.TriggerPlotStart();
    }

    // 结束剧情时调用
    public void PushedEnd()
    {

        plotText.text += "\n\n---END---"; // 显示剧情结束标记
        continueButton.SetActive(false); // 隐藏继续按钮
        selectionContainer.gameObject.SetActive(false); // 隐藏选择项容器
        PlotManager.Instance.TriggerPlotEnd();
    }

    // 继续剧情的方法
    public void ContinuePlot()
    {
        if(!PlotManager.Instance.GetDuringPlot())
        {
            Debug.LogWarning("当前不在剧情中，点击此按钮无效。");
        }
        // 隐藏继续按钮，并向 PlotManager 发送继续剧情的信号
        continueButton.SetActive(false);
        PlotManager.Instance.TriggerUserAction(false, 0);
    }

    // 显示对侧对话
    public void ShowDialog(string name, string content)
    {
        plotText.text += $"{name}:\n{content}\n\n";
        continueButton.SetActive(true);
    }

    // 显示自身对话
    public void ShowSelfDialog(string name, string content)
    {
        plotText.text += $"{name} (Me):\n{content}\n\n";
        continueButton.SetActive(true);
    }

    // 显示旁白
    public void ShowNarration(string content)
    {
        plotText.text += $"旁白:\n{content}\n\n";
        continueButton.SetActive(true);
    }

    // 显示选择项
    public void ShowSelection(List<string> choices)
    {
        selectionContainer.gameObject.SetActive(true);

        // 清空之前的选择项
        foreach (Transform child in selectionContainer)
        {
            Destroy(child.gameObject);
        }

        // 创建新的选择项
        for (int i = 0; i < choices.Count; i++)
        {
            string choiceText = choices[i];
            int choiceIndex = i;

            Button choiceButton = Instantiate(selectionButtonPrefab, selectionContainer);
            choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = choiceText;
            choiceButton.onClick.AddListener(() => SelectChoice(choiceIndex));
        }
    }

    // 选择一个选项
    private void SelectChoice(int choiceIndex)
    {
        selectionContainer.gameObject.SetActive(false); // 隐藏选择项
        PlotManager.Instance.TriggerUserAction(true, choiceIndex); // 通知 PlotManager 选择了某项
    }

}
