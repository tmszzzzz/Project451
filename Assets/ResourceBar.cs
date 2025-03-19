using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UI;
using UnityEngine.UI;

public class ResourceBar : MonoBehaviour
{
    [SerializeField] private Slider Resourceslider; 
    
    // Start is called before the first frame update
    void Start()
    {
        Resourceslider.maxValue = GlobalVar.instance.maxResourcePoint;
        Resourceslider.value = GlobalVar.instance.resourcePoint;
    }

    // Update is called once per frame
    void Update()
    {
        Resourceslider.value = GlobalVar.instance.resourcePoint;
    }
}
