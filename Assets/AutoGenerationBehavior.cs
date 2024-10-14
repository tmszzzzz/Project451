using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoGenerationBehavior : MonoBehaviour
{
    private GameObject thisNode;
    public CanvasBehavior canvasBehavior;
    // Start is called before the first frame update
    void Start()
    {
        thisNode = this.gameObject;
        canvasBehavior = GameObject.Find("Canvas").GetComponent<CanvasBehavior>();

        int neighborCount = 0;
        foreach (var nNode in canvasBehavior.GetNeighbors(thisNode)) 
        {
            neighborCount++;
        }

        thisNode.GetComponent<NodeBehavior>().properties.awakeThreshold = neighborCount %2 == 0 ? neighborCount/2 : neighborCount + 1;
        thisNode.GetComponent<NodeBehavior>().properties.exposeThreshold = thisNode.GetComponent<NodeBehavior>().properties.awakeThreshold + 1;
        thisNode.GetComponent<NodeBehavior>().properties.maximumNumOfBooks = 2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
