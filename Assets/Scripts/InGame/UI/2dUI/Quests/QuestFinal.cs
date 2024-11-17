using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestFinal : QuestUnit
{
    private List<NodeBehavior> _nodesList = new List<NodeBehavior>();
    private void Start()
    {
        foreach (GameObject nodeGameObject in RoundManager.instance.canvas.GetNodeList())
        {
            _nodesList.Add(nodeGameObject.GetComponent<NodeBehavior>());
        }
    }

    public override bool CheckIfQuestIsFinished()
    {
        foreach (NodeBehavior node in _nodesList)
        {
            if (node.properties.state < Properties.StateEnum.AWAKENED)
            {
                return false;
            }
        }

        return true;
    }
    
    private int NumOfCurrentAwakenedNodes()
    {
        int finished = 0;
        foreach (NodeBehavior node in _nodesList)
        {
            if (node.properties.state >= Properties.StateEnum.AWAKENED)
            {
                finished++;
            }
        }
        return finished;
    }

    public override string UpdateDescription()
    {
        return "使本区域全部（"+ NumOfCurrentAwakenedNodes() +"/"+ _nodesList.Count + "）个人加入";
    }
    
    [SerializeField] private string winGamePlotPath = "Assets/Resources/Plots/winGameScene.txt";
    
    public override void Update()
    {
        base.Update();

        if (isQuestCompleted)
        {
            PlotManager.instance.AddPlotQueue(winGamePlotPath, RoundManager.instance.canvas.Me);
        }
    }
}