using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlotsScrollRect : ScrollRect
{
    private float initialVerticalPosition;

    protected override void Start()
    {
        base.Start();
        initialVerticalPosition = verticalNormalizedPosition; // Store the initial position
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
        
        // Prevent scrolling below the initial position
        if (verticalNormalizedPosition < initialVerticalPosition)
        {
            verticalNormalizedPosition = initialVerticalPosition;
        }
    }
}
