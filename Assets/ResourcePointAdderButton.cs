using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourcePointAdderButton : MonoBehaviour
{
    private GlobalVar globalVar;
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private GameObject button;
    private float targetScale;
    [SerializeField] float lerpSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        globalVar = GlobalVar.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (globalVar.resourcePoint <= 0)
        {
            targetScale = 0;
        }
        else
        {
            targetScale = 1;
        }

        textMesh.text = globalVar.resourcePoint.ToString();

        if (button.transform.localScale.x != targetScale)
        {
            button.transform.localScale = Vector3.Lerp(button.transform.localScale, new Vector3(targetScale, targetScale, targetScale), lerpSpeed * Time.deltaTime);
        }
    }
}
