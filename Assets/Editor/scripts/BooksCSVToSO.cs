using UnityEngine;
using UnityEditor;
using System.IO;

public class BookDataImporter : MonoBehaviour
{
    [MenuItem("Tools/Import Books")]
    public static void ImportBooks()
    {
        string csvPath = Application.dataPath + "/Books/书表.csv";
        string[] lines = File.ReadAllLines(csvPath, System.Text.Encoding.GetEncoding("GB18030"));
        
        BooksData booksData = ScriptableObject.CreateInstance<BooksData>();

        // 跳过第一行（表头）
        for (int i = 1; i < lines.Length; i++)
        {
            string[] fields = lines[i].Split(',');

            BookManager.Book book = new BookManager.Book(
                int.Parse(fields[0]),
                fields[1],
                fields[2],
                int.Parse(fields[3]),
                int.Parse(fields[4]),
                _genBookType(fields[5]),
                false,
                false,
                0
            );
            
            booksData.books.Add(book);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("书表导入完成！");
    }

    private static BookManager.Book.BookType _genBookType(string type)
    {
        
    }
}
