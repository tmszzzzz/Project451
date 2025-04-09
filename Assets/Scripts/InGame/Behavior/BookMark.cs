using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class BookMark : MonoBehaviour
{
    [SerializeField] private Image colorImage;
    [SerializeField] private Image patternImage;
    [SerializeField] private TextMeshProUGUI bookNameText;

    // Update is called once per frame
    void Update()
    {
        
    }

    // 配置书签视觉和交互
    public void ConfigureBookmark(BookManager.Book book)
    {
        colorImage.color = BookManager.Book.GetBookMarksColor(book.type);
        // 设置花纹
        if (book.additionalInfluence > 1)
        {
            // patternImage.sprite = ;
        }
        bookNameText.text = book.name;
    }
}
