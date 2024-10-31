using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;  
public class PlotDisplayArea : MonoBehaviour
{
    public float lerpAlphaSpeed = 10f;
    private List<GameObject> plots = new List<GameObject>();
    [SerializeField] private RectTransform plotDisplayArea;
    [SerializeField] private float maxWidth;
    [SerializeField] private float textFontSize;

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

    public void PlotNewText(bool isSelf, string name, string context)
    {
        GameObject newPlotTextGameObject = new();
        newPlotTextGameObject.transform.SetParent(gameObject.transform);

        ContentSizeFitter contentSizeFitter = newPlotTextGameObject.AddComponent<ContentSizeFitter>();
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        TextMeshProUGUI newPlotText = newPlotTextGameObject.AddComponent<TextMeshProUGUI>();
        newPlotText.enableWordWrapping = true;
        newPlotText.overflowMode = TextOverflowModes.Overflow;
        newPlotText.alignment = TextAlignmentOptions.TopLeft;
        newPlotText.text = name + ":\n" + context;
        newPlotText.alignment = isSelf ? TextAlignmentOptions.Left : TextAlignmentOptions.Right;
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
        Debug.Log(rectTransform.rect.height);
        plots.Add(newPlotTextGameObject);
        DestroyOverScreenPlots();
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
}
