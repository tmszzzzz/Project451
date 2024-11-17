using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningVignette : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip heartbeat;

    public float r = 0;

    public float realA = 0;
    // Update is called once per frame
    void Update()
    {
        int v = GlobalVar.instance.globalExposureValue;
        int half = GlobalVar.instance.maxGlobalExposureValue / 2;
        int _3quarter = half / 2 + half;
        realA = Mathf.Lerp(realA, (float)(v - half) / (float)half, Time.deltaTime);
        image.color=new Color(0.5f + (r - 0.5f)* Mathf.Clamp(v-_3quarter,0,_3quarter / 3)/ (GlobalVar.instance.maxGlobalExposureValue - _3quarter),image.color.g,image.color.b,realA) ;
    }

    public void PlayHeartbeatSound()
    {
        Debug.Log("1");
        int v = GlobalVar.instance.globalExposureValue;
        int half = GlobalVar.instance.maxGlobalExposureValue / 2;
        int _3quarter = half / 2 + half;
        audioSource.PlayOneShot(heartbeat,1f*Mathf.Clamp(v-_3quarter,0,_3quarter / 3)/ (GlobalVar.instance.maxGlobalExposureValue - _3quarter));
    }
}
