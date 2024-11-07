using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoGenerationBehavior : MonoBehaviour
{
    private GameObject thisNode;
    public CanvasBehavior canvasBehavior;

    public float neighborToAwakeThresholdRatio = 0.5f;
    public bool isRoundUp = true;
    public int maximumNumOfBooks = 2;
    public int fallThreshold = 0;

    public int increasementFromAwakeToExpose = 1;
    // Start is called before the first frame update
    void Update()
    {
        thisNode = this.gameObject;
        canvasBehavior = GameObject.Find("Canvas").GetComponent<CanvasBehavior>();

        int neighborCount = 0;
        foreach (var nNode in canvasBehavior.GetNeighbors(thisNode)) 
        {
            neighborCount++;
        }

        RegionAdjustValues();

        thisNode.GetComponent<NodeBehavior>().properties.awakeThreshold = isRoundUp ? (int)(neighborCount * neighborToAwakeThresholdRatio + 1) : (int)(neighborCount * neighborToAwakeThresholdRatio);
        thisNode.GetComponent<NodeBehavior>().properties.exposeThreshold = thisNode.GetComponent<NodeBehavior>().properties.awakeThreshold + increasementFromAwakeToExpose;
        thisNode.GetComponent<NodeBehavior>().properties.maximumNumOfBooks = maximumNumOfBooks;
        thisNode.GetComponent<NodeBehavior>().properties.fallThreshold = fallThreshold;
        // thisNode.GetComponent<NodeBehavior>().properties.awakeThreshold = neighborCount %2 == 0 ? (neighborCount/2) : (neighborCount/2) + 1;
        // thisNode.GetComponent<NodeBehavior>().properties.exposeThreshold = thisNode.GetComponent<NodeBehavior>().properties.awakeThreshold + 1;
        // thisNode.GetComponent<NodeBehavior>().properties.maximumNumOfBooks = 2;
    }

    void RegionAdjustValues()
    {
        if(thisNode.GetComponent<NodeBehavior>().properties.region == 0)
        {
            neighborToAwakeThresholdRatio = 0.5f;
            isRoundUp = true;
            maximumNumOfBooks = 2;
            increasementFromAwakeToExpose = 1;
            fallThreshold = 0;

        }
        else if (thisNode.GetComponent<NodeBehavior>().properties.region == 1)
        {
            neighborToAwakeThresholdRatio = 0.8f;
            isRoundUp = true;
            maximumNumOfBooks = 3;
            increasementFromAwakeToExpose = 2;
            fallThreshold = 1;
        }
        else if (thisNode.GetComponent<NodeBehavior>().properties.region == 2)
        {
            neighborToAwakeThresholdRatio = 1.2f;
            isRoundUp = true;
            maximumNumOfBooks = 4;
            increasementFromAwakeToExpose = 3;
            fallThreshold = 2;
        }
    }
}
