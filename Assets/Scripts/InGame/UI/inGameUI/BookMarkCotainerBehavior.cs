using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BookMarkCotainerBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float hover_scale = 1f;
    [SerializeField] private float unhover_scale = 0.3f;
    private float target_scale = 0.3f;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        target_scale = hover_scale;
    }

    public void  OnPointerExit(PointerEventData eventData)
    {
        target_scale = unhover_scale;
    }

    private void Update()
    {
        foreach (Transform child in transform)
        {
            Vector3 currentScale = child.localScale;
            float newScale = Mathf.Lerp(currentScale.x, target_scale, Time.deltaTime * 2f);
            child.localScale = new Vector3(newScale, newScale, newScale);
        }
    }
}
