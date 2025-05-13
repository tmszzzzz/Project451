using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ImagePlayController : MonoBehaviour
{
    public static ImagePlayController instance;
    public List<ImagePlay> imagePlayPages;
    private int currentPage = 0;
    public bool first = true;
    public TextMeshProUGUI firstText;
    public bool finished = false;
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        PlayNextPage();
    }
    
    public void PlayNextPage()
    {
        if (currentPage < imagePlayPages.Count && currentPage > -1)
        {
            imagePlayPages[currentPage++].gameObject.SetActive(true);
        }
        else
        {
            finished = true;
            // gameObject.SetActive(false);
        }
    }
}