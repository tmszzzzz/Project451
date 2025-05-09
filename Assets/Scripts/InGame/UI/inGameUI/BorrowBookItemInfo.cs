using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BorrowBookItemInfo : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _name;
    public BookManager.Book _book;
    private Color _color = new Color(0.5f, 0.5f, 0.5f, 1f);

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

    public void UpdateItem()
    {
        _name.text = _book.name;
        _image.color = GetBookColor(_book.id); // 优化查找逻辑
        
        // 延迟一帧再计算位置
        StartCoroutine(UpdatePositionNextFrame());
    }
    
    private IEnumerator UpdatePositionNextFrame()
    {
        yield return null; // 等待一帧，确保布局完成
    
        // 强制刷新布局
        LayoutRebuilder.ForceRebuildLayoutImmediate(_name.rectTransform);
    
        float textWidth = _name.preferredWidth;
    
        _name.rectTransform.anchoredPosition = new Vector2(textWidth / 2 + 5, 0);
        _image.rectTransform.anchoredPosition = new Vector2(textWidth + 10, 0);
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(_name.rectTransform);
    }
    
    private Color GetBookColor(int bookId)
    {
        if (GlobalVar.instance.allBooks.Contains(bookId))
        {
            return BookTypeColors[_book.type];
        }
        return _color;
    }
}
