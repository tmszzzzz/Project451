using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class BookMark : MonoBehaviour
{
    [SerializeField] private Image colorImage;
    [SerializeField] private Image patternImage;
    [SerializeField] private TextMeshProUGUI bookNameText;
    [SerializeField] public float riseHeight = 1f;    // 上升高度
    public float moveDuration = 0.2f;  // 动画时长

    private Vector3 originalPosition;
    private Coroutine currentAnimation;
    private bool areMouseEventsBlocked;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E))
        {
            areMouseEventsBlocked = true;
            if (currentAnimation != null)
            {
                StopCoroutine(currentAnimation);
                currentAnimation = null;
            }
            transform.position = originalPosition;
            bookNameText.gameObject.SetActive(false);
        }
        areMouseEventsBlocked = false;
    }

    // 配置书签视觉和交互
    public void ConfigureBookmark(BookManager.Book book)
    {
        originalPosition = transform.position;
        // 设置基础颜色
        if (book.type == BookManager.Book.BookType.StonemasonChisel)
        {
            colorImage.color = Color.red;
        }else if (book.type == BookManager.Book.BookType.TravelerFlint)
        {
            colorImage.color = Color.blue;
        }
        // 设置花纹
        if (book.additionalInfluence > 1)
        {
            // patternImage.sprite = ;
        }
        bookNameText.text = book.name;
    }
    
    public void OnPointerEnterPattern()
    {
        areMouseEventsBlocked = true;
        bookNameText.gameObject.SetActive(true);
        // 停止之前的动画（避免冲突）
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }

        Vector3 targetPos = originalPosition + Camera.main.transform.up * riseHeight;
        currentAnimation = StartCoroutine(MoveBookmark(targetPos));
    }
    
    public void OnPointerExitPattern()
    {
        if (areMouseEventsBlocked) return;
        bookNameText.gameObject.SetActive(false);
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        currentAnimation = StartCoroutine(MoveBookmark(originalPosition));
    }
    
    // 插值移动协程
    private IEnumerator MoveBookmark(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;

        while (elapsedTime < moveDuration)
        {
            // 计算插值进度（0~1）
            float t = elapsedTime / moveDuration;
            // 应用缓动曲线（可选）
            t = Mathf.SmoothStep(0, 1, t);

            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
