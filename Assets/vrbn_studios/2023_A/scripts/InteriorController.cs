using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InteriorController : MonoBehaviour
{

    public Material[] interiorMaterialArray;

    public bool enableInteriorMapping = true;

    void UpdateShaderValues(bool enableInteriorMapping)
    {
        if (interiorMaterialArray == null)
            return;
        foreach (Material mat in interiorMaterialArray)
        {

            if (enableInteriorMapping)
            {
                mat.EnableKeyword("_ENABLEINTERIOR_ON");
            }
            else
            {
                mat.DisableKeyword("_ENABLEINTERIOR_ON");
            }
        }
    }

    public void Dirty(){
        UpdateShaderValues(enableInteriorMapping);
    }

    // void OnValidate() {
    //     UpdateShaderValues(enableInteriorMapping);
    // }


    
}
