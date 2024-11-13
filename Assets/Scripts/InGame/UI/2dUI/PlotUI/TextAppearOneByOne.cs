using UnityEngine;
using UnityEngine.UI; // Include if using Unity's built-in UI
using TMPro;
using System; // Include if using TextMeshPro

public class TextAppearOneByOne : MonoBehaviour
{
    public string fullText = "You shouldn't see this man."; // The full text to display
    public float typingSpeed = 0.1f; // Time between each character
    public AudioClip typingSound; // Reference to the typing sound effect
    public GameObject author; // Reference to the continue butto
    private TextMeshProUGUI uiText; // For Unity's built-in UI
    public bool isTypingFinished = false; // Flag to check if typing is finished

    void Start()
    {
        uiText = GetComponent<TextMeshProUGUI>(); // For Unity's built-in UIs
        // uiText = GetComponent<TextMeshProUGUI>(); // Uncomment if using TextMeshPro
        StartCoroutine(TypeText());
    }

    private System.Collections.IEnumerator TypeText()
    {
        foreach (char letter in fullText)
        {
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

        isTypingFinished = true; // Set the flag to true when typing is finished
    }

}