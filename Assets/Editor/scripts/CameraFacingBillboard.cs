using UnityEngine;

public class CameraFacingBillboard : MonoBehaviour
{
    private Camera m_Camera;

    private void Start()
    {
        m_Camera = Camera.main;
    }

    //Orient the camera after all movement is completed this frame to avoid jittering
    void LateUpdate()
    {
        var lookPos = m_Camera.transform.position - transform.position;
        lookPos.y = 0;
        transform.rotation = Quaternion.LookRotation(lookPos);
    }
}