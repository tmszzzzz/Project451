using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

public class PlotFuncManager : MonoBehaviour
{
    public ToggleBookButton Button;
    public static PlotFuncManager instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void test()
    {
        Debug.Log("Function test called.");
    }

    public void lostABook()
    {
        var l = RoundManager.instance.canvas.GetNodeList();
        var bookL = new List<GameObject>();
        foreach (var node in l)
        {
            if(node.GetComponent<NodeBehavior>().properties.numOfBooks > 0) bookL.Add(node);
        }

        System.Random random = new System.Random();
        if (l.Count != 0)
        {
            int randomIndex = random.Next(bookL.Count);
            RoundManager.instance.canvas.AddNodeNumOfBooks(bookL[randomIndex],-1);
            MessageBar.instance.AddMessage($"{bookL[randomIndex].gameObject.name}损失了一本书.");
        }
    }

    public void gainHugeAmountOfExposureValue()
    {
        GlobalVar.instance.AddGlobalExposureValue(50);
        MessageBar.instance.AddMessage($"你的暴露值大幅增长.");
    }

    public void openTaskFromDario()
    {
        QuestPanel.instance.AddQuest("Deal");
    }

    public void openSceneOfOffice()
    {
        QuestPanel.instance.AddQuest("Office");
    }

    public void openSceneOfPoliceStation()
    {
        QuestPanel.instance.AddQuest("Police");
    }

    public void openSceneOfFirehouse()
    {
        QuestPanel.instance.AddQuest("FireHouse");
    }

    public void openQuest0()
    {
        QuestPanel.instance.AddQuest("Zero");
        Button.ToggleBook();   
    }

    public void gainTwoNewMembers()
    {
        CanvasBehavior cb = RoundManager.instance.canvas;
        var l = cb.GetNodeList();
        var joinedL = new List<GameObject>();
        foreach (var node in l)
        {
            if (node.GetComponent<NodeBehavior>().properties.state >= Properties.StateEnum.AWAKENED)
            {
                joinedL.Add(node);
            }
        }

        var nbL = new List<GameObject>();
        foreach (var node in joinedL)
        {
            var nodeNb = cb.GetNeighbors(node);
            foreach (var nbs in nodeNb)
            {
                if (nbs.GetComponent<NodeBehavior>().properties.state <= Properties.StateEnum.NORMAL)
                {
                    nbL.Add(nbs);
                }
            }
        }

        System.Random random = new Random();
        for (int i = 0; i < 4; i++)
        {
            if (nbL.Count <= 0)
            {
                break;
            }
            int v = random.Next(nbL.Count);
            nbL[v].GetComponent<NodeBehavior>().SetState(Properties.StateEnum.AWAKENED);
            MessageBar.instance.AddMessage($"{nbL[v].gameObject.name}加入了.");
            nbL.Remove(nbL[v]);
        }
    }

    public void gainOneResourcePoint()
    {
        GlobalVar.instance.resourcePoint++;
        MessageBar.instance.AddMessage("你获得了一点情报点.");
    }

    public void increasePunishment()
    {
        GlobalVar.instance.exposureValueAdditionOfDetective += 3;
        MessageBar.instance.AddMessage("被看火者侦察的惩罚增加了.");
    }

    public void reduceTwoAllocationLimit()
    {
        GlobalVar.instance.allocationLimit = Mathf.Max(GlobalVar.instance.allocationLimit-2,1);
        MessageBar.instance.AddMessage("书的分配流转上限降低了.");
    }

    public void increseAmountOfBooksFromFireFighter()
    {
        GlobalVar.instance.numOfFirefighterGiveBooks *= 2;
        MessageBar.instance.AddMessage("消防员提供的书数量翻倍.");
    }


}
