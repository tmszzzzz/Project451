using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathingEffect : MonoBehaviour
{
    public float minScaleF = 0.8f;      // Minimum scale (inhaled state)
    public float maxScaleF = 1.1f;  // Maximum scale (exhaled state)

    private Vector3 minScale ;      // Minimum scale (inhaled state)
    private Vector3 maxScale ;  // Maximum scale (exhaled state)
    public float breathingSpeed = 2.0f;         // Speed of the breathing cycle

    private void Start()
    {
        // Set the min and max scales
        minScale = new Vector3(minScaleF, minScaleF, minScaleF);
        maxScale = new Vector3(maxScaleF, maxScaleF, maxScaleF);
    }


    private void Update()
    {
        // Calculate the scale factor using a sinusoidal function to simulate breathing
        float scale = (Mathf.Sin(Time.time * breathingSpeed) + 1.0f) / 2.0f; // Scales from 0 to 1
        transform.localScale = Vector3.Lerp(minScale, maxScale, scale); // Lerp between min and max scales
    }
}
