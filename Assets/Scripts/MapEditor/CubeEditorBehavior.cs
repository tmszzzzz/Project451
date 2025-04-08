using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CubeEditorBehavior : MonoBehaviour
{
    public Properties properties;
    private Renderer objRenderer;
    private Dictionary<int, Color> ColorMap;
    public bool selected;

    public List<GameObject> TypeTag;

    public TextMeshPro BookNum;
    // Start is called before the first frame update
    void Start()
    {
        objRenderer = GetComponent<MeshRenderer>();

        objRenderer.material = new Material(objRenderer.material);

        ColorMap = new Dictionary<int, Color>();
        ColorMap.Add(0, Color.green);
        ColorMap.Add(1, Color.blue);
        ColorMap.Add(2, Color.red);
    }
    void Update()
    {
        objRenderer.material.color = ColorMap[properties.region] + (!selected ? Color.black : 0.5f * Color.white);
        int l = TypeTag.Count;
        for (int i = 0; i < l; i++)
        {
            if(i != (int)properties.type-1) TypeTag[i].SetActive(false);
            else TypeTag[i].SetActive(true);
        }

        if (properties.borrowBooks.Count > 0)
        {
            BookNum.gameObject.SetActive(true);
            BookNum.text = $"{properties.borrowBooks.Count}";
        }else BookNum.gameObject.SetActive(false);
    }
}
