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
    public Slider slider;
    public Material sliderMaterial;
    public NodeUI nodeUI;
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
    }

    protected virtual void Update()
    {
        sliderMaterial = slider.GetComponent<InfluenceBarBehavior>()._material;
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
                if (basicInfluence >= properties.exposeThreshold) prediction.state = Properties.StateEnum.EXPOSED;
                break;
            case Properties.StateEnum.AWAKENED:
                prediction.state = Properties.StateEnum.AWAKENED;
                if(basicInfluence + additionalInfluence < properties.fallThreshold) prediction.state = Properties.StateEnum.NORMAL;
                if (basicInfluence >= properties.exposeThreshold) prediction.state = Properties.StateEnum.EXPOSED;
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
                if (GlobalVar.instance.allowPlot)
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
                else
                {
                    if (this is KeyNodeBehavior)
                    {
                        StartCoroutine("WaitPlot1");
                    }
                    else
                    {
                        StartCoroutine("WaitPlot2");
                    }
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

    private IEnumerator WaitPlot1()
    {
        while (!GlobalVar.instance.allowPlot)
        {
            yield return null; // 等待一帧
        }
        PlotManager.instance.AddPlotQueue(plotFileName, gameObject);
    }
    private IEnumerator WaitPlot2()
    {
        while (!GlobalVar.instance.allowPlot)
        {
            yield return null; // 等待一帧
        }
        PlotManager.instance.AddPlotQueue(plotFileName, null);
    }
    
    public void AddABook(BookManager.Book book)
    {
        properties.books.Add(book);
        nodeUI.GenerateBookmarks();
    }
    
    public void RemoveABook(BookManager.Book book)
    {
        if (properties.books.Contains(book))
        {
            properties.books.Remove(book);
            nodeUI.GenerateBookmarks();
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
    }

    public int GetExposureValue()
    {
        int overInfluence = Math.Max(this.NowState().basicInfluence - this.properties.exposeThreshold, 0);
        int pow = overInfluence * 2 / this.properties.exposeThreshold;
        int result = IntPow(GlobalVar.instance.exposureValueAdditionOfExposedNode, pow);
        if (GlobalVar.instance.exposureCoefficient == 0.8f)
        {
            return result * 4 / 5 + 4;
        }
        if (GlobalVar.instance.exposureCoefficient == 1.2f)
        {
            return result * 6 / 5 + 4;
        }
        return result + 4;
    }
    
    public static int IntPow(int x, int pow)
    {
        if (pow < 0)
        {
            Debug.LogWarning("参数错误");
            return -1;
        }
        int result = 1;
        while (pow > 0)
        {
            if ((pow & 1) == 1) result *= x;
            x *= x;
            pow >>= 1;
        }
        return result;
    }
}
