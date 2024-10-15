using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleAnimator : MonoBehaviour
{
    public GameObject uiText;  // Reference to the Text component
    public RectTransform targetTransform;  // Target position for the text
    private Vector3 targetPosition;  // Target position for the text
    public float targetScale = 3f;  // Target size for the text
    public float animationDuration = 2f;  // Duration for the animation
    
    private RectTransform rectTransform;  // To access and control position
    private Vector3 initialPosition;  // Starting position
    private float initialScale;  // Starting font size
    private float elapsedTime = 0f;  // Timer to track animation progress
    public bool isFinished = false;
    
    void Start()
    {
        targetPosition = targetTransform.anchoredPosition;  // Get target position
        rectTransform = uiText.GetComponent<RectTransform>();  // Get RectTransform component
        initialPosition = rectTransform.anchoredPosition;  // Store initial position
        initialScale = rectTransform.localScale.x;  // Store initial font size
    }

    void Update()
    {
        if (elapsedTime < animationDuration)  // If animation is not finished
        {
            elapsedTime += Time.deltaTime;  // Increment elapsed time
            float t = Mathf.Clamp01(elapsedTime / animationDuration);  // Normalize time (0 to 1)

            // Interpolate position and size
            rectTransform.anchoredPosition = Vector3.Lerp(initialPosition, targetPosition, t);
            Vector3 newScale = Vector3.one * Mathf.Lerp(initialScale, targetScale, t);
            uiText.GetComponent<RectTransform>().localScale = newScale;
        }
        isFinished = elapsedTime >= animationDuration;  // Set flag when animation is finished
    }
}
