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
    public Material litMaterial;
    public Sprite sprite;
    private void Start()
    {
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
                litMaterial = new Material(v.influenceConfigs[book.basicInfluence - 1]);
                sprite = v.influenceSpriteConfigs[book.basicInfluence - 1];
                patternImage.sprite = sprite;
                break;
            }
        }if(!find) Debug.LogWarning($"No match type for {book.id}-{book.name}: {book.type}.");
    }

    // 配置书签视觉和交互
    public void ConfigureBookmark(BookManager.Book associatedBook,int index)
    {
        book = associatedBook;
        nodeIndex = index;
        patternImage.sprite = this.sprite;
        if (book.isPreallocatedOut)
        {
            patternImage.color = new Color(1, 1, 1, 0.4f);
        }
        bookNameText.text = book.name;
    }

    public GameObject getParentNode()
    {
        return CanvasBehavior.instance.GetNodeList()[nodeIndex];
    }
    
}
