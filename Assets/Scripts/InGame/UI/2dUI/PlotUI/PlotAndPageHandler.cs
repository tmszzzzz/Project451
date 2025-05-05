using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlotAndPageHandler : MonoBehaviour
{
    public bool isPaging = false;
    [SerializeField] private GameObject pageButton;
    [SerializeField] private GameObject thisNode;
    [SerializeField] private float targetScale = 1.5f;
    [SerializeField] private Sprite pageSprite;
    private bool onLoad = false;
    void Start()
    {
        if(!onLoad) pageButton.SetActive(false);
        pageSprite = thisNode.GetComponent<NodeBehavior>().pageSprite;
    }
    // Update is called once per frame
    void Update()
    {
        float targetPageScale = isPaging ? targetScale : 0f;
        float lerpSpeed = 5f;

        //plotButton.transform.localScale = Vector3.Lerp(plotButton.transform.localScale, new Vector3(targetPlotScale, targetPlotScale, targetPlotScale), Time.deltaTime * lerpSpeed);
        pageButton.transform.localScale = Vector3.Lerp(pageButton.transform.localScale, new Vector3(targetPageScale, targetPageScale, targetPageScale), Time.deltaTime * lerpSpeed);
    }

    public void OnAwakeShowButtons()
    {
        if (pageSprite != null)
        {
            pageButton.SetActive(true);
            isPaging = true;
        }
    }

    public void OnLoadShowButtons()
    {
        pageButton.SetActive(true);
        isPaging = true;
        onLoad = true;
    }

    public void OnFallHideButtons()
    {

        if (pageSprite != null)
        {
            pageButton.SetActive(false);
            isPaging = false;
        }
    }

    public void Plot()
    {
        
    }

    public void Page()
    {
        if (isPaging)
        {
            //Debug.Log("Node" + " is paging " + pageSprite.name);
            GlobalVar.instance.AddResourcePoint(1);
            BookController.instance.AddOnePageToBook(pageSprite);
            
            isPaging = false;
            //clear pageSprite  
            pageSprite = null;
            pageButton.GetComponent<BreathingEffect>().enabled = false;
            CameraBehavior.instance.GetComponent<AudioSource>().PlayOneShot(CameraBehavior.instance.pop);
        }
    }
}
