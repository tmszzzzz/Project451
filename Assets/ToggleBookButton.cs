using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ToggleBookButton : MonoBehaviour
{
    private bool isBookOpen = false;
    public Transform bookActivatedPosition;
    public Transform bookDeactivatedPosition;
    public GameObject book;
    public Image shadowBackground;
    public float shadowBackgroundAlpha = 233f;

    public void ToggleBook()
    {
        isBookOpen = !isBookOpen;
    } 

    // Update is called once per frame
    void Update()
    {
        if (isBookOpen)
        {
            book.transform.position = Vector3.Lerp(book.transform.position, bookActivatedPosition.position, Time.deltaTime * 5f);
            Color shadowColor = shadowBackground.color;
            shadowColor.a = Mathf.Lerp(shadowColor.a, shadowBackgroundAlpha, Time.deltaTime * 5f);
            shadowBackground.color = shadowColor;
        }
        else
        {
            book.transform.position = Vector3.Lerp(book.transform.position, bookDeactivatedPosition.position, Time.deltaTime * 5f);
            Color shadowColor = shadowBackground.color;
            shadowColor.a = Mathf.Lerp(shadowColor.a, 0, Time.deltaTime * 5f);
            shadowBackground.color = shadowColor;
        }
    }
}