using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public enum ResourcePointUsedType
{
    Info,
    Range,
    Limit,
    DecreaseExposure,
}

public class ResourcePointAdderButton : MonoBehaviour
{
    private GlobalVar globalVar;
    [SerializeField] private ResourcePointUsedType resourcePointUsedType;
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private GameObject button;
    private float targetScale;
    [SerializeField] float lerpSpeed = 10f;
    private BreathingEffect _breathingEffect;

    // Start is called before the first frame update
    void Start()
    {
        globalVar = GlobalVar.instance;
        _breathingEffect = this.GetComponent<BreathingEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        if (globalVar.resourcePoint <= 0)
        {
            targetScale = 0;
            _breathingEffect.enabled = false;
        }
        else
        {
            targetScale = 1;
            _breathingEffect.enabled = true;
        }

        textMesh.text = globalVar.resourcePoint.ToString();

        if (button.transform.localScale.x != targetScale)
        {
            button.transform.localScale = Vector3.Lerp(button.transform.localScale, new Vector3(targetScale, targetScale, targetScale), lerpSpeed * Time.deltaTime);
        }
    }
}
