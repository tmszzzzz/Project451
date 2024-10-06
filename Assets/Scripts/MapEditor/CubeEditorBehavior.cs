using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CubeEditorBehavior : MonoBehaviour
{
    public PropertiesEditor properties;
    private Renderer objRenderer;
    private Dictionary<int, Color> ColorMap;
    public bool selected;
    // Start is called before the first frame update
    void Start()
    {
        objRenderer = GetComponent<MeshRenderer>();

        objRenderer.material = new Material(objRenderer.material);

        ColorMap = new Dictionary<int, Color>();
        ColorMap.Add(-1, Color.gray);
        ColorMap.Add(0, Color.green);
        ColorMap.Add(1, Color.yellow);
        ColorMap.Add(2, Color.red);
    }
    void Update()
    {
        objRenderer.material.color = selected ? Color.red : Color.white;
    }
}
