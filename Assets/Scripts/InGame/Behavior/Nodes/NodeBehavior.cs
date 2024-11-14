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
    // Start is called before the first frame update
    protected virtual void Start()
    {
        if(String.IsNullOrEmpty(plotFileName))plotFileName = "null";
        
        ColorMap = new Dictionary<int, Color>();
        ColorMap.Add(-1, Color.gray);
        ColorMap.Add(0, Color.gray);
        ColorMap.Add(1, Color.yellow);
        ColorMap.Add(2, Color.red);
        mb = MessageBar.instance;
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
        //修改此方法的判断逻辑时，请记得一并修改PredictState
        CanvasBehavior cb = transform.parent.GetComponent<CanvasBehavior>();
        if (cb == null)
        {
            Debug.LogWarning("Script \"CanvasBehavior\" not found in canvas.");
            return new StatePrediction(Properties.StateEnum.DEAD,0);
        }
        List<GameObject> nList = cb.GetNeighbors(gameObject);

        int influence = properties.numOfBooks;
        foreach (GameObject go in nList)
        {
            NodeBehavior cub = go.GetComponent<NodeBehavior>();
            if (cub != null)
            {
                influence += cub.properties.state > 0 ? cub.properties.numOfBooks : 0;
            }
        }
        return new StatePrediction(properties.state, influence);
    }

    public override StatePrediction PredictState()
    {
        //修改此方法的判断逻辑时，请记得一并修改NowState
        CanvasBehavior cb = transform.parent.GetComponent<CanvasBehavior>();
        if (cb == null)
        {
            Debug.LogWarning("Script \"CanvasBehavior\" not found in canvas.");
            return new StatePrediction(Properties.StateEnum.DEAD,0);
        }
        List<GameObject> nList = cb.GetNeighbors(gameObject);

        int influence = properties.numOfBooks + RoundManager.instance.BookAllocationMap[gameObject];
        foreach (GameObject go in nList)
        {
            NodeBehavior cub = go.GetComponent<NodeBehavior>();
            if (cub != null)
            {
                influence += cub.properties.state > 0 ? cub.properties.numOfBooks  + RoundManager.instance.BookAllocationMap[cub.gameObject] : 0;
            }
        }

        StatePrediction prediction = new StatePrediction(Properties.StateEnum.DEAD, influence);
        switch (properties.state)
        {
            case Properties.StateEnum.NORMAL:
                prediction.state = Properties.StateEnum.NORMAL;
                if(influence >= properties.awakeThreshold) prediction.state = Properties.StateEnum.AWAKENED;
                if(influence >= properties.exposeThreshold) prediction.state = Properties.StateEnum.EXPOSED;
                break;
            case Properties.StateEnum.AWAKENED:
                prediction.state = Properties.StateEnum.AWAKENED;
                if(influence < properties.fallThreshold) prediction.state = Properties.StateEnum.NORMAL;
                if(influence >= properties.exposeThreshold) prediction.state = Properties.StateEnum.EXPOSED;
                break;
            case Properties.StateEnum.EXPOSED:
                prediction.state = Properties.StateEnum.EXPOSED;
                if(influence < properties.exposeThreshold) prediction.state = Properties.StateEnum.AWAKENED;
                if(influence < properties.fallThreshold) prediction.state = Properties.StateEnum.NORMAL;
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

}
