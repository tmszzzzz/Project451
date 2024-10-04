using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NodeBehavior : MonoBehaviour
{
    public Properties properties;
    private Renderer objRenderer;
    private Dictionary<int, Color> ColorMap;
    public bool selected;
    // Start is called before the first frame update
    void Start()
    {
        objRenderer = GetComponent<MeshRenderer>();

        objRenderer.material = new Material(objRenderer.material);

        ColorMap = new Dictionary<int, Color>();
        ColorMap.Add(-1, Color.gray);
        ColorMap.Add(0, Color.green);
        ColorMap.Add(1, Color.yellow);
        ColorMap.Add(2, Color.red);
    }

    //public void OnObjectClicked()
    //{
    //    if (properties.state != Properties.StateEnum.EXPOSED) properties.state++;
    //    else properties.state = Properties.StateEnum.DEAD;
    //}

    // Update is called once per frame
    void Update()
    {
        //objRenderer.material.color = selected ? Color.red : Color.white;
    }

    private void OnMouseDown()
    {
        RefreshState();
    }

    public Properties.StateEnum RefreshState()
    {

        CanvasBehavior cb = transform.parent.GetComponent<CanvasBehavior>();
        if (cb == null)
        {
            Debug.LogWarning("Script \"CanvasBehavior\" not found in canvas.");
            return Properties.StateEnum.DEAD;
        }
        List<GameObject> nList = cb.GetNeighbors(gameObject);

        foreach(GameObject nb in nList)
        {
            Debug.Log(nb.name);
        }

        int AwakeInfluence = 0;
        int ExposeInfluence = 0;
        foreach (GameObject go in nList)
        {
            NodeBehavior cub = go.GetComponent<NodeBehavior>();
            if (cub != null)
            {
                AwakeInfluence += cub.properties.state > 0 ? cub.properties.NumOfBooks + 1 : 0;
                ExposeInfluence += (cub.properties.state > 0 || cub.properties.state == Properties.StateEnum.DEAD) ? cub.properties.NumOfBooks + 1 : 0;
            }
        }
        //Debug.Log(2);
        if (properties.state == Properties.StateEnum.DEAD) return (Properties.StateEnum)Mathf.Max((int)properties.state,(int)Properties.StateEnum.DEAD);
        else if (ExposeInfluence >= properties.exposeThreshold) return (Properties.StateEnum)Mathf.Max((int)properties.state, (int)Properties.StateEnum.EXPOSED);
        else if (AwakeInfluence >= properties.awakeThreshold) return (Properties.StateEnum)Mathf.Max((int)properties.state, (int)Properties.StateEnum.AWAKENED);
        else return (Properties.StateEnum)Mathf.Max((int)properties.state, (int)Properties.StateEnum.NORMAL);
        //真的要让状态不可逆吗？此处存疑
    }
    //public void SetState(Properties.StateEnum stateEnum)
    //{
    //    properties.state = stateEnum;
    //}
}
