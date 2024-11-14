using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestToFindLeon : QuestUnit
{
    private NodeBehavior LNode;

    private void Start()
    {
        LNode = RoundManager.instance.canvas.GetNodeList()[3].GetComponent<KeyNodeBehavior>();
    }

    public override bool CheckIfQuestIsFinished()
    {
        return LNode.properties.state >= Properties.StateEnum.AWAKENED ? true : false;
    }

    public override string UpdateDescription()
    {
        return "找到（"+ (LNode.properties.state >= Properties.StateEnum.AWAKENED ? 1 : 0) +"/1）名关键人物";
    }
}