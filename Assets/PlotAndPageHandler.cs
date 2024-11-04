using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlotAndPageHandler : MonoBehaviour
{
    bool isPlotting = false;
    bool isPaging = false;
    [SerializeField] private GameObject plotButton;
    [SerializeField] private GameObject pageButton;
    [SerializeField] private GameObject thisNode;
    [SerializeField] private float targetScale = 1.5f;
    private string plotFilename;
    private Sprite pageSprite;
    void Start()
    {
        plotButton.SetActive(false);
        pageButton.SetActive(false);
        plotFilename = thisNode.GetComponent<NodeBehavior>().properties.plotFileName;
        pageSprite = thisNode.GetComponent<NodeBehavior>().properties.pageSprite;
    }
    // Update is called once per frame
    void Update()
    {
        float targetPlotScale = isPlotting ? targetScale : 0f;
        float targetPageScale = isPaging ? targetScale : 0f;
        float lerpSpeed = 5f;

        plotButton.transform.localScale = Vector3.Lerp(plotButton.transform.localScale, new Vector3(targetPlotScale, targetPlotScale, targetPlotScale), Time.deltaTime * lerpSpeed);
        pageButton.transform.localScale = Vector3.Lerp(pageButton.transform.localScale, new Vector3(targetPageScale, targetPageScale, targetPageScale), Time.deltaTime * lerpSpeed);
    }

    public void OnAwakeShowButtons()
    {
        if (plotFilename != null)
        {
            plotButton.SetActive(true);
            isPlotting = true;
        }

        if (pageSprite != null)
        {
            pageButton.SetActive(true);
            isPaging = true;
        }
    }

    public void OnSinkHideButtons()
    {
        if (plotFilename != null)
        {
            plotButton.SetActive(false);
            isPlotting = false;
        }

        if (pageSprite != null)
        {
            pageButton.SetActive(false);
            isPaging = false;
        }
    }

    public void Plot()
    {
        Debug.Log("Node"  + " is plotting " + plotFilename);
        isPlotting = false;
    }

    public void Page()
    {
        Debug.Log("Node" + " is paging " + pageSprite.name);
        isPaging = false;
    }
}
