using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreludeProcessManager : MonoBehaviour
{

    public List<GameObject> quotation1;
    public List<GameObject> quotation2;
    public List<GameObject> titles;
    private bool isBusy = false;

    public TextMeshProUGUI waitForInputText;
    // Start is called before the first frame update
    void Start()
    {
        waitForInputText.enabled = false;
        isBusy = false;
    }

    void fadeAwayQuotations() 
    {
        foreach (var quotation in quotation1)
        {
            quotation.GetComponent<TextFader>().enabled = true;
        }
        foreach (var quotation in quotation2)
        {
            quotation.GetComponent<TextFader>().enabled = true;
        }
    }

    void titleAnimated()
    {
        foreach (var title in titles) 
        {
            title.GetComponent<TitleAnimator>().enabled = true;
        }
    }

    void typingQuotations(int num) 
    {
        if (num == 1)
        {
            quotation1[0].GetComponent<TextTyping>().enabled = true;
        }
        else if (num == 2)
        {
            quotation2[0].GetComponent<TextTyping>().enabled = true;
        }
    }
    
    int index = 0;
    // Update is called once per frame
    void checkIfFinished() 
    {
        if (index == 1 && quotation1[0].GetComponent<TextTyping>().isTypingFinished) 
        {
            isBusy = false;
        }
        else if (index == 2 && quotation2[0].GetComponent<TextTyping>().isTypingFinished)
        {
            isBusy = false;
        }
        else if (index == 3 && titles[0].GetComponent<TitleAnimator>().isFinished) 
        {
            isBusy = false;
        }
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isBusy)
        {
            switch (index) 
            {
                case 0:
                    typingQuotations (1);
                    break;
                case 1:
                    typingQuotations (2);
                    break;
                case 2:
                    fadeAwayQuotations();
                    titleAnimated();
                    break;
            }
            isBusy = true;
            index += 1;
        }
        checkIfFinished();
        if (!isBusy) {
            waitForInputText.enabled = true;   
        } else 
        {
            waitForInputText.enabled = false;
        }
    }
}