using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
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
    }

    public void test()
    {
        Debug.Log("Function test called.");
    }

    public void loseTheGame()
    {
        GameLoader.instance.SaveDataForEndScene(false);
        StartCoroutine(Camera.main.GetComponent<esc>().LoadSceneCoroutine(2));
    }

    public void winTheGame()
    {
        GameLoader.instance.SaveDataForEndScene(true);
        StartCoroutine(Camera.main.GetComponent<esc>().LoadSceneCoroutine(2));
    }

    public void emergencyCode()
    {
        GlobalVar.instance.globalExposureValue = GlobalVar.instance.maxGlobalExposureValue / 2;
        MessageBar.instance.AddMessage("他们的牺牲使你获得了第二次机会。");
    }

    public void lostABook()
    {
        var l = RoundManager.instance.canvas.GetNodeList();
        var bookL = new List<GameObject>();
        foreach (var node in l)
        {
            if(node.GetComponent<NodeBehavior>().properties.books.Count > 0) bookL.Add(node);
        }

        System.Random random = new System.Random();
        if (l.Count != 0)
        {
            int randomIndex = random.Next(bookL.Count);
            RoundManager.instance.canvas.AddNodeNumOfBooks(bookL[randomIndex],-1);
            MessageBar.instance.AddMessage($"{NameManager.instance.ConvertNodeNameToName(bookL[randomIndex].gameObject.name)}损失了一本书.");
        }
    }

    public void gainHugeAmountOfExposureValue()
    {
        int origin = GlobalVar.instance.globalExposureValue;
        GlobalVar.instance.AddGlobalExposureValue(30);
        if (GlobalVar.instance.globalExposureValue > 80) GlobalVar.instance.globalExposureValue = Mathf.Max(origin, 80);
        MessageBar.instance.AddMessage($"你的暴露值大幅增长.");
    }

    public void openTaskFromDario()
    {
        QuestPanel.instance.AddQuest("Deal");
    }

    public void openSceneOfOffice()
    {
        RoundManager.instance.switching1 = true;
        QuestPanel.instance.AddQuest("Office");
    }

    public void openSceneOfPoliceStation()
    {
        RoundManager.instance.switching2 = true;
        QuestPanel.instance.AddQuest("Police");
    }

    public void openSceneOfFirehouse()
    {
        RoundManager.instance.switching3 = true;
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
                if (nbs.GetComponent<NodeBehavior>().properties.state <= Properties.StateEnum.NORMAL && !nbL.Contains(nbs))
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
            MessageBar.instance.AddMessage($"{NameManager.instance.ConvertNodeNameToName(nbL[v].gameObject.name)}加入了.");
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

    public void OpenFinalQuest()
    {
        QuestPanel.instance.AddQuest("Final");
    }

    public void bgmOff()
    {
        BackgroundMusicManager.instance.switchTo(-1);
    }

    public void winDealWithD()
    {
        GlobalVar.instance.resourcePoint+=3;
        MessageBar.instance.AddMessage("你获得了三点情报点.");
    }
    
    public void loseDealWithD()
    {
        MessageBar.instance.AddMessage("赌约失败了.");
    }
}
