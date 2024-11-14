using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlotAndPageHandler : MonoBehaviour
{
    bool isPaging = false;
    [SerializeField] private GameObject pageButton;
    [SerializeField] private GameObject thisNode;
    [SerializeField] private float targetScale = 1.5f;
    private Sprite pageSprite;
    void Start()
    {
        pageButton.SetActive(false);
        pageSprite = thisNode.GetComponent<NodeBehavior>().pageSprite;
    }
    // Update is called once per frame
    void Update()
    {
        float targetPageScale = isPaging ? targetScale : 0f;
        float lerpSpeed = 5f;
        
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
        Debug.Log("Don't call this function");
    }

    public void Page()
    {
        if (isPaging)
        {
            Debug.Log("Node" + " is paging " + pageSprite.name);
            GlobalVar.instance.AddResourcePoint(1);
            isPaging = false;

            //clear pageSprite  
            pageSprite = null;

            pageButton.GetComponent<BreathingEffect>().enabled = false;
        }
    }
}
