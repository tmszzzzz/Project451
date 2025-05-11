using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookController : MonoBehaviour
{
    public Journal book;
    public GameObject newContentBookAlertImage;
    public static BookController instance;
    [SerializeField] private Sprite nodesTutorialPage;
    [SerializeField] private Sprite bookmarkTutorialPage;
    [SerializeField] private Sprite nodeTypeTutorialPage;
    [SerializeField] private Sprite exposureTutorialPage;
    [SerializeField] private Sprite indexTutorialPage;
    [SerializeField] private Sprite firewatcherTutorialPage;

    public void addnodesTutorialPage()
    {
        AddOnePageToBook(nodesTutorialPage);
    }
    
    public void addBookmarkTutorialPage()
    {
        AddOnePageToBook(bookmarkTutorialPage);
    }
    
    public void addNodeTypeTutorialPage()
    {
        AddOnePageToBook(nodeTypeTutorialPage);
    }
    
    public void addExposureTutorialPage()
    {
        AddOnePageToBook(exposureTutorialPage);
    }
    
    public void addIndexTutorialPage()
    {
        AddOnePageToBook(indexTutorialPage);
    }
    
    public void addFirewatcherTutorialPage()
    {
        AddOnePageToBook(firewatcherTutorialPage);
    }
    
    
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
