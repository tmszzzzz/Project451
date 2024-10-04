using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CubeEditorBehavior : MonoBehaviour
{
    public PropertiesEditor properties;
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
        objRenderer.material.color = selected ? Color.red : Color.white;
    }

    //public Properties.StateEnum RefreshState()
    //{
        /*
        CanvasEditorBehavior cb = transform.parent.GetComponent<CanvasEditorBehavior>();
        if(cb == null)
        {
            Debug.LogWarning("Script \"CanvasBehavior\" not found in canvas.");
            return Properties.StateEnum.DEAD;
        }
        List<GameObject> nList = cb.GetNeighbors(gameObject);
        int Influence = 0;
        foreach(GameObject go in nList)
        {
            CubeEditorBehavior cub = go.GetComponent<CubeEditorBehavior>();
            if (cub != null) Influence += cub.properties.state > 0 ? cub.properties.influence : 0;
            if (cub != null) Influence -= cub.properties.state < 0 ? 2 * cub.properties.influence : 0;
        }
        //Debug.Log(2);
        if (properties.state == Properties.StateEnum.DEAD) return Properties.StateEnum.DEAD;
        else if (Influence >= properties.exposeThreshold) return Properties.StateEnum.EXPOSED;
        else if (properties.state == Properties.StateEnum.NORMAL && Influence >= properties.awakeThreshold) return Properties.StateEnum.AWAKENED;
        else if (properties.state >= Properties.StateEnum.AWAKENED && Influence <= properties.supressThreshold) return Properties.StateEnum.NORMAL; 
        else return (int)properties.state > 1 ? properties.state - 1 : properties.state;
        */
    //}
    //public void SetState(Properties.StateEnum stateEnum)
    //{
    //    properties.state = stateEnum;
    //}
}
