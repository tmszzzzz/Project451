using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestOfDeal : QuestUnit
{
    private List<NodeBehavior> keyNodesInPoliceStation;

    private int startTurn;
    [SerializeField] private 
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject node in RoundManager.instance.canvas.GetNodeList())
        {
            NodeBehavior thisnode = node.GetComponent<NodeBehavior>();
            if (thisnode.properties.type == Properties.typeEnum.KEYNODE && thisnode.properties.region == 1)
            {
                keyNodesInPoliceStation.Add(thisnode);
            }
        }

        startTurn = RoundManager.instance.roundNum;
    }


    public override bool CheckIfQuestIsFinished()
    {
        return false;
    }
}
