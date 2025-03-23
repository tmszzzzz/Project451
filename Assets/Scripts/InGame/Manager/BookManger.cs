using System.Collections.Generic;
using UnityEngine;

public class BookManager : MonoBehaviour
{
    public static BookManager instance;

    // Book结构体
    [System.Serializable]
    public struct BookInfo
    {
        public string id;                   // 唯一书籍ID
        public string name;                 // 书名
        public string description;          // 描述
        public int basic_influence;         // 提供的基础影响力值
        public int additional_influence;    // 提供的额外影响力值
        public BookType type;               // 书籍类型，用于分类随机获取
    }

    [System.Serializable]
    public enum BookType
    {
        A,
        B
    }

    [SerializeField] private BooksData booksData;
    public BookInfo[] bookDictionary;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        if (booksData == null)
        {
            Debug.LogError("BooksData 未配置！");
            return;
        }
        bookDictionary = booksData.booksInfo;
    }

    // 随机获取一本书
    public BookInfo GetRandomBook()
    {
        if (bookDictionary.Length == 0) return default;
        int index = Random.Range(0, bookDictionary.Length);
        return bookDictionary[index];
    }
}
