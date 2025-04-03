using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BooksData", menuName = "Custom/BooksData")]
public class BooksData : ScriptableObject
{
    public List<BookManager.Book> books;
}
