using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public int numOfBooks = 0;
    public int maximumNumOfBooks;
    public int unlockTag;
    public int region;
    //public string description = "";
    //public string plotFileName = "";
    //public Sprite pageSprite;
}
