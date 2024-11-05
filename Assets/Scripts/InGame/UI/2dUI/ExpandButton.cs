using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ExpandButton : MonoBehaviour
{
    public SlidableInfoPanel slidableInfoPanel;
    public Image arrowImage;
    private bool isExpanded = true;
    // Start is called before the first frame update
    public void toggleExpand()
    {
        isExpanded = !isExpanded;
        if (isExpanded)
        {
            slidableInfoPanel.Expand();
        }
        else
        {
            slidableInfoPanel.Shrink();
        }
    }   

    void Update()
    {
        if (isExpanded)
        {
            arrowImage.transform.rotation = Quaternion.Lerp(arrowImage.transform.rotation, Quaternion.Euler(0, 0, 90), Time.deltaTime * 5f);
        }
        else 
        {
            arrowImage.transform.rotation = Quaternion.Lerp(arrowImage.transform.rotation, Quaternion.Euler(0, 0, -90), Time.deltaTime * 5f);
        }
    }

    // Update is called once per fram
}
