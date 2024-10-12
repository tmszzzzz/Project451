using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisgustingRealTimeUpdateNodeBehavior : MonoBehaviour
{
    //Don't use this script once more sophisticated solution is found
    public NodeBehavior nodeBehavior;
    public RoundManager roundManager;
    public CanvasBehavior canvasBehavior;
    public GameObject thisNode;
    private BaseNodeBehavior.StatePrediction statePrediction;

    void Start()
    {
        nodeBehavior = thisNode.GetComponent<NodeBehavior>();
        roundManager = RoundManager.Instance;
        canvasBehavior = roundManager.canvas;
    }

    void DebugCheck() 
    {
        if (thisNode == null) 
        {
            Debug.LogWarning("thisNode is null");
        }

        if (roundManager == null)
        {
            Debug.LogWarning("roundManager is null");
        }

        if (nodeBehavior == null)
        {
            Debug.LogWarning("nodeBehavior is null");
        }

        if (canvasBehavior == null)
        {
            Debug.LogWarning("canvasBehavior is null");
        }
    }

    // Update is called once per frame
    public BaseNodeBehavior.StatePrediction RealTimeRefreshState() 
    {
        DebugCheck();

        statePrediction = nodeBehavior.RefreshState();

        int influence = statePrediction.influence;
        Properties properties = nodeBehavior.properties;

        foreach (GameObject neighborNode in canvasBehavior.GetNeighbors(thisNode))
        {
            influence += roundManager.bookAllocationMap[neighborNode];
        }

        if (properties.state == Properties.StateEnum.DEAD) return new BaseNodeBehavior.StatePrediction((Properties.StateEnum)Mathf.Max((int)properties.state, (int)Properties.StateEnum.DEAD),influence);
        else if (influence >= properties.exposeThreshold) return new BaseNodeBehavior.StatePrediction(Properties.StateEnum.EXPOSED, influence);
        else if (influence >= properties.awakeThreshold) return new BaseNodeBehavior.StatePrediction((Properties.StateEnum)Mathf.Max((int)properties.state, (int)Properties.StateEnum.AWAKENED), influence);
        else return new BaseNodeBehavior.StatePrediction((Properties.StateEnum)Mathf.Max((int)properties.state, (int)Properties.StateEnum.NORMAL), influence);
    }
}
