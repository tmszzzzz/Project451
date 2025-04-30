using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ImagePlay : MonoBehaviour
{
    public List<Image> images;
    public float fadeDuration = 1.0f; // 淡入持续时间（秒）
    private int currentImage = 0;
    private void Start()
    {
        for (int i = 0; i < images.Count; i++)
        {
            images[i].color = new Color(1, 1, 1, 0);
        }
    }
    
    public void ClickAndPlay()
    {
        if (currentImage < images.Count)
        {
            StartCoroutine(FadeInImage(images[currentImage++], fadeDuration));
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    
    private IEnumerator FadeInImage(Image image, float duration)
    {
        float elapsedTime = 0f;
        Color startColor = image.color;
        Color targetColor = new Color(1, 1, 1, 1); // 完全不透明

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            image.color = Color.Lerp(startColor, targetColor, elapsedTime / duration);
            yield return null; // 等待下一帧
        }

        image.color = targetColor; // 确保最终完全显示
    }
}
