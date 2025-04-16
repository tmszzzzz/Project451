using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondElement : MonoBehaviour
{
    public string bookName;

    public void setGlobalBookName()
    {
        GlobalVar.instance.bookName = bookName;
    }
    
    public void cancelSetGlobalBookName()
    {
        GlobalVar.instance.bookName = "";
    }
}
