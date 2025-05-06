using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ToggleBookButton : MonoBehaviour
{
    public bool isBookOpen = false;
    public Transform bookActivatedPosition;
    public Transform bookDeactivatedPosition;
    public GameObject book;
    public Image shadowBackground;
    public float shadowBackgroundAlpha = 233f;

    private static ToggleBookButton _instance;
    
    public static  ToggleBookButton Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType< ToggleBookButton>();
            }

            return _instance;
        }
    }
    public void ToggleBook()
    {
        isBookOpen = !isBookOpen;
        GlobalVar.instance.closeInfoPanel = true;
        if (isBookOpen)
        {
            RoundManager.instance.OperationForbidden();
        }
        else
        {
            RoundManager.instance.OperationRelease();
        }
    } 

    // Update is called once per frame
    void Update()
    {
        if (GlobalVar.instance.openInfoPanel)
        {
            isBookOpen = !isBookOpen;
            GlobalVar.instance.openInfoPanel = false;
        }
        if (isBookOpen)
        {
            book.transform.position = Vector3.Lerp(book.transform.position, bookActivatedPosition.position, Time.deltaTime * 3f);
            Color shadowColor = shadowBackground.color;
            shadowBackground.gameObject.SetActive(true);
            shadowColor.a = Mathf.Lerp(shadowColor.a, shadowBackgroundAlpha, Time.deltaTime * 3f);
            shadowBackground.color = shadowColor;
        }
        else
        {
            book.transform.position = Vector3.Lerp(book.transform.position, bookDeactivatedPosition.position, Time.deltaTime * 3f);
            Color shadowColor = shadowBackground.color;
            shadowColor.a = Mathf.Lerp(shadowColor.a, 0, Time.deltaTime * 3f);
            if(shadowColor.a <= 0.01f) shadowBackground.gameObject.SetActive(false);
            shadowBackground.color = shadowColor;
        }
    }
}
