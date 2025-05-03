using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BorrowBookItemInfo : MonoBehaviour
{
    [SerializeField] private Image _color;
    [SerializeField] private TextMeshProUGUI _name;
    public BookManager.Book _book;
    // Start is called before the first frame update
    void Start()
    {
        InitItem();
    }

    // Update is called once per frame
    void Update()
    {
        CheckStatus();
    }

    public static readonly Dictionary<BookManager.Book.BookType, Color> BookTypeColors = new Dictionary<BookManager.Book.BookType, Color>()
    {
        { BookManager.Book.BookType.fankang, new Color(0.8f, 0, 0, 1f) },
        { BookManager.Book.BookType.fansi, new Color(0, 0, 0.8f, 1f) },
        { BookManager.Book.BookType.huanxiang, new Color(0.8f, 0, 1f, 1f) },
        { BookManager.Book.BookType.shijiao, new Color(0, 0.8f, 0.8f, 1f) },
        { BookManager.Book.BookType.wangxi, new Color(1f, 0.5f, 0, 1f) },
        { BookManager.Book.BookType.yuyan, new Color(0.4f, 0, 0.8f, 1f) },
        { BookManager.Book.BookType.zhishi, new Color(0.9f, 0.9f, 0, 1f) },
        { BookManager.Book.BookType.Unknown, new Color(0.5f, 0.5f, 0.5f, 1f) }
    };
    
    private void InitItem()
    {
        _name.text = _book.name;
        _color.color = new Color(0.5f, 0.5f, 0.5f, 1f);
    }

    private void CheckStatus()
    {
        foreach (BookManager.Book book in GlobalVar.instance.allBooks)
        {
            if (book.id == _book.id)
            {
                _color.color = BookTypeColors[_book.type];
            }
        }
    }
}
