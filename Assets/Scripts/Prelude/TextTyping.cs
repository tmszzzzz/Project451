using UnityEngine;
using UnityEngine.UI; // Include if using Unity's built-in UI
using TMPro;
using System; // Include if using TextMeshPro

public class TextTyping : MonoBehaviour
{
    public string fullText = "You shouldn't see this man."; // The full text to display
    public float typingSpeed = 0.1f; // Time between each character
    public AudioClip typingSound; // Reference to the typing sound effect
    public GameObject author; // Reference to the continue butto
    private TextMeshProUGUI uiText; // For Unity's built-in UI
    private AudioSource audioSource; // Audio source for playing sounds
    private float timeBetweenFinishedAndAuthor; // Time between typing finished and author text
    // private TextMeshProUGUI uiText; // Uncomment if using TextMeshPro
    public bool isTypingFinished = false; // Flag to check if typing is finished

    void Start()
    {
        uiText = GetComponent<TextMeshProUGUI>(); // For Unity's built-in UIs
        // uiText = GetComponent<TextMeshProUGUI>(); // Uncomment if using TextMeshPro
        audioSource = gameObject.AddComponent<AudioSource>(); // Add an AudioSource component
        audioSource.clip = typingSound; // Assign the audio clip
        audioSource.loop = true;
        uiText.text = ""; // Set initial text to empty
        timeBetweenFinishedAndAuthor = typingSpeed; // Calculate time
        StartCoroutine(TypeText());
    }

    private System.Collections.IEnumerator TypeText()
    {
        audioSource.Play(); // Play the typing sound
        foreach (char letter in fullText)
        {
            uiText.text += letter; // Add one character at a time
            //Debug.Log(audioSource.isPlaying);
            // Check if the current character is a space
            if (letter == ' ')
            {
                yield return new WaitForSeconds(typingSpeed * 3); // Longer pause for spaces
            }
            else
            {
                yield return new WaitForSeconds(typingSpeed); // Wait for the specified time
            }
        }

        isTypingFinished = true; // Set the flag to true when typing is finished
    }

    void Update()
    {
        Debug.Log(audioSource.isPlaying);
        if (isTypingFinished)
        {
            audioSource.Stop(); // Stop the typing sound
            timeBetweenFinishedAndAuthor -= Time.deltaTime; // Count down the time
            if (timeBetweenFinishedAndAuthor <= 0)
            {
                author.SetActive(true); // Show the author text
            }
        }
    }
}