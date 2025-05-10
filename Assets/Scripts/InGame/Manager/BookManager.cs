using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class BookManager : MonoBehaviour
{
    [System.Serializable]
    public class BookRandomConfig
    {
        public List<float> LevelWeights; // 等级权重
        public List<Book.BookType> AllowedTypes; // 允许的类型
        
        // 无参构造
        public BookRandomConfig()
        {
            LevelWeights = new List<float>();
            AllowedTypes = new List<Book.BookType>();
        }
        
        public BookRandomConfig(BookRandomConfig config)
        {
            // 使用null合并运算符提供默认值
            LevelWeights = new List<float>(config.LevelWeights);
            AllowedTypes = new List<Book.BookType>(config.AllowedTypes);
        }
    }
    
    
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
        [JsonConstructor]
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
    public Book GetRandomBook(BookRandomConfig config = null)
    {
        if (illustration.Count == 0) return null;
        config ??= GlobalVar.instance.bookRandomConfig;
        // 可获得的类型的书按照基础影响力分为三类
        List<List<Book>> accessBooks = GenerateTypeList(config.AllowedTypes);
        // 按概率获取去哪一类的基础影响力获得书籍
        int levelIndex = getLevelIndex(config.LevelWeights);
        int index = Random.Range(0, accessBooks[levelIndex].Count);
        Book book = new Book(accessBooks[levelIndex][index]);
        // 借书单判断用
        GlobalVar.instance.allBooks.Add(book.id);
        return book;
    }

    public List<List<Book>> GenerateTypeList(List<Book.BookType> AllowedTypes)
    {
        List<List<Book>> typeList = new List<List<Book>>()
        {
            new List<Book>(), // 基础影响力0级
            new List<Book>(), // 基础影响力1级
            new List<Book>()  // 基础影响力2级
        };
        for (int i = 0; i < illustration.Count; i++)
        {
            if (AllowedTypes.Contains(illustration[i].type))
            {
                typeList[illustration[i].basicInfluence - 1].Add(illustration[i]);
            } 
        }
        return typeList;
    }

    public int getLevelIndex(List<float> levelWeights)
    {
        // 参数合法性校验
        if (levelWeights == null || levelWeights.Count == 0)
        {
            Debug.LogError("概率配置错误：权重数组不能为空");
            return -1;
        }

        // 计算总权重并校验负值
        float totalWeight = 0;
        foreach (float weight in levelWeights)
        {
            if (weight < 0)
            {
                Debug.LogError("概率配置错误：权重值不能为负数");
                return -1;
            }
            totalWeight += weight;
        }

        if (totalWeight <= 0)
        {
            Debug.LogError("概率配置错误：总权重小于0");
            return -1;
        }

        // 生成随机采样点
        float randomValue = Random.Range(0f, totalWeight);
    
        // 累加权重寻找命中区间
        float cumulative = 0f;
        for (int i = 0; i < levelWeights.Count; i++)
        {
            cumulative += levelWeights[i];
            if (randomValue <= cumulative)
            {
                return i;
            }
        }

        // 兜底逻辑（理论上不会执行到这里）
        Debug.LogWarning("存在未知错误");
        return levelWeights.Count - 1;
    }
}

