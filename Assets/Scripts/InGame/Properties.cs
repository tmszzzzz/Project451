using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using BookInfo = BookManager.BookInfo;

[System.Serializable]
public class Properties
{

    public enum StateEnum
    {
        DEAD=-1, NORMAL=0, AWAKENED=1, EXPOSED=2
    }
    public StateEnum state;
    //public int influence;
    public enum typeEnum
    {
        NORMAL = 0, FIREFIGHTER = 1, BIBLIOFHILE = 2, KEYNODE = 3
    }
    public typeEnum type;
    public int awakeThreshold;
    public int exposeThreshold;
    public int fallThreshold;
    public List<BookInfo> books = new List<BookInfo>();
    public int numOfBooks = 0;
    public int maximumNumOfBooks;
    public int unlockTag;
    public int region;

    public string typeNameToCNString(typeEnum inputType)
    {
        switch (inputType)
        {
            case typeEnum.NORMAL:
                return "普通";
            case typeEnum.FIREFIGHTER:
                return "消防员";
            case typeEnum.BIBLIOFHILE:
                return "藏书者";
            case typeEnum.KEYNODE:
                return "关键人物";
            default:
                return "未知";
        }
    }

    public string stateNameToCNString(StateEnum inputState)
    {
        switch (inputState)
        {
            case StateEnum.DEAD:
                return "死亡";
            case StateEnum.NORMAL:
                return "未加入";
            case StateEnum.AWAKENED:
                return "已加入";
            case StateEnum.EXPOSED:
                return "已暴露";
            default:
                return "未知";
        }
    }
    //public string description = "";
    //public string plotFileName = "";
    //public Sprite pageSprite;
}
