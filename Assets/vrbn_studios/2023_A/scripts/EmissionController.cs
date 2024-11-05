using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public class EmissionController : MonoBehaviour
{
    [HideInInspector]
    public float maxIntensity = 40000;
    [HideInInspector] 
    public float minWaitTime = 300;
    [HideInInspector] 
    public float maxWaitTime = 800;

    public Material[] emissionMaterialArray;

    public float emissionIntensity;

    private ArrayList courutineList = new ArrayList();

    void Start(){

        var mrs = GetComponentsInChildren<MeshRenderer>();
        ArrayList materialsArraylist = new ArrayList();
        foreach (var mr in mrs)
        {
            var mats = mr.materials;
            foreach (var m in mats)
            {
                if (m.HasFloat("_emissionMultiplier"))
                    materialsArraylist.Add(m);
            }
        }

        emissionMaterialArray = new Material[materialsArraylist.Count];
        int i = 0;
        foreach (Material m in materialsArraylist)
        {
            emissionMaterialArray[i] = m;
            i++;
        }

        UpdateShaderValues(emissionIntensity);
    }

    void UpdateShaderValues(float emissionValue)
    {
        if (emissionMaterialArray == null)
            return;
        foreach (Material mat in emissionMaterialArray)
        {
            mat.SetFloat("_emissionMultiplier", emissionValue);
        }
    }

    public void Dirty()
    {
        UpdateShaderValues(emissionIntensity);
    }


    // Update is called once per frame
    void Update()
    {
        // uncommnet this if you like to control in runtime 
        // UpdateShaderValues(emissionIntensity);
    }


    // void OnValidate()
    // {
    //     UpdateShaderValues(emissionIntensity);
    // }


    public void enableLights()
    {
        foreach (Material mat in emissionMaterialArray)
        {
            courutineList.Add(StartCoroutine(ApplyLightsValue(Random.Range(minWaitTime, maxWaitTime), maxIntensity, mat)));
        }
    }

    // Method to setup the lights fade in 
    private IEnumerator ApplyLightsValue(float inc, float max, Material material)
    {
        float i = 0;
        yield return new WaitForSeconds(Random.Range(0.5f,2));

        while (i <= max)
        {

            i = i + inc;
            if (material.HasFloat("_emissionMultiplier"))
            {
                material.SetFloat("_emissionMultiplier", i);
                yield return new WaitForSeconds(0.05f);
            }
        }
    }

    public void stopCourutines(){
        foreach (Coroutine coru in courutineList)
        {
            StopCoroutine(coru);
        }
    }
}
