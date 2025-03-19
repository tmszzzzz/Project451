using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProbabilityOfInoText : MonoBehaviour
{
    private GlobalVar globalVar;
    private TextMeshProUGUI textMesh;
    // Start is called before the first frame update
    void Start()
    {
        globalVar = GlobalVar.instance;
        textMesh = gameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        textMesh.text = $"{globalVar.probabilityOfNodesInspectingDetective}%";
    }
}
