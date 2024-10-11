using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public GameObject testPrefab;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(testPrefab, new Vector3(0, 5, 0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
