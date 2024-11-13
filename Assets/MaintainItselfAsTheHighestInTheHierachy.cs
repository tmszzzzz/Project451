using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaintainItselfAsTheHighestInTheHierachy : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.SetAsFirstSibling();
    }
}
