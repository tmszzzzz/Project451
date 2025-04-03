using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookSlotBehavior : MonoBehaviour
{
    private int maxBook;
    private int currentBook;
    [SerializeField] private float slotInterval;
    [SerializeField] private GameObject thisNode;
    [SerializeField] private GameObject bookSlotPrefab;
    [SerializeField] private List<GameObject> bookSlots;
    // Start is called before the first frame update

    void Start()
    {
        //currentBook = thisNode.GetComponent<NodeBehavior>().properties.numOfBooks;
        //maxBook = thisNode.GetComponent<NodeBehavior>().properties.maximumNumOfBooks;

        bookSlots = new List<GameObject>();
        initializeBookSlots();
    }

    void initializeBookSlots() 
    {
        float startX = -slotInterval * (maxBook - 1) / 2;
        for (int i = 0; i < maxBook; i++) 
        {
            GameObject slot = Instantiate(bookSlotPrefab, Vector3.one , Quaternion.identity);
            slot.transform.SetParent(transform);
            slot.transform.localPosition = new Vector3(startX + i * slotInterval, 0, 0);
            slot.transform.localRotation = Quaternion.identity;
            bookSlots.Add(slot);
        }
    }

    void updateBookSlots() 
    {
        for (int i = 0; i < maxBook; i++)
        {
            if (i < currentBook)
            {
                bookSlots[i].GetComponent<Slot>().hasBook = true;
            }
            else 
            {
                bookSlots[i].GetComponent<Slot>().hasBook = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //maxBook = thisNode.GetComponent<NodeBehavior>().properties.maximumNumOfBooks;

        transform.rotation = transform.parent.rotation;
        //caculate the current book and update the book slots
        //currentBook = thisNode.GetComponent<NodeBehavior>().properties.numOfBooks;
        // currentBook += RoundManager.instance.BookAllocationMap[thisNode];
        updateBookSlots();
    }
}
