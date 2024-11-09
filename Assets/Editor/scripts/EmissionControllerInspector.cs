using UnityEngine;
using System.Collections;
using UnityEditor;

// Creates a custom Label on the inspector for all the scripts named ScriptName
// Make sure you have a ScriptName script in your
// project, else this will not work.
[CustomEditor(typeof(EmissionController))]
[CanEditMultipleObjects]
public class EmissionControllerInspector : Editor
{
    SerializedProperty emissionIntensityProp;

    void OnEnable()
    {
        // Setup the SerializedProperties.
        emissionIntensityProp = serializedObject.FindProperty ("emissionIntensity");
    }
    public override void OnInspectorGUI()
    {
        EmissionController ec = (EmissionController)target;

        serializedObject.Update ();

        base.OnInspectorGUI();

        if (!emissionIntensityProp.hasMultipleDifferentValues)
        {
            ec.Dirty();
        }
    }
}
