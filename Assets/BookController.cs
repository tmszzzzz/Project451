using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookController : MonoBehaviour
{
    public Book book;
    public GameObject newContentBookAlertImage;

    public void addOnePageToBook(Sprite pageSpirite)
    {
        book.addOnePageToEndOfBook(pageSpirite);
        newContentBookAlertImage.SetActive(true);
    }

    public void disableAlarm()
    {
        newContentBookAlertImage.SetActive(false);
    }

    void Start()
    {
        disableAlarm();
    }
}
