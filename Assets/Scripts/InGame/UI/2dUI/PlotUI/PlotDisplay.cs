using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class PlotDisplay : MonoBehaviour
{
    [SerializeField] private GameObject plotPanel;
    [SerializeField] private GameObject plotDisplayArea;
    [SerializeField] private GameObject plotSelectionArea;
    [SerializeField] private Image shadowImage;
    [SerializeField] private float shadowAlpha = 0.95f; 
    [SerializeField] private List<Image> imagesTobeFaded;
    private bool isPlotting = false;
    [SerializeField] private float lerpAlphaSpeed = 10f;
    void Update()
    {
        if(shadowImage.color.a < 0.1f && isPlotting == false)
        {
            shadowImage.gameObject.SetActive(false);
            plotPanel.SetActive(false);
        }

        float targetShadowAlpha = isPlotting ? shadowAlpha : 0f;
        shadowImage.color = new Color(shadowImage.color.r, shadowImage.color.g, shadowImage.color.b, Mathf.Lerp(shadowImage.color.a, targetShadowAlpha, Time.deltaTime * lerpAlphaSpeed));

        float targetAlpha = isPlotting ? 1f : 0f;
        foreach (Image image in imagesTobeFaded)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(image.color.a, targetAlpha, Time.deltaTime * lerpAlphaSpeed));
        }
    }

    void OpenPlots()
    {
        if (isPlotting)
        {
            Debug.Log("Plot again");
        }
        shadowImage.gameObject.SetActive(true);
        plotPanel.SetActive(true);
        isPlotting = true;
    }


    void ClosePlots()
    {
        if (!isPlotting)
        {
            Debug.Log("Plot is not open already");
        }
        isPlotting = false;
        plotDisplayArea.GetComponent<PlotDisplayArea>().ClosePlots();
        plotSelectionArea.GetComponent<PlotSelectionArea>().ClosePlots(); 
    }

    // 开始新剧情时调用
    public void PushedStart()
    {
        OpenPlots();
        PlotManager.Instance.TriggerPlotStart();
        CreateContinueBUtton();
    }



    // 结束剧情时调用
    public void PushedEnd()
    {
        ClosePlots();
        PlotManager.Instance.TriggerPlotEnd();
    }

    // 继续剧情的方法
    public void ContinuePlot()
    {
        if(!PlotManager.Instance.GetDuringPlot())
        {
            Debug.LogWarning("当前不在剧情中，点击此按钮无效。");
        }

        PlotManager.Instance.TriggerUserAction(false, 0);
    }

    void CreateContinueBUtton()
    {
        Debug.Log("calling from selection area");
        Button continueButton = plotSelectionArea.GetComponent<PlotSelectionArea>().NeedButtons(1)[0];
        //Debug.Log(continueButton.gameObject.activeSelf);
        continueButton.onClick.AddListener(ContinuePlot);
    }

    // 显示对侧对话
    public void ShowDialog(string name, string content)
    {
        plotDisplayArea.GetComponent<PlotDisplayArea>().PlotNewText(false, name, content);
        CreateContinueBUtton();
    }

    // 显示自身对话
    public void ShowSelfDialog(string name, string content)
    {
        plotDisplayArea.GetComponent<PlotDisplayArea>().PlotNewText(true, name, content);
        CreateContinueBUtton();
    }

    // 显示旁白
    public void ShowNarration(string content)
    {
        Debug.Log("Already abandon this function");
        // plotText.text += $"旁白:\n{content}\n\n";
        // continueButton.SetActive(true);
    }

    // 显示选择项
    public void ShowSelection(List<string> choices)
    {
        // selectionContainer.gameObject.SetActive(true);

        // // 清空之前的选择项
        // foreach (Transform child in selectionContainer)
        // {
        //     Destroy(child.gameObject);
        // }

        // // 创建新的选择项
        // for (int i = 0; i < choices.Count; i++)
        // {
        //     string choiceText = choices[i];
        //     int choiceIndex = i;

        //     Button choiceButton = Instantiate(selectionButtonPrefab, selectionContainer);
        //     choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = choiceText;
        //     choiceButton.onClick.AddListener(() => SelectChoice(choiceIndex));
        // }
    }

    // 选择一个选项
    private void SelectChoice(int choiceIndex)
    {
        // selectionContainer.gameObject.SetActive(false); // 隐藏选择项
        // PlotManager.Instance.TriggerUserAction(true, choiceIndex); // 通知 PlotManager 选择了某项
    }

}
