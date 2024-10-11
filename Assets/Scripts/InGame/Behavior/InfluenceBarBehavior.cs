using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfluenceBarBehavior : MonoBehaviour
{
    [SerializeField] private Slider influenceBarSlider;
    [SerializeField] private NodeBehavior nodeBehavior;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color awakenedColor;
    [SerializeField] private Color exposedColor;
    private int exposeThreshold;
    private int awakeThreshold;
    private BaseNodeBehavior.StatePrediction statePrediction;
    // Start is called before the first frame update
    void Start()
    {
        if (nodeBehavior == null) {
            Debug.LogWarning("NodeBehavior not found in InfluenceBarBehavior.");
        }
        exposeThreshold = nodeBehavior.properties.exposeThreshold;
        awakeThreshold = nodeBehavior.properties.awakeThreshold;
    }

    void updateInfluenceBar() 
    {
        statePrediction = nodeBehavior.RefreshState();

        influenceBarSlider.value = (float)statePrediction.influence / (float)exposeThreshold;
        //influenceBarSlider.value = 0.5f;
        if(influenceBarSlider.value != 0) {
            Debug.Log(influenceBarSlider.value);
        }

        if (statePrediction.state == Properties.StateEnum.NORMAL) {
            influenceBarSlider.fillRect.GetComponent<Image>().color = normalColor;
        } else if (statePrediction.state == Properties.StateEnum.AWAKENED) {
            influenceBarSlider.fillRect.GetComponent<Image>().color = awakenedColor;
        } else if (statePrediction.state == Properties.StateEnum.EXPOSED) {
            influenceBarSlider.fillRect.GetComponent<Image>().color = exposedColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        updateInfluenceBar();
    }
}
