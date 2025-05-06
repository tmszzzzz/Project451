using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class NodeUI : MonoBehaviour
{
    public NodeBehavior nb;
    [SerializeField] protected GameObject bookmarkPrefab; // 书签预制体
    private List<GameObject> spawnedBookmarks = new List<GameObject>();
    public float bookmarkSpacing = 1f;                      // 书签间距
    public float heightFromTheNode = 1.3f;                             // 书签高度
    public float width = 150f;
    public float height = 150f;
    // Start is called before the first frame update
    void Start()
    {
        if (nb.properties.state > 0)
        {
            // 生成书签
            GenerateBookmarks();
        }
    }
    
    // 生成书签
    public void GenerateBookmarks()
    {
        // 暴露值预测
        CanvasBehavior.instance.RefreshPreviewExposureValue();
        Transform canvas = transform.Find("NodeUICanvas");
        ClearBookmarks();
        List<float> offsets = new List<float>();
        int count = nb.properties.books.Count;
        bool isOdd = count % 2 == 1;
        
        if (isOdd) {
            int initial = - count / 2;
            for (int i = 0; i < count; ++i)
            {
                offsets.Add(initial);
                initial += 1;
            }
        } else {
            float initial = - count / 2 + 0.5f;
            for (int i = 0; i < count; ++i)
            {
                offsets.Add(initial);
                initial += 1;
            }
        }

        foreach (var book in nb.properties.books) {
            // 实例化书签
            GameObject bookmarkObj = Instantiate(
                bookmarkPrefab,
                canvas.transform
            );
            bookmarkObj.transform.localPosition = new Vector3(offsets[nb.properties.books.IndexOf(book)] * bookmarkSpacing, heightFromTheNode, 0f);
            bookmarkObj.transform.localRotation = Quaternion.identity;
            // bookmarkObj.transform.localScale = new Vector3(xScale, yScale, 1);
            bookmarkObj.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            // 配置书签
            bookmarkObj.GetComponent<BookMark>().ConfigureBookmark(book,CanvasBehavior.instance.GetNodeList().IndexOf(this.gameObject));
            spawnedBookmarks.Add(bookmarkObj);
        }
    }
    
    void ClearBookmarks() {
        foreach (var bm in spawnedBookmarks) {
            Destroy(bm.gameObject);
        }
        spawnedBookmarks.Clear();
    }
    
    public List<GameObject> GetBookMarkList()
    {
        return this.spawnedBookmarks;
    }
}
