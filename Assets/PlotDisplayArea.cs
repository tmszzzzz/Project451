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
    private List<GameObject> plots = new List<GameObject>();
    [SerializeField] private RectTransform plotDisplayArea;
    [SerializeField] private float maxWidth;
    [SerializeField] private float textFontSize;
    [SerializeField] private ProfileData profileData;

    void Awake()
    {
        selfProfileImage = selfProfile.GetComponent<Image>();
        otherProfileImage = otherProfile.GetComponent<Image>();
        selfName = selfProfile.GetComponentInChildren<TextMeshProUGUI>();
        otherName = otherProfile.GetComponentInChildren<TextMeshProUGUI>();
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
        foreach (GameObject plot in plots)
        {
            Destroy(plot);
        }
        plots.Clear();
    }

    void DestroyOverScreenPlots()
    {

        //destroy plots that are out of screen, which have a y position that are higher than half of the screen height
        foreach (GameObject plot in plots)
        {
            if (plot.GetComponent<RectTransform>().anchoredPosition.y > Screen.height)
            {
                Debug.Log("Destroy plot");
                plots.Remove(plot);
                Destroy(plot);
            }
        }
    }

    void MovePlotsUp(float distance)
    {

       plotDisplayArea.sizeDelta = new Vector2(plotDisplayArea.sizeDelta.x, plotDisplayArea.sizeDelta.y + distance);

        foreach (GameObject plot in plots)
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
            selfName.text = name;
        }
        else
        {
            ControleProfileActive(false, true);
            otherProfileImage.sprite = profileData.GetProfileSpriteByName(name);
            otherName.text = name;
        }
    }

    string wrapUpNameWithRichText(string name, bool isSelf)
    {
        name = "<color=#" + profileData.GetColorByName(name).ToHexString() + ">" + name + "</color>";
        name = "<align=\"center\">" + name + "</align>";
        return name;
    }

    public void PlotNewText(bool isSelf, string name, string context)
    {
        string presentedName = wrapUpNameWithRichText(name, isSelf);

        GameObject newPlotTextGameObject = new();
        newPlotTextGameObject.transform.SetParent(gameObject.transform);

        ContentSizeFitter contentSizeFitter = newPlotTextGameObject.AddComponent<ContentSizeFitter>();
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        TextMeshProUGUI newPlotText = newPlotTextGameObject.AddComponent<TextMeshProUGUI>();
        newPlotText.enableWordWrapping = true;
        newPlotText.overflowMode = TextOverflowModes.Overflow;
        newPlotText.alignment = TextAlignmentOptions.TopLeft;
        newPlotText.text = presentedName + "\n" + context;
        newPlotText.alignment = TextAlignmentOptions.Left;
        newPlotText.fontSize = textFontSize;

        RectTransform rectTransform = newPlotTextGameObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(maxWidth, 25f);
        rectTransform.pivot = isSelf ? new Vector2(0, 0) : new Vector2(1, 0);
        rectTransform.anchorMin = isSelf ? new Vector2(0, 0) : new Vector2(1, 0f);
        rectTransform.anchorMax = isSelf ? new Vector2(0, 0f) : new Vector2(1, 0f);
        rectTransform.localScale = new Vector3(1, 1, 1);

        rectTransform.anchoredPosition = new Vector2(isSelf ? 10 : -10, 0);

        //别删除，需要这段强制更新一次layout使得contentSizeFitter生效
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        MovePlotsUp(rectTransform.rect.height);
        plots.Add(newPlotTextGameObject);
        plotDisplayArea.anchoredPosition = new Vector2(plotDisplayArea.anchoredPosition.x, plotDisplayArea.anchoredPosition.y - rectTransform.rect.height);

        DestroyOverScreenPlots();

        UpdateProfile(isSelf, name);
    }

    public float startYPositionOfDisplayedPlots = 10f;
    public float lerpScreenSpeed;
    void Update()
    {
        Vector2 targetPosition = new Vector2(plotDisplayArea.anchoredPosition.x, startYPositionOfDisplayedPlots);
        plotDisplayArea.anchoredPosition = Vector2.Lerp(plotDisplayArea.anchoredPosition, targetPosition, Time.deltaTime * lerpScreenSpeed);
    }

    // Update is called once per frame
}
