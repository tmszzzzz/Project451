using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class BookDataImporter : MonoBehaviour
{
    [MenuItem("Tools/Import Books")]
    public static void ImportBooks()
    {
        string csvPath = Application.dataPath + "/Resources/Books/书表.csv";
        string[] lines = File.ReadAllLines(csvPath);
        BooksData booksData = ScriptableObject.CreateInstance<BooksData>();
        booksData.books = new List<BookManager.Book>();

        // 跳过第一行（表头）
        for (int i = 1; i < lines.Length; i++)
        {
            string[] fields = lines[i].Split(',');
            if (string.IsNullOrEmpty(lines[i]) || fields[1] == "") continue;
            BookManager.Book book = new BookManager.Book(
                int.Parse(fields[0]),
                fields[1],
                fields[2],
                int.Parse(fields[3]),
                int.Parse(fields[4]),
                BookManager.Book.ParseCnToBookType(fields[5]),
                false,
                false,
                0
            );
            
            
            booksData.books.Add(book);
        }
        
        string assetPath = "Assets/Resources/Books/AllBooksData.asset";
        AssetDatabase.CreateAsset(booksData, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("书表导入完成！");
    }

}
