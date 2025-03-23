using System.Collections.Generic;
using UnityEngine;

public class BookManager : MonoBehaviour
{
    public static BookManager instance;

    // Book�ṹ��
    [System.Serializable]
    public struct BookInfo
    {
        public string id;                   // Ψһ�鼮ID
        public string name;                 // ����
        public string description;          // ����
        public int basic_influence;         // �ṩ�Ļ���Ӱ����ֵ
        public int additional_influence;    // �ṩ�Ķ���Ӱ����ֵ
        public BookType type;               // �鼮���ͣ����ڷ��������ȡ
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
            Debug.LogError("BooksData δ���ã�");
            return;
        }
        bookDictionary = booksData.booksInfo;
    }

    // �����ȡһ����
    public BookInfo GetRandomBook()
    {
        if (bookDictionary.Length == 0) return default;
        int index = Random.Range(0, bookDictionary.Length);
        return bookDictionary[index];
    }
}
