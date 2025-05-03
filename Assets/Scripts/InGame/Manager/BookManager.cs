using System.Collections.Generic;
using UnityEngine;

public class BookManager : MonoBehaviour
{
    public static BookManager instance;

    [System.Serializable]
    public class Book
    {
        public enum BookType
        {
            fankang,
            fansi,
            huanxiang,
            shijiao,
            wangxi,
            yuyan,
            zhishi,
            Unknown
        }

        private static Dictionary<string, BookType> _dictionary = new Dictionary<string, BookType>
        {
            { "反抗", BookType.fankang },
            { "反思", BookType.fansi },
            { "幻想", BookType.huanxiang },
            { "视角", BookType.shijiao },
            { "往昔", BookType.wangxi },
            { "预言", BookType.yuyan },
            { "知识", BookType.zhishi },
        };

        public static BookType ParseCnToBookType(string bt)
        {
            BookType v;
            if (_dictionary.TryGetValue(bt, out v))
            {
                return v;
            }

            return BookType.Unknown;
        }

        public int id; // 唯一书籍ID
        public string name; // 书名
        public string description; // 描述
        public int basicInfluence; // 提供的基础影响力值
        public int additionalInfluence; // 提供的额外影响力值
        public BookType type; // 书籍类型，用于分类随机获取
        public bool isPreallocatedIn; // 是否被预分配入
        public bool isPreallocatedOut; // 是否被预分配出
        public int parentId; // 所属节点ID

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

        // 带参数的构造函数
        public Book(int id, string name, string description, int basicInfluence, int additionalInfluence, BookType type,
            bool isPreallocatedIn, bool isPreallocatedOut, int parentId)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.basicInfluence = basicInfluence;
            this.additionalInfluence = additionalInfluence;
            this.type = type;
            this.isPreallocatedIn = isPreallocatedIn;
            this.isPreallocatedOut = isPreallocatedOut;
            this.parentId = parentId;
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
        Book book = new Book(illustration[index]);
        GlobalVar.instance.allBooks.Add(book);
        return book;
    }
}

