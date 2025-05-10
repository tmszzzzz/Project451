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
        { BookManager.Book.BookType.fankang, new Color(0.9f, 0.219f, 0.094f, 1f) },
        { BookManager.Book.BookType.fansi, new Color(0.125f, 0.654f, 0.87f, 1f) },
        { BookManager.Book.BookType.huanxiang, new Color(0.996f, 0.459f, 0.9f, 1f) },
        { BookManager.Book.BookType.shijiao, new Color(0.65f, 0.313f, 0.815f, 1f) },
        { BookManager.Book.BookType.wangxi, new Color(0.698f, 0.537f, 0.349f, 1f) },
        { BookManager.Book.BookType.yuyan, new Color(0.996f, 0.682f, 0.133f, 1f) },
        { BookManager.Book.BookType.zhishi, new Color(0.058f, 0.76f, 0.545f, 1f) },
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
