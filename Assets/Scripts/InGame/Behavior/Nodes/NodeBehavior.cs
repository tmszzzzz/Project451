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

    public override Properties.StateEnum RefreshState()
    {

        CanvasBehavior cb = transform.parent.GetComponent<CanvasBehavior>();
        if (cb == null)
        {
            Debug.LogWarning("Script \"CanvasBehavior\" not found in canvas.");
            return Properties.StateEnum.DEAD;
        }
        List<GameObject> nList = cb.GetNeighbors(gameObject);

        int AwakeInfluence = 0;
        int ExposeInfluence = 0;
        foreach (GameObject go in nList)
        {
            NodeBehavior cub = go.GetComponent<NodeBehavior>();
            if (cub != null)
            {
                AwakeInfluence += cub.properties.state > 0 ? cub.properties.numOfBooks + 1 : 0;
                ExposeInfluence += (cub.properties.state > 0 || cub.properties.state == Properties.StateEnum.DEAD) ? cub.properties.numOfBooks + 1 : 0;
            }
        }
        //Debug.Log(2);
        if (properties.state == Properties.StateEnum.DEAD) return (Properties.StateEnum)Mathf.Max((int)properties.state, (int)Properties.StateEnum.DEAD);
        else if (ExposeInfluence >= properties.exposeThreshold)
        {
            float v = Random.value;
            if (properties.state == Properties.StateEnum.EXPOSED && v < GlobalVar.Instance.exposeToDeathRate)
                return Properties.StateEnum.DEAD;
            else return Properties.StateEnum.EXPOSED;
        }
        else if (AwakeInfluence >= properties.awakeThreshold) return (Properties.StateEnum)Mathf.Max((int)properties.state, (int)Properties.StateEnum.AWAKENED);
        else return (Properties.StateEnum)Mathf.Max((int)properties.state, (int)Properties.StateEnum.NORMAL);
        //真的要让状态不可逆吗？此处存疑
    }
    public override void SetState(Properties.StateEnum stateEnum)
    {
        properties.state = stateEnum;
    }
}
