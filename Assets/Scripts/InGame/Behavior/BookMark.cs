using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class BookMark : MonoBehaviour
{
    
    [System.Serializable]
    public class TypeConfig
    {
        public BookManager.Book.BookType type;
        public List<Material> influenceConfigs;
        public List<Sprite> influenceSpriteConfigs;
    }
    
    [SerializeField] private Image patternImage;
    [SerializeField] private TextMeshProUGUI bookNameText;
    
    [SerializeField] private List<TypeConfig> typeConfigurations;
    public int nodeIndex;
    public BookManager.Book book;
    public RoundManager.BookAllocationItem alloc;
    private BookManager.Book.BookType temp; // 高亮改好之后删掉
    private void Update()
    {
        // // 索引高亮
        // if (GlobalVar.instance.bookName == book.name)
        // {
        //     // 后面改成高亮
        //     book.type++;
        //     if (book.type == BookManager.Book.BookType.zhishi)
        //     {
        //         book.type--;
        //     }
        //     foreach (var v in typeConfigurations)
        //     {
        //         if (book.type == v.type)
        //         {
        //             if (book.basicInfluence > 3)
        //             {
        //                 Debug.LogWarning($"Book {book.name} has illegal influence: {book.basicInfluence}");
        //             }
        //             patternImage.material = v.influenceConfigs[book.basicInfluence - 1];
        //             break;
        //         }
        //     }
        // }
        // else
        // {
        //     book.type = temp;
        //     foreach (var v in typeConfigurations)
        //     {
        //         if (book.type == v.type)
        //         {
        //             if (book.basicInfluence > 3)
        //             {
        //                 Debug.LogWarning($"Book {book.name} has illegal influence: {book.basicInfluence}");
        //             }
        //             patternImage.material = v.influenceConfigs[book.basicInfluence - 1];
        //             break;
        //         }
        //     }
        // }
    }

    // 配置书签视觉和交互
    public void ConfigureBookmark(BookManager.Book associatedBook,int index)
    {
        book = associatedBook;
        temp = book.type;   // 高亮改好之后删掉
        nodeIndex = index;
        bool find = false;
        foreach (var v in typeConfigurations)
        {
            if (book.type == v.type)
            {
                find = true;
                if (book.basicInfluence > 3)
                {
                    Debug.LogWarning($"Book {book.name} has illegal influence: {book.basicInfluence}");
                }
                // patternImage.material = v.influenceConfigs[book.basicInfluence - 1];
                patternImage.sprite = v.influenceSpriteConfigs[book.basicInfluence - 1];
                // 分配出虚化效果
                if (book.isPreallocatedOut)
                {
                    patternImage.color = new Color(1, 1, 1, 0.36f);
                }
                break;
            }
        }if(!find) Debug.LogWarning($"No match type for {book.id}-{book.name}: {book.type}.");
        bookNameText.text = book.name;
    }

    public GameObject getParentNode()
    {
        return CanvasBehavior.instance.GetNodeList()[nodeIndex];
    }
    
}
