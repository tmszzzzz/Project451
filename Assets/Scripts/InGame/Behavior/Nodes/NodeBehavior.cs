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
    [SerializeField] protected bool hadAwakenedBefore = false;

    public string description = "";
    public string plotFileName = "";
    public Sprite pageSprite;
    // Start is called before the first frame update
    protected virtual void Start()
    {

        ColorMap = new Dictionary<int, Color>();
        ColorMap.Add(-1, Color.gray);
        ColorMap.Add(0, Color.gray);
        ColorMap.Add(1, Color.yellow);
        ColorMap.Add(2, Color.red);
        mb = RoundManager.Instance.messageBar;
    }
    protected virtual void Update()
    {
        objColor.color = ColorMap[(int)properties.state];
    }

    public override StatePrediction PredictState()
    {

        CanvasBehavior cb = transform.parent.GetComponent<CanvasBehavior>();
        if (cb == null)
        {
            Debug.LogWarning("Script \"CanvasBehavior\" not found in canvas.");
            return new StatePrediction(Properties.StateEnum.DEAD,0);
        }
        List<GameObject> nList = cb.GetNeighbors(gameObject);

        int influence = properties.numOfBooks + RoundManager.Instance.BookAllocationMap[gameObject];
        foreach (GameObject go in nList)
        {
            NodeBehavior cub = go.GetComponent<NodeBehavior>();
            if (cub != null)
            {
                influence += cub.properties.state > 0 ? cub.properties.numOfBooks  + RoundManager.Instance.BookAllocationMap[cub.gameObject] : 0;
            }
        }
        if (properties.state == Properties.StateEnum.EXPOSED) 
        {
            if (influence < properties.exposeThreshold) return new StatePrediction(Properties.StateEnum.AWAKENED, influence);
        }

        if (influence >= properties.exposeThreshold) return new StatePrediction(Properties.StateEnum.EXPOSED, influence);
        else if (influence >= properties.awakeThreshold) return new StatePrediction((Properties.StateEnum)(int)Properties.StateEnum.AWAKENED, influence);
        else if (influence < properties.fallThreshold) return new StatePrediction((Properties.StateEnum)(int)Properties.StateEnum.NORMAL, influence);
        else return new StatePrediction((Properties.StateEnum)Mathf.Max((int)properties.state, (int)Properties.StateEnum.NORMAL), influence);
    }

    public override void SetState(Properties.StateEnum stateEnum)
    {
        if (properties.state == Properties.StateEnum.NORMAL && (stateEnum == Properties.StateEnum.AWAKENED || stateEnum == Properties.StateEnum.EXPOSED) && !hadAwakenedBefore)
        {
            plotAndPageHandler.OnAwakeShowButtons();
            hadAwakenedBefore = true;
        }
        if (stateEnum == Properties.StateEnum.NORMAL && (properties.state == Properties.StateEnum.AWAKENED || properties.state == Properties.StateEnum.EXPOSED))
        {
            plotAndPageHandler.OnFallHideButtons();
        }
        properties.state = stateEnum;
    }

}
