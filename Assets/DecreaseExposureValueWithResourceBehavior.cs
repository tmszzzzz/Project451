using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecreaseExposureValueWithResourceBehavior : MonoBehaviour
{
    private GlobalVar globalVar;
    private float targetScale;
    [SerializeField] private float lerpSpeed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        globalVar = GlobalVar.instance; 
    }

    // Update is called once per frame
    void Update()
    {
        targetScale = globalVar.resourcePoint > 0 ? 1 : 0;

        if (this.gameObject.transform.localScale.x != targetScale)
        {
            this.gameObject.transform.localScale = Vector3.Lerp(this.gameObject.transform.localScale, new Vector3(targetScale, targetScale, targetScale), lerpSpeed * Time.deltaTime);
        }
    }
}
