using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditionCameraBehavior : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Camera thisCamera;
    void Update()
    {
        thisCamera.transform.position = cam.transform.position;
        thisCamera.transform.rotation = cam.transform.rotation;
        thisCamera.fieldOfView = cam.fieldOfView;
    }
}
