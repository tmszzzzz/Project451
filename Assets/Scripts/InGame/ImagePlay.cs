using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ImagePlay : MonoBehaviour
{
    public List<Image> images;
    public float fadeDuration = 1.0f; // 淡入持续时间（秒）
    public AudioSource audioSource;
    private int currentImage = -1;
    private void Start()
    {
        for (int i = 0; i < images.Count; i++)
        {
            images[i].color = new Color(1, 1, 1, 0);
        }
    }
    
    public void ClickAndPlay()
    {
        currentImage++;
        if (ImagePlayController.instance.first)
        {
            ImagePlayController.instance.first = false;
            ImagePlayController.instance.firstText.gameObject.SetActive(false);
        }
        if (currentImage < images.Count)
        {
            StartCoroutine(FadeInImage(images[currentImage], fadeDuration));
            if (currentImage == images.Count - 1)
            {
                Invoke("ShowText", 1);
            }
        }
        else
        {
            gameObject.SetActive(false);
            ImagePlayController.instance.PlayNextPage();
        }
    }

    private void ShowText()
    {
        this.transform.GetChild(this.transform.childCount - 1).gameObject.SetActive(true);
    }
    
    private IEnumerator FadeInImage(Image image, float duration)
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play(); // 直接播放
        }
        else
        {
            Debug.LogWarning("No audio clip assigned");
        }
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
