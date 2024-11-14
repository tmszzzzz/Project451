using UnityEngine;
using UnityEngine.UI; // Include if using Unity's built-in UI
using TMPro;
using System;
using UnityEngine.Events; // Include if using TextMeshPro

public class TextAppearOneByOne : MonoBehaviour
{
    public string fullText = "You shouldn't see this man."; // The full text to display
    public float typingSpeed = 0.1f; // Time between each character
    public AudioClip typingSound; // Reference to the typing sound effect
    private TextMeshProUGUI uiText; // For Unity's built-in UI
    public bool isTypingFinished = false; // Flag to check if typing is finished
    
    public UnityEvent onTypingFinished = new UnityEvent();
    private string originalText;
    void Start()
    {
        uiText = GetComponent<TextMeshProUGUI>(); // For Unity's built-in UIs
        originalText = uiText.text;
        //onTypingFinished.AddListener(()=>PlotDisplay.Instance.PlotDisplayIsFree());
        StartCoroutine(TypeText());
    }
    

    private System.Collections.IEnumerator TypeText()
    {
        bool stopEarly = false;
        
        foreach (char letter in fullText)
        {
            if (PlotDisplay.Instance.isSkipping)
            {
                stopEarly = true;
                break;
            }
            
            uiText.text += letter; // Add one character at a time
            //Debug.Log(audioSource.isPlaying);
            // Check if the current character is a space
            if (letter == ' ')
            {
                yield return new WaitForSeconds(typingSpeed); // Longer pause for spaces
            }
            else
            {
                yield return new WaitForSeconds(typingSpeed); // Wait for the specified time
            }
        }

        if (stopEarly)
        {
            uiText.text = originalText + fullText;
        }

        onTypingFinished.Invoke();  
        isTypingFinished = true; // Set the flag to true when typing is finished
    }

}