using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookController : MonoBehaviour
{
    public Book book;
    public GameObject newContentBookAlertImage;
    public static BookController instance;
    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void AddOnePageToBook(Sprite pageSpirite)
    {
        book.addOnePageToEndOfBook(pageSpirite);
        TurnPageTo(book.bookPages.Length);
        Alarm();
    }

    public void TurnPageTo(int num)
    {
        book.currentPage = num % 2 == 0? num - 1 : num;
        book.UpdateSprites();
    }

    public void subsititute(Sprite pageSprite, int num)
    {
        book.SubstitutueAPageInTheMiddle(pageSprite, num);
        TurnPageTo(num);
        Alarm(); 
    }

    public void Alarm()
    {
        newContentBookAlertImage.SetActive(true);
    }

    public void DisableAlarm()
    {
        newContentBookAlertImage.SetActive(false);
    }

    void Start()
    {
        DisableAlarm();
    }
}
