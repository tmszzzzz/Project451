using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestOfDeal : QuestUnit
{
    private List<NodeBehavior> keyNodesInPoliceStation = new List<NodeBehavior>();

    [SerializeField] private int turnsForCompletion = 30;
    
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

        if(GlobalVar.instance.dealStartRound == 0) GlobalVar.instance.dealStartRound = GlobalVar.instance.roundNum;
    }


    public override bool CheckIfQuestIsFinished()
    {
        if (AllKeyNodesInRegionTransformed())
            return true;
        
        if (GlobalVar.instance.roundNum - GlobalVar.instance.dealStartRound <= turnsForCompletion)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    bool AllKeyNodesInRegionTransformed()
    {
        foreach (NodeBehavior node in keyNodesInPoliceStation)
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
        return "赌约还剩" + (turnsForCompletion - GlobalVar.instance.roundNum + GlobalVar.instance.dealStartRound) + "天";
    }

    public string winDealPlotPath;
    public string loseDealPlotPath;

    public override void ActionsWhenQuestIsCompleted()
    {
        if (AllKeyNodesInRegionTransformed())
        {
            PlotManager.instance.AddPlotQueue(winDealPlotPath, RoundManager.instance.canvas.Me);
        }
        else
        {
            PlotManager.instance.AddPlotQueue(loseDealPlotPath, RoundManager.instance.canvas.Me);
        }
    }
}
