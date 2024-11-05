using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtController : MonoBehaviour
{
 public Material[] dirtMaterialArray;

    [Range(0.0f, 1.0f)]
    public float dirtIntensity = 0.25f;

     void Start(){

        var mrs = GetComponentsInChildren<MeshRenderer>();
        ArrayList dirtMaterialArraylist = new ArrayList();
        foreach (var mr in mrs)
        {
            var mats = mr.materials;
            foreach (var m in mats)
            {
                if (m.HasFloat("_dirtMultiplier"))
                    dirtMaterialArraylist.Add(m);
            }
        }

        dirtMaterialArray = new Material[dirtMaterialArraylist.Count];
        int i = 0;
        foreach (Material m in dirtMaterialArraylist)
        {
            dirtMaterialArray[i] = m;
            i++;
        }

        UpdateShaderValues(dirtIntensity);
    }
    
    void UpdateShaderValues(float dirtValue){
        if (dirtMaterialArray == null)
            return;
        foreach (Material mat in dirtMaterialArray)
        {
           mat.SetFloat("_dirtMultiplier", dirtValue); 
        }
    }

    public void Dirty(){ 
        UpdateShaderValues(dirtIntensity);
    }


    // Update is called once per frame
    void Update()
    { 
        // uncommnet this if you like to control in runtime 
        // UpdateShaderValues(dirtIntensity);
    }


    // void OnValidate() {
    //     UpdateShaderValues(dirtIntensity);
    // }
}
