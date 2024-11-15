using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;  
public class PlotDisplayArea : MonoBehaviour
{
    public float lerpAlphaSpeed = 10f;
    [SerializeField] private GameObject selfProfile;
    [SerializeField] private GameObject otherProfile;

    private Image selfProfileImage;
    private Image otherProfileImage;
    private TextMeshProUGUI selfName;
    private TextMeshProUGUI otherName;
    private List<GameObject> adreadyFinishedPlots = new List<GameObject>();
    [SerializeField] private RectTransform plotDisplayArea;
    [SerializeField] private float maxWidth;
    [SerializeField] private float textFontSize;
    [SerializeField] private ProfileData profileData;
    [SerializeField] private TMP_FontAsset fontAsset;

    void Awake()
    {
        selfProfileImage = selfProfile.GetComponent<Image>();
        otherProfileImage = otherProfile.GetComponent<Image>();
        selfName = selfProfile.GetComponentInChildren<TextMeshProUGUI>();
        otherName = otherProfile.GetComponentInChildren<TextMeshProUGUI>();
        selfName.font = fontAsset;
        otherName.font = fontAsset;
    }

    public void OpenPlots()
    {
        ControleProfileActive(true, false);
        ControleProfileActive(false, false);
    }

    void ControleProfileActive(bool isSelf, bool conditon)
    {
        if (isSelf)
        {
            selfProfile.SetActive(conditon);
        }
        else
        {
            otherProfile.SetActive(conditon);
        }
    }

    public void ClosePlots()
    {
        foreach (GameObject plot in adreadyFinishedPlots)
        {
            Destroy(plot);
        }
        adreadyFinishedPlots.Clear();
    }

    void MovePlotsUp(float distance)
    {

       plotDisplayArea.sizeDelta = new Vector2(plotDisplayArea.sizeDelta.x, plotDisplayArea.sizeDelta.y + distance);

        foreach (GameObject plot in adreadyFinishedPlots)
        {
                plot.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, distance);
        }
    }

    void UpdateProfile(bool isSelf, string name)
    {
        if (isSelf)
        {
            ControleProfileActive(true, true);
            selfProfileImage.sprite = profileData.GetProfileSpriteByName(name);
            selfName.text = WrapUpNameWithRichText(name);
        }
        else
        {
            ControleProfileActive(false, true);
            otherProfileImage.sprite = profileData.GetProfileSpriteByName(name);
            otherName.text = WrapUpNameWithRichText(name);
        }
    }

    string WrapUpNameWithRichText(string name)
    {
        name = "<color=#" + profileData.GetColorByName(name).ToHexString() + ">" + name + "</color>";
        name = "<align=\"center\">" + name + "</align>";
        return name;
    }

    private GameObject currentPlottingPlot = null;
    private float currentPlottingHeight = 0;

    private RectTransform ChangeThePositionOfNarratorToMiddle(RectTransform originalRect)
    {
        originalRect.anchorMin = new Vector2(0.5f, 0);
        originalRect.anchorMax = new Vector2(0.5f, 0);
        originalRect.pivot = new Vector2(0.5f, 0);
        originalRect.anchoredPosition = new Vector2(0, 0);
        return originalRect;
    }
    
    public void PlotNewText(bool isSelf, string name, string context)
    {
        string presentedName = WrapUpNameWithRichText(name);

        GameObject newPlotTextGameObject = new();
        newPlotTextGameObject.transform.SetParent(gameObject.transform);

        ContentSizeFitter contentSizeFitter = newPlotTextGameObject.AddComponent<ContentSizeFitter>();
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        TextMeshProUGUI newPlotText = newPlotTextGameObject.AddComponent<TextMeshProUGUI>();
        newPlotText.enableWordWrapping = true;
        newPlotText.overflowMode = TextOverflowModes.Overflow;
        newPlotText.alignment = TextAlignmentOptions.TopLeft;
        newPlotText.text = presentedName + "\n";
        newPlotText.alignment = TextAlignmentOptions.Left;
        newPlotText.fontSize = textFontSize;
        newPlotText.font = fontAsset;

        RectTransform rectTransform = newPlotTextGameObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(maxWidth, 25f);
        rectTransform.pivot = isSelf ? new Vector2(0, 0) : new Vector2(1, 0);
        rectTransform.anchorMin = isSelf ? new Vector2(0, 0) : new Vector2(1, 0f);
        rectTransform.anchorMax = isSelf ? new Vector2(0, 0f) : new Vector2(1, 0f);
        rectTransform.localScale = new Vector3(1, 1, 1);
        rectTransform.anchoredPosition = new Vector2(isSelf ? 10 : -10, 0);

        if (name == "旁白")
        {
            rectTransform = ChangeThePositionOfNarratorToMiddle(rectTransform);
        }
        
        // //别删除，需要这段强制更新一次layout使得contentSizeFitter生效
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        TextAppearOneByOne textAppearOneByOne = newPlotTextGameObject.AddComponent<TextAppearOneByOne>();
        textAppearOneByOne.fullText = context;
        textAppearOneByOne.typingSpeed = 1f / PlotDisplay.Instance.wordPerSecond;
        textAppearOneByOne.onTypingFinished.AddListener(() => CurrentPlottingPlotFinished());
        
        MovePlotsUp(rectTransform.sizeDelta.y);
        currentPlottingHeight = rectTransform.sizeDelta.y;
        currentPlottingPlot = newPlotTextGameObject;

        UpdateProfile(isSelf, name);

    }

    void UpdateHeightOfAlreadyFinishedPlots()
    {
        if (currentPlottingPlot == null) return;

        RectTransform rectTransform = currentPlottingPlot.GetComponent<RectTransform>();
        
        if (currentPlottingHeight != rectTransform.sizeDelta.y)
        {
            MovePlotsUp(rectTransform.sizeDelta.y - currentPlottingHeight);
            currentPlottingHeight = rectTransform.sizeDelta.y;
        }
    }

    public void CurrentPlottingPlotFinished()
    {
        RectTransform rectTransform = currentPlottingPlot.GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        UpdateHeightOfAlreadyFinishedPlots();
        adreadyFinishedPlots.Add(currentPlottingPlot);
        currentPlottingPlot = null;
    }
    
    
    void Update()
    {
        UpdateHeightOfAlreadyFinishedPlots();
    }

    // Update is called once per frame
}
