using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class PlotDisplay : MonoBehaviour
{
    private static PlotDisplay instance;
    
    public static PlotDisplay Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlotDisplay>();
            }

            return instance;
        }
    }
    
    [SerializeField] private GameObject plotPanel;
    [SerializeField] private GameObject plotDisplayArea;
    [SerializeField] private GameObject plotSelectionArea;
    [SerializeField] private Image shadowImage;
    [SerializeField] private float shadowAlpha = 0.95f; 
    [SerializeField] private List<Image> imagesTobeFaded;
    private bool _isPlotting = false;
    [SerializeField] private float lerpAlphaSpeed = 10f;
    void Update()
    {
        if(shadowImage.color.a < 0.1f && _isPlotting == false)
        {
            shadowImage.gameObject.SetActive(false);
            plotPanel.SetActive(false);
        }

        float targetShadowAlpha = _isPlotting ? shadowAlpha : 0f;
        shadowImage.color = new Color(shadowImage.color.r, shadowImage.color.g, shadowImage.color.b, Mathf.Lerp(shadowImage.color.a, targetShadowAlpha, Time.deltaTime * lerpAlphaSpeed));

        float targetAlpha = _isPlotting ? 1f : 0f;
        foreach (Image image in imagesTobeFaded)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(image.color.a, targetAlpha, Time.deltaTime * lerpAlphaSpeed));
        }
    }

    void OpenPlots()
    {
        if (_isPlotting)
        {
            Debug.Log("Plot again");
        }
        shadowImage.gameObject.SetActive(true);
        plotPanel.SetActive(true);
        _isPlotting = true;

        plotDisplayArea.GetComponent<PlotDisplayArea>().OpenPlots();
    }


    void ClosePlots()
    {
        if (!_isPlotting)
        {
            Debug.Log("Plot is not open already");
        }

        Debug.Log("Plot is not open already");
        _isPlotting = false;
        plotDisplayArea.GetComponent<PlotDisplayArea>().ClosePlots();
        plotSelectionArea.GetComponent<PlotSelectionArea>().ClosePlots(); 
    }

    // 开始新剧情时调用
    public void PushedStart()
    {
        timeForLastPlotToBeRead = 0f;

        OpenPlots();
        PlotManager.instance.TriggerPlotStart();
        //CreateContinueBUtton();
    }

    void CreateEndButton()
    {
        Button endButton = CreateContinueButton("End");
        endButton.onClick.AddListener(ClosePlots);
        endButton.onClick.AddListener(PlotManager.instance.TriggerPlotEnd);
    }
    
    // 结束剧情时调用
    public void PushedEnd()
    {
        plotSelectionArea.GetComponent<PlotSelectionArea>().ClearCurrentButtons();
        
        UnityEngine.Events.UnityEvent newEvents = new UnityEngine.Events.UnityEvent();
        newEvents.AddListener(CreateEndButton);
        StartCoroutine(InvokeAfterDelay(newEvents, timeForLastPlotToBeRead));

        //ClosePlots();
        // PlotManager.Instance.TriggerPlotEnd();
    }

    // 继续剧情的方法
    public void ContinuePlot()
    {
        if(!PlotManager.instance.GetDuringPlot())
        {
            Debug.LogWarning("当前不在剧情中，点击此按钮无效。");
        }

        PlotManager.instance.TriggerUserAction(false, 0);
    }

    Button CreateContinueButton(string content)
    {
        Debug.Log("calling from selection area");

        Button continueButton = plotSelectionArea.GetComponent<PlotSelectionArea>().NeedButtons(1)[0];
        continueButton.onClick.AddListener(ContinuePlot);

        TextMeshProUGUI buttonText = continueButton.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = content;

        return continueButton;
    }

    private float timeForLastPlotToBeRead = 0f;
    public float wordPerSecond = 10f;
    public float CalculateReadingTime(string content)
    {
        return content.Length/wordPerSecond + 1.5f;
    }

    // 显示对侧对话
    public void ShowDialog(string name, string content)
    {
        plotSelectionArea.GetComponent<PlotSelectionArea>().ClearCurrentButtons();
        
        UnityEngine.Events.UnityEvent newEvents = new UnityEngine.Events.UnityEvent();
        newEvents.AddListener(() => plotDisplayArea.GetComponent<PlotDisplayArea>().PlotNewText(false, name, content));
        newEvents.AddListener(ContinuePlot);

        StartCoroutine(InvokeAfterDelay(newEvents, timeForLastPlotToBeRead));

        timeForLastPlotToBeRead = CalculateReadingTime(content);
        Debug.Log("Time for last plot to be read: " + timeForLastPlotToBeRead);
    }

    private void CreateContinueButtonForSelfDialog(bool isSelf, string characterName, string content)
    {
        Button continueButton = CreateContinueButton(content);
        continueButton.onClick.AddListener(() => plotDisplayArea.GetComponent<PlotDisplayArea>().PlotNewText(true, characterName, content));

    }

    // 显示自身对话
    public void ShowSelfDialog(string characterName, string content)
    {
        plotSelectionArea.GetComponent<PlotSelectionArea>().ClearCurrentButtons();

        UnityEngine.Events.UnityEvent newEvents = new UnityEngine.Events.UnityEvent();
        newEvents.AddListener(() => CreateContinueButtonForSelfDialog(true, characterName, content));
        StartCoroutine(InvokeAfterDelay(newEvents, timeForLastPlotToBeRead)); 
        
        timeForLastPlotToBeRead = CalculateReadingTime(content);
        Debug.Log("Time for last plot to be read: " + timeForLastPlotToBeRead);
    }

    // 显示旁白
    public void ShowNarration(string content)
    {
        Debug.Log("Already abandon this function");
        // plotText.text += $"旁白:\n{content}\n\n";
        // continueButton.SetActive(true);
    }

    private void MakeSelections(List<string> choices)
    {
        List<Button> choicesButtons = plotSelectionArea.GetComponent<PlotSelectionArea>().NeedButtons(choices.Count);

        for (int i = 0; i < choices.Count; i++)
        {
            string choiceText = choices[i];
            int choiceIndex = i;

            Button choiceButton = choicesButtons[i];
            choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = choiceText;
            choiceButton.onClick.AddListener(() => SelectChoice(choiceIndex));
            //choiceButton.onClick.AddListener(ContinuePlot);
        }
    }
    
    // 显示选择项
    public void ShowSelection(List<string> choices)
    {
        plotSelectionArea.GetComponent<PlotSelectionArea>().ClearCurrentButtons();
        
        UnityEngine.Events.UnityEvent newEvents = new UnityEngine.Events.UnityEvent();
        newEvents.AddListener(() => MakeSelections(choices));
        StartCoroutine(InvokeAfterDelay(newEvents, timeForLastPlotToBeRead));

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
        PlotManager.instance.TriggerUserAction(true, choiceIndex);
        // selectionContainer.gameObject.SetActive(false); // 隐藏选择项
        // PlotManager.Instance.TriggerUserAction(true, choiceIndex); // 通知 PlotManager 选择了某项
    }

    private System.Collections.IEnumerator InvokeAfterDelay(UnityEngine.Events.UnityEvent action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action.Invoke();
    }
}
