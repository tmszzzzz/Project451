using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class InfluenceBarBehavior : MonoBehaviour
{
    [SerializeField] private Material initMaterial;
    [SerializeField] private Slider influenceBarSlider;
    [SerializeField] private NodeBehavior nodeBehavior;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color awakenedColor;
    [SerializeField] private Color exposedColor;
    private float targetValue;
    private int exposeThreshold;
    private int awakeThreshold;
    private int fallThreshold;
    // [SerializeField] private float minValue = 0.05f;
    private BaseNodeBehavior.StatePrediction statePrediction;
    private float basicInfluence = 0;
    public Material _material;
    // Start is called before the first frame update
    void Start()
    {
        if (nodeBehavior == null) {
            Debug.LogWarning("NodeBehavior not found in InfluenceBarBehavior.");
        }
        _material = new Material(initMaterial);
        updateInfluenceBar(_material);
        exposeThreshold = nodeBehavior.properties.exposeThreshold;
        awakeThreshold = nodeBehavior.properties.awakeThreshold;
        fallThreshold = nodeBehavior.properties.fallThreshold;
        influenceBarSlider.fillRect.GetComponent<Image>().material = _material;
        //RoundManager.Instance.BookAllocationChange += updateInfluenceBar;
    }

    void updateInfluenceBar(Material material) 
    {
        statePrediction = nodeBehavior.PredictState();
        int additionalInfluence = statePrediction.additionalInfluence;
        if (statePrediction.state == Properties.StateEnum.NORMAL) {
            material.SetColor("_ColorOn", normalColor);
        } else if (statePrediction.state == Properties.StateEnum.AWAKENED) {
            material.SetColor("_ColorOn", awakenedColor);
        } else if (statePrediction.state == Properties.StateEnum.EXPOSED) {
            material.SetColor("_ColorOn", exposedColor);
        }
        // material.SetColor("_ColorOff", );
        material.SetFloat("_Segments", exposeThreshold);
        material.SetFloat("_Active", basicInfluence);
        // material.SetFloat("_HighlightCount", basicInfluence);
        material.SetFloat("_Triangle1Index", Mathf.Max(0, fallThreshold - additionalInfluence));
        material.SetFloat("_Triangle2Index", Mathf.Max(0, awakeThreshold - additionalInfluence));
        // targetValue = Math.Max((float)statePrediction.basicInfluence / (float)exposeThreshold, minValue);
        // if (statePrediction.state == Properties.StateEnum.NORMAL) {
        //     //material.
        //     influenceBarSlider.fillRect.GetComponent<Image>().color = normalColor;
        // } else if (statePrediction.state == Properties.StateEnum.AWAKENED) {
        //     influenceBarSlider.fillRect.GetComponent<Image>().color = awakenedColor;
        // } else if (statePrediction.state == Properties.StateEnum.EXPOSED) {
        //     influenceBarSlider.fillRect.GetComponent<Image>().color = exposedColor;
        // }
    }

    // Update is called once per frame
    void Update()
    {
        updateInfluenceBar(_material);
        // Lerp效果
        LerpAnimation();
    }

    void LerpAnimation()
    {
        if (statePrediction.basicInfluence - basicInfluence >= 0.5 || statePrediction.basicInfluence - basicInfluence <= -0.5)
        {
            basicInfluence = Mathf.Lerp(basicInfluence, statePrediction.basicInfluence, 0.05f);
        }
        else
        {
            // 防止float值始终不能变到目标值
            // 为0时留有一点
            basicInfluence = statePrediction.basicInfluence == 0 ? 0.2f : statePrediction.basicInfluence;
        }
    }
    void OnDestroy()
    {
        // // 手动销毁材质实例
        // if (_material != null) {
        //     Destroy(_material);
        // }
        //RoundManager.Instance.RoundChange -= updateInfluenceBar;
    }
}
