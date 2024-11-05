using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public bool hasBook = false;

    public Image hasBookImage;
    public Image noBookImage;
    // Update is called once per frame
    void Update()
    {
        if(hasBook)
        {
            hasBookImage.gameObject.SetActive(true);
            noBookImage.gameObject.SetActive(false);
        }
        else 
        {
            hasBookImage.gameObject.SetActive(false);
            noBookImage.gameObject.SetActive(true);
        }
    }
}
