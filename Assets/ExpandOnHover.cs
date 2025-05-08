using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandOnHover : MonoBehaviour
{
    [SerializeField] private float expandScale = 1.2f; // Scale when hovered
    [SerializeField] private float normalScale = 1f;   // Normal scale
    [SerializeField] private float smoothTime = 0.2f;  // Smooth transition time

    private Vector3 targetScale;
    private Vector3 currentVelocity;

    // Start is called before the first frame update
    void Start()
    {
        targetScale = Vector3.one * normalScale;
        transform.localScale = targetScale;
    }

    // Update is called once per frame
    void Update()
    {
        // Smoothly interpolate current scale to target scale
        transform.localScale = Vector3.SmoothDamp(
            transform.localScale,
            targetScale,
            ref currentVelocity,
            smoothTime
        );
    }

    void OnMouseEnter()
    {
        targetScale = Vector3.one * expandScale;
    }

    void OnMouseExit()
    {
        targetScale = Vector3.one * normalScale;
    }
}
