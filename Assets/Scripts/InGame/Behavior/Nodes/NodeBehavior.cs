using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NodeBehavior : BaseNodeBehavior
{
    public Properties properties;
    public PlotAndPageHandler plotAndPageHandler;
    public MessageBar mb;
    protected Renderer objRenderer;
    protected Dictionary<int, Color> ColorMap;
    [SerializeField]
    protected bool selected;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        objRenderer = GetComponent<MeshRenderer>();

        objRenderer.material = new Material(objRenderer.material);

        ColorMap = new Dictionary<int, Color>();
        ColorMap.Add(-1, Color.gray);
        ColorMap.Add(0, Color.gray);
        ColorMap.Add(1, Color.yellow);
        ColorMap.Add(2, Color.red);
        mb = RoundManager.Instance.messageBar;
        RoundManager.Instance.RoundChange += OnRoundChange;
    }
    protected virtual void Update()
    {
        objRenderer.material.color = ColorMap[(int)properties.state] + (selected?new Color(0.5f,0.5f,0.5f):new Color(0,0,0));
    }

    protected virtual void OnMouseEnter()
    {
        selected = true;
    }
    protected virtual void OnMouseExit()
    {
        selected = false;
    }

    protected virtual void OnRoundChange()
    {
        //if (properties.state == Properties.StateEnum.EXPOSED)
        //{
        //    GlobalVar.Instance.AddGlobalExposureValue(GlobalVar.Instance.exposureValueAdditionOfExposedNode);
        //}
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

        int influence = properties.numOfBooks + RoundManager.Instance.bookAllocationMap[gameObject];
        foreach (GameObject go in nList)
        {
            NodeBehavior cub = go.GetComponent<NodeBehavior>();
            if (cub != null)
            {
                influence += cub.properties.state > 0 ? cub.properties.numOfBooks  + RoundManager.Instance.bookAllocationMap[cub.gameObject] : 0;
            }
        }
        if (properties.state == Properties.StateEnum.EXPOSED) 
        {
            if (influence < properties.exposeThreshold) return new StatePrediction(Properties.StateEnum.AWAKENED, influence);
        }

        //Debug.Log(2);
        if (influence >= properties.exposeThreshold) return new StatePrediction(Properties.StateEnum.EXPOSED, influence);
        else if (influence >= properties.awakeThreshold) return new StatePrediction((Properties.StateEnum)(int)Properties.StateEnum.AWAKENED, influence);
        else return new StatePrediction((Properties.StateEnum)Mathf.Max((int)properties.state, (int)Properties.StateEnum.NORMAL), influence);
    }

    public override void SetState(Properties.StateEnum stateEnum)
    {
        if (properties.state == Properties.StateEnum.NORMAL && (stateEnum == Properties.StateEnum.AWAKENED || stateEnum == Properties.StateEnum.EXPOSED))
        {
            Debug.Log("Node " + gameObject.name + " is awaked.");
            plotAndPageHandler.OnAwakeShowButtons();
        }

        properties.state = stateEnum;
    }

    protected void OnDestroy()
    {
        RoundManager.Instance.RoundChange -= OnRoundChange;
    }
}
