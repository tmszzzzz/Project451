using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class TextFader : MonoBehaviour
{
    public float fadeTime = 2;
    private TextMeshProUGUI fadeText;
    private float alphaValue;
    public float fadeAwayPerSecond;
    // Start is called before the first frame update
    void Start()
    {
        fadeText = GetComponent<TextMeshProUGUI>();
        fadeAwayPerSecond = 1 / fadeTime;
        alphaValue = fadeText.color.a;
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeTime > 0) 
        {
            alphaValue -= fadeAwayPerSecond * Time.deltaTime;
            fadeText.color = new Color(fadeText.color.r, fadeText.color.g, fadeText.color.b, alphaValue);
            fadeTime -= Time.deltaTime;
        }
    }
}
