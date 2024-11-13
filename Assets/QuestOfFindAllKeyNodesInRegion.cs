using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestOfFindAllKeyNodesInRegion : QuestUnit
{
    [SerializeField] private int targetRegion;
    private string regionName;
    private List<NodeBehavior> keyNodesInThisRegion = new List<NodeBehavior>();
    
    // Start is called before the first frame update
    void Awake()
    {
        if (targetRegion == 0)
        {
            regionName = "广播中心";

        }
        else if (targetRegion == 1)
        {
            regionName = "警察局";
        }
        else if (targetRegion == 2)
        {
            regionName = "消防局";
        }

        foreach (GameObject node in RoundManager.instance.canvas.GetNodeList())
        {
            NodeBehavior thisnode = node.GetComponent<NodeBehavior>();
            if (thisnode.properties.type == Properties.typeEnum.KEYNODE && thisnode.properties.region == targetRegion)
            {
                keyNodesInThisRegion.Add(thisnode);
            }
                
        }
    }

    public override bool CheckIfQuestIsFinished()
    {
        foreach (NodeBehavior node in keyNodesInThisRegion)
        {
            if (node.properties.state < Properties.StateEnum.AWAKENED)
            {
                return false;
            }
        }

        return true;
    }

    public override string UpdateDescription()
    {
        int finished = 0;
        foreach (NodeBehavior node in keyNodesInThisRegion)
        {
            if (node.properties.state >= Properties.StateEnum.AWAKENED)
            {
                finished++;
            }
        }

        return "找到"+ regionName +"内（"+ finished +"/"+ keyNodesInThisRegion.Count + "）名关键人物";
    }
}