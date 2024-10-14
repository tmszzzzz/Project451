using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YZBillboard : MonoBehaviour
{
    void Awake()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }
    // Update is called once per frame
    void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(new Vector3(Camera.main.transform.forward.x,0, Camera.main.transform.forward.z));
    }
}
