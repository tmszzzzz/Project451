using System.Collections.Generic;
using UnityEngine;

public class BookManager : MonoBehaviour
{
    public static BookManager instance;
    // public int assignRuntimeId = 0;
    
    [System.Serializable]
    public class Book
    {
        public enum BookType
        {
            TravelerFlint,
            StonemasonChisel
        }
        public int id;                   // 唯一书籍ID
        public string name;                 // 书名
        public string description;          // 描述
        public int basicInfluence;          // 提供的基础影响力值
        public int additionalInfluence;     // 提供的额外影响力值
        public BookType type;               // 书籍类型，用于分类随机获取
        public bool isPreallocatedIn;       // 是否被预分配入
        public bool isPreallocatedOut;      // 是否被预分配出
        public int parentId;                // 所属节点ID
        // public int runtimeId;            // 运行时ID

        public Book(Book book)
        {
            this.id = book.id;
            this.name = book.name;
            this.description = book.description;
            this.basicInfluence = book.basicInfluence;
            this.additionalInfluence = book.additionalInfluence;
            this.type = book.type;
            this.isPreallocatedIn = book.isPreallocatedIn;
            this.isPreallocatedOut = book.isPreallocatedOut;
            this.parentId = book.parentId;
            // this.runtimeId = book.runtimeId;
        }

        public GameObject GetParent() 
        {
            return CanvasBehavior.instance.GetNodeByID(this.parentId);    
        }
    }
    
    [SerializeField] private BooksData booksData;
    public List<Book> illustration;

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
            Debug.LogError("BooksData is null");
            return;
        }
        illustration = booksData.books;
    }

    // 随机获取一本书
    public Book GetRandomBook()
    {
        if (illustration.Count == 0) return null;
        int index = Random.Range(0, illustration.Count);
        // illustration[index].runtimeId = assignRuntimeId++;
        return illustration[index];
    }

    
}

