using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PropertiesEditor
{
   
    public enum typeEnum
    {
        NORMAL=0, FIREFIGHTER=1, BIBLIOFHILE=2
    }
    public typeEnum type;
    //public int influence;
    public int awakeThreshold;
    public int exposeThreshold;
    public int maximumNumOfBooks;
}
