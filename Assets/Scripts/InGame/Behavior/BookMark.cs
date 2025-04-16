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
    }
    
    [SerializeField] private Image patternImage;
    [SerializeField] private TextMeshProUGUI bookNameText;
    
    [SerializeField] private List<TypeConfig> typeConfigurations;
    public int nodeIndex;
    public BookManager.Book book;
    // 配置书签视觉和交互
    public void ConfigureBookmark(BookManager.Book associatedBook,int index)
    {
        book = associatedBook;
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
                patternImage.material = v.influenceConfigs[book.basicInfluence - 1];
                break;
            }
        }if(!find) Debug.LogWarning($"No match type for {book.id}-{book.name}: {book.type}.");
        bookNameText.text = book.name;
    }
    
    public void OnPointerEnterPattern()
    {
        bookNameText.gameObject.SetActive(true);
    }
    
    public void OnPointerExitPattern()
    {
        bookNameText.gameObject.SetActive(false);
    }

    public GameObject getParentNode()
    {
        return CanvasBehavior.instance.GetNodeList()[nodeIndex];
    }
}
