using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class BookMark : MonoBehaviour
{
    [System.Serializable]
    public class InfluenceVisualConfig
    {
        public Sprite patternSprite;  // 花纹贴图
    }
    
    [System.Serializable]
    public class TypeConfig
    {
        public BookManager.Book.BookType type;
        public InfluenceVisualConfig[] influenceConfigs;
    }
    
    [SerializeField] private Image patternImage;
    [SerializeField] private TextMeshProUGUI bookNameText;
    
    [SerializeField] private TypeConfig[] typeConfigurations;
    
    
    // 配置书签视觉和交互
    public void ConfigureBookmark(BookManager.Book book)
    {
        foreach (var v in typeConfigurations)
        {
            if (book.type == v.type)
            {
                patternImage.sprite = v.influenceConfigs[book.basicInfluence - 1].patternSprite;
                break;
            }
        }
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

}
