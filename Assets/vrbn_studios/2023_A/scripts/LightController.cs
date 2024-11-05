using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
// using UnityEngine.Rendering.HighDefinition;
using UnityEngine;

public class LightController : MonoBehaviour
{
    float maxEmissionValue = 20;
    public float maxEmissionValueURP = 20;
    public float maxEmissionValueHDRP = 250000;
    public Transform sunTransform;
    // public Transform camera;
    // public Light light;
    public Color nightColor = Color.black;
    public Color dayColor = new Color(0.4523673f, 0.7669336f, 0.9150943f);

    bool lights = false;

    public Camera mainCamera;

    public Light mainLight;

    private EmissionController[] emissionControllers;

    // Start is called before the first frame update
    void Start()
    {
        emissionControllers = FindObjectsOfType<EmissionController>();
    }

    void setEnableLights(bool lights)
    {
        // Changing camera backgroup color
        mainCamera.clearFlags = CameraClearFlags.SolidColor;
        mainCamera.backgroundColor = lights ? nightColor : dayColor;
        mainLight.color = lights ? nightColor : dayColor;
        RenderSettings.ambientLight = lights ? nightColor : Color.white;
    }

    void setEmissionValue(bool lights)
    {
        if (GraphicsSettings.currentRenderPipeline.GetType().ToString().Contains("HighDefinition"))
        {
            maxEmissionValue = maxEmissionValueHDRP;
        }
        else 
        {
            maxEmissionValue = maxEmissionValueURP;
        }

        foreach (EmissionController ec in emissionControllers)
        {
            ec.emissionIntensity = lights ? maxEmissionValue : 0;
            ec.Dirty();
        }
    }

    void updateLights(bool lights)
    {
        // enable/disable directional lights
        setEnableLights(lights);

        // setting up emission value
        setEmissionValue(lights);
    }

    public void toggleLight()
    {
        lights = !lights;
        updateLights(lights);
    }
}
