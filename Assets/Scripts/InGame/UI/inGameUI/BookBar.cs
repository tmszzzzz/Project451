using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
public class BookBar : MonoBehaviour
{
    [SerializeField] private GameObject thisnode;
    [SerializeField] private Slider slider;
    private Properties properties;
    private int maxBookNum;
    // Start is called before the first frame update
    void Start()
    {
        properties = thisnode.GetComponent<NodeBehavior>().properties;
        maxBookNum = math.max(properties.maximumNumOfBooks, 1);
    }

    // Update is called once per frame
    void Update()
    {
        int booknum = properties.numOfBooks;
        // booknum += RoundManager.instance.BookAllocationMap[thisnode];
        slider.value = (float)booknum / maxBookNum;
    }
}
