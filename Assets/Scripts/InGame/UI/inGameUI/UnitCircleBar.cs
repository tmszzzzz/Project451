using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UnitCircleBar : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float radius = 100f;
    [SerializeField] private float thickness = 20f;
    [SerializeField] private Color activeColor = Color.cyan;
    [SerializeField] private Color inactiveColor = Color.gray;
    [SerializeField] private float highlightOffset = 20f;
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private GameObject markerPrefab;

    private int maxValue;
    private int currentValue;
    private List<Image> unitImages = new List<Image>();
    private List<GameObject> markers = new List<GameObject>();
    private List<Vector3> originalPositions = new List<Vector3>();

    public void Initialize(int maxValue)
    {
        this.maxValue = maxValue;
        CreateUnits();
    }

    private void CreateUnits()
    {
        // Clear existing units
        foreach (var unit in unitImages)
        {
            if (unit != null)
                Destroy(unit.gameObject);
        }
        unitImages.Clear();
        originalPositions.Clear();

        float angleStep = 360f / maxValue;
        float currentAngle = 0f;

        for (int i = 0; i < maxValue; i++)
        {
            GameObject unitObj = Instantiate(unitPrefab, transform);
            Image unitImage = unitObj.GetComponent<Image>();
            
            // Configure the image
            unitImage.type = Image.Type.Filled;
            unitImage.fillMethod = Image.FillMethod.Radial360;
            unitImage.fillAmount = 1f / maxValue;
            unitImage.color = inactiveColor;

            // Position the unit
            RectTransform rectTransform = unitObj.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(radius * 2, radius * 2);
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localRotation = Quaternion.Euler(0, 0, currentAngle);

            unitImages.Add(unitImage);
            originalPositions.Add(rectTransform.localPosition);
            
            currentAngle += angleStep;
        }
    }

    public void SetValue(int value)
    {
        int previousValue = currentValue;
        currentValue = Mathf.Clamp(value, 0, maxValue);

        // Animate the color change
        for (int i = 0; i < unitImages.Count; i++)
        {
            if (i < currentValue)
            {
                unitImages[i].DOColor(activeColor, animationDuration);
            }
            else
            {
                unitImages[i].DOColor(inactiveColor, animationDuration);
            }
        }
    }

    public void HighlightRange(int startIndex, int endIndex, bool highlight)
    {
        startIndex = Mathf.Clamp(startIndex, 0, maxValue - 1);
        endIndex = Mathf.Clamp(endIndex, 0, maxValue - 1);

        for (int i = startIndex; i <= endIndex; i++)
        {
            RectTransform rectTransform = unitImages[i].GetComponent<RectTransform>();
            Vector3 targetPos = highlight ? 
                originalPositions[i] + (rectTransform.localPosition - Vector3.zero).normalized * highlightOffset :
                originalPositions[i];

            rectTransform.DOLocalMove(targetPos, animationDuration);
        }
    }

    public void SetMarker(int index, Color color)
    {
        if (index < 0 || index >= maxValue)
            return;

        GameObject marker = Instantiate(markerPrefab, transform);
        RectTransform markerRect = marker.GetComponent<RectTransform>();
        Image markerImage = marker.GetComponent<Image>();

        // Position the marker
        float angle = (360f / maxValue) * index;
        float radians = angle * Mathf.Deg2Rad;
        Vector3 position = new Vector3(
            Mathf.Cos(radians) * (radius + thickness/2),
            Mathf.Sin(radians) * (radius + thickness/2),
            0
        );

        markerRect.localPosition = position;
        markerRect.localRotation = Quaternion.Euler(0, 0, angle);
        markerImage.color = color;

        markers.Add(marker);
    }

    public void ClearMarkers()
    {
        foreach (var marker in markers)
        {
            if (marker != null)
                Destroy(marker.gameObject);
        }
        markers.Clear();
    }

    public void SetActiveColor(Color color)
    {
        activeColor = color;
        // Update current active segments
        for (int i = 0; i < currentValue; i++)
        {
            if (i < unitImages.Count)
            {
                unitImages[i].color = color;
            }
        }
    }
}

