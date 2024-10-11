using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NodeBehavior : BaseNodeBehavior
{
    public Properties properties;
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
        ColorMap.Add(0, Color.green);
        ColorMap.Add(1, Color.yellow);
        ColorMap.Add(2, Color.red);
        mb = RoundManager.Instance.messageBar;
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

    

    public override StatePrediction RefreshState()
    {

        CanvasBehavior cb = transform.parent.GetComponent<CanvasBehavior>();
        if (cb == null)
        {
            Debug.LogWarning("Script \"CanvasBehavior\" not found in canvas.");
            return new StatePrediction(Properties.StateEnum.DEAD,0);
        }
        List<GameObject> nList = cb.GetNeighbors(gameObject);

        int influence = 0;
        foreach (GameObject go in nList)
        {
            NodeBehavior cub = go.GetComponent<NodeBehavior>();
            if (cub != null)
            {
                influence += cub.properties.state > 0 ? cub.properties.numOfBooks + 1 : 0;
            }
        }
        //Debug.Log(2);
        if (properties.state == Properties.StateEnum.DEAD) return new StatePrediction((Properties.StateEnum)Mathf.Max((int)properties.state, (int)Properties.StateEnum.DEAD),influence);
        else if (influence >= properties.exposeThreshold) return new StatePrediction(Properties.StateEnum.EXPOSED, influence);
        else if (influence >= properties.awakeThreshold) return new StatePrediction((Properties.StateEnum)Mathf.Max((int)properties.state, (int)Properties.StateEnum.AWAKENED), influence);
        else return new StatePrediction((Properties.StateEnum)Mathf.Max((int)properties.state, (int)Properties.StateEnum.NORMAL), influence);
    }
    public override void SetState(Properties.StateEnum stateEnum)
    {
        properties.state = stateEnum;
    }
}
