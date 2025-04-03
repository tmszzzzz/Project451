using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class NumberOfCollectedBooks : MonoBehaviour
{
    private int gainBooks = 0;
    // Update is called once per frame
    void Update()
    {
        foreach (GameObject node in RoundManager.instance.canvas.GetNodeList())
        {
            NodeBehavior nodeBehavior = node.GetComponent<NodeBehavior>();
            gainBooks += nodeBehavior.properties.books.Count;
        }
        gameObject.GetComponent<TextMeshProUGUI>().text = $"{gainBooks}";
        gainBooks = 0;
    }
}
