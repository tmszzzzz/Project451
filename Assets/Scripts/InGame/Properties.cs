using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public List<BookManager.Book> books = new List<BookManager.Book>();
    public int unlockTag;
    public int region;
    public List<BookManager.Book> borrowBooks = new List<BookManager.Book>();
    
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
    
    public List<BookManager.Book.BookType> GetBookType()
    {
        HashSet<BookManager.Book.BookType> types = new HashSet<BookManager.Book.BookType>();
        foreach (BookManager.Book book in borrowBooks)
        {
            types.Add(book.type); // 重复的不会添加
        }
        return types.ToList();
    }

    public void AddABook(BookManager.Book book)
    {
        books.Add(book);
        //TODO
    }
    
    public void RemoveABook(BookManager.Book book)
    {
        books.Remove(book);
        //TODO
    }

    public void SetABooksState(BookManager.Book book,int pin,int pout)
    {
        //传入的两个参数取值为任意，规则是：如果为正，对应in/out字段设为true，为负则设为false，否则保持原状。
        if (pin != 0) book.isPreallocatedIn = pin > 0;
        if (pout != 0) book.isPreallocatedOut = pout > 0;
        //TODO
    }
}
