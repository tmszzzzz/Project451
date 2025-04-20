using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NodeBehavior : BaseNodeBehavior
{
    public Properties properties;
    public PlotAndPageHandler plotAndPageHandler;
    public MessageBar mb;
    [SerializeField] protected Image objColor;
    protected Dictionary<int, Color> ColorMap;
    public bool hadAwakenedBefore = false;

    public string description = "";
    public string plotFileName = "";
    public Sprite pageSprite;
    [SerializeField] protected GameObject bookmarkPrefab; // 书签预制体
    private List<GameObject> spawnedBookmarks = new List<GameObject>();
    public float bookmarkSpacing = 1f;                      // 书签间距
    public float height = 1.3f;                             // 书签高度
    public float xScale = 1f;
    public float yScale = 1f;
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (String.IsNullOrEmpty(plotFileName)) plotFileName = "null";

        ColorMap = new Dictionary<int, Color>();
        ColorMap.Add(-1, Color.gray);
        ColorMap.Add(0, Color.gray);
        ColorMap.Add(1, Color.yellow);
        ColorMap.Add(2, Color.red);
        mb = MessageBar.instance;
        if (properties.state > 0)
        {
            // 生成书签
            GenerateBookmarks();
        }
    }

    protected virtual void Update()
    {
        objColor.color = ColorMap[(int)properties.state];
        if (transform.parent.GetComponent<CanvasBehavior>().editorMode)
        {
            GetComponent<MeshRenderer>().material.color = ColorMap[properties.region];
        }
    }

    public override StatePrediction NowState()
    {
        // 考虑所有未被预分配入的书，而且不关心它们是否被预分配出
        // 修改此方法的判断逻辑时，请记得一并修改PredictState
        
        // 单例模式省略下面的内容
        // CanvasBehavior cb = transform.parent.GetComponent<CanvasBehavior>();
        // if (cb == null)
        // {
        //     Debug.LogWarning("Script \"CanvasBehavior\" not found in canvas.");
        //     return new StatePrediction(Properties.StateEnum.DEAD,0);
        // }
        
        List<GameObject> nList = CanvasBehavior.instance.GetNeighbors(gameObject);

        // 处理自身影响力
        int basicInfluence = 0;
        int additionalInfluence = 0;
        List<BookManager.Book.BookType> types = properties.GetBookType();
        foreach (var book in properties.books)
        {
            if (!book.isPreallocatedIn)
            {
                basicInfluence += book.basicInfluence;
                if (types.Contains(book.type))
                {
                    additionalInfluence += book.additionalInfluence;
                }
            }
        }
        
        // 处理周围节点影响力
        foreach (GameObject go in nList)
        {
            NodeBehavior cub = go.GetComponent<NodeBehavior>();
            if (cub != null && cub.properties.state > 0)
            {
                types = cub.properties.GetBookType();
                foreach (var book in cub.properties.books)
                {
                    if (!book.isPreallocatedIn)
                    {
                        basicInfluence += book.basicInfluence;
                        if (types.Contains(book.type))
                        {
                            additionalInfluence += book.additionalInfluence;
                        }
                    }
                }
            }
        }
        return new StatePrediction(properties.state, basicInfluence, additionalInfluence);
    }

    public override StatePrediction PredictState()
    {
        // 考虑所有未被预分配出的书，而且不关心它们是否被预分配入
        // 修改此方法的判断逻辑时，请记得一并修改NowState
        
        // CanvasBehavior cb = transform.parent.GetComponent<CanvasBehavior>();
        // if (cb == null)
        // {
        //     Debug.LogWarning("Script \"CanvasBehavior\" not found in canvas.");
        //     return new StatePrediction(Properties.StateEnum.DEAD,0);
        // }
        
        List<GameObject> nList = CanvasBehavior.instance.GetNeighbors(gameObject);

        // 处理自身影响力
        int basicInfluence = 0;
        int additionalInfluence = 0;
        List<BookManager.Book.BookType> types = properties.GetBookType();
        foreach (var book in properties.books)
        {
            if (!book.isPreallocatedOut)
            {
                basicInfluence += book.basicInfluence;
                if (types.Contains(book.type))
                {
                    additionalInfluence += book.additionalInfluence;
                }
            }
        }
        
        // 处理周围节点影响力
        foreach (GameObject go in nList)
        {
            NodeBehavior cub = go.GetComponent<NodeBehavior>();
            if (cub != null && cub.properties.state > 0)
            {
                types = cub.properties.GetBookType();
                foreach (var book in cub.properties.books)
                {
                    if (!book.isPreallocatedOut)
                    {
                        basicInfluence += book.basicInfluence;
                        if (types.Contains(book.type))
                        {
                            additionalInfluence += book.additionalInfluence;
                        }
                    }
                }
            }
        }

        StatePrediction prediction = new StatePrediction(Properties.StateEnum.DEAD, basicInfluence, additionalInfluence);
        switch (properties.state)
        {
            case Properties.StateEnum.NORMAL:
                prediction.state = Properties.StateEnum.NORMAL;
                if(basicInfluence + additionalInfluence >= properties.awakeThreshold) prediction.state = Properties.StateEnum.AWAKENED;
                if(basicInfluence >= properties.exposeThreshold) prediction.state = Properties.StateEnum.EXPOSED;
                break;
            case Properties.StateEnum.AWAKENED:
                prediction.state = Properties.StateEnum.AWAKENED;
                if(basicInfluence + additionalInfluence < properties.fallThreshold) prediction.state = Properties.StateEnum.NORMAL;
                if(basicInfluence >= properties.exposeThreshold) prediction.state = Properties.StateEnum.EXPOSED;
                break;
            case Properties.StateEnum.EXPOSED:
                prediction.state = Properties.StateEnum.EXPOSED;
                if(basicInfluence < properties.exposeThreshold) prediction.state = Properties.StateEnum.AWAKENED;
                if(basicInfluence + additionalInfluence< properties.fallThreshold) prediction.state = Properties.StateEnum.NORMAL;
                break;
        }

        return prediction;
    }

    public override void SetState(Properties.StateEnum stateEnum)
    {
        if (properties.state == Properties.StateEnum.NORMAL &&
            (stateEnum == Properties.StateEnum.AWAKENED || stateEnum == Properties.StateEnum.EXPOSED) &&
            !hadAwakenedBefore)
        {
            GameProcessManager GMinstance = GameProcessManager.instance;
            GMinstance.NodeAwakend(this.gameObject);

            plotAndPageHandler.OnAwakeShowButtons();
            hadAwakenedBefore = true;
            if(plotFileName != "null")
            {
                if (this is KeyNodeBehavior)
                {
                    PlotManager.instance.AddPlotQueue(plotFileName, gameObject);
                    Debug.Log($"{gameObject.name} 增加了一段focus剧情，聚焦于{gameObject.name}");
                }
                else
                {
                    PlotManager.instance.AddPlotQueue(plotFileName, null);
                    Debug.Log($"{gameObject.name} 增加了一段普通剧情");
                }
            }
        }

        if (stateEnum == Properties.StateEnum.NORMAL && (properties.state == Properties.StateEnum.AWAKENED ||
                                                         properties.state == Properties.StateEnum.EXPOSED))
        {
            plotAndPageHandler.OnFallHideButtons();
        }

        switch (stateEnum)
        {
            case Properties.StateEnum.NORMAL:
                if(properties.state != stateEnum) Instantiate(RoundManager.instance.DownFx,transform.position,Quaternion.identity);
                break;
            case Properties.StateEnum.AWAKENED:
                if(properties.state != stateEnum) Instantiate(RoundManager.instance.ActiveFx,transform.position,Quaternion.identity);
                break;
            case Properties.StateEnum.EXPOSED:
                Instantiate(RoundManager.instance.ExposeFx,transform.position,Quaternion.identity);
                break;
        }

        properties.state = stateEnum;
    }

    // 生成书签
    private void GenerateBookmarks()
    {
        Transform canvas = transform.Find("NodeUICanvas");
        ClearBookmarks();
        List<float> offsets = new List<float>();
        int count = properties.books.Count;
        bool isOdd = count % 2 == 1;
        
        if (isOdd) {
            int initial = - count / 2;
            for (int i = 0; i < count; ++i)
            {
                offsets.Add(initial);
                initial += 1;
            }
        } else {
            float initial = - count / 2 + 0.5f;
            for (int i = 0; i < count; ++i)
            {
                offsets.Add(initial);
                initial += 1;
            }
        }

        foreach (var book in properties.books) {
            // 实例化书签
            GameObject bookmarkObj = Instantiate(
                bookmarkPrefab,
                canvas.transform
            );
            bookmarkObj.transform.localPosition = new Vector3(offsets[properties.books.IndexOf(book)] * bookmarkSpacing, height, 0f);
            bookmarkObj.transform.localRotation = Quaternion.identity;
            bookmarkObj.transform.localScale = new Vector3(xScale, yScale, 1);
            // 配置书签
            bookmarkObj.GetComponent<BookMark>().ConfigureBookmark(book,CanvasBehavior.instance.GetNodeList().IndexOf(this.gameObject));
            spawnedBookmarks.Add(bookmarkObj);
        }
    }
    
    void ClearBookmarks() {
        foreach (var bm in spawnedBookmarks) {
            Destroy(bm.gameObject);
        }
        spawnedBookmarks.Clear();
    }
    
    public void AddABook(BookManager.Book book)
    {
        properties.books.Add(book);
        GenerateBookmarks();
    }
    
    public void RemoveABook(BookManager.Book book)
    {
        if (properties.books.Contains(book))
        {
            properties.books.Remove(book);
            GenerateBookmarks();
        }
        else
        {
            Debug.Log("not found");
        }
        
    }

    public void SetABooksState(BookManager.Book book,int pin,int pout)
    {
        //传入的两个参数取值为任意，规则是：如果为正，对应in/out字段设为true，为负则设为false，否则保持原状。
        if (pin != 0) book.isPreallocatedIn = pin > 0;
        if (pout != 0) book.isPreallocatedOut = pout > 0;
        //TODO
    }

    public List<GameObject> GetBookMarkList()
    {
        return this.spawnedBookmarks;
    }
}
