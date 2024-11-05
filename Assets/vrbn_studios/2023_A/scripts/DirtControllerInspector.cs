using UnityEngine;
using System.Collections;
using UnityEditor;

// Creates a custom Label on the inspector for all the scripts named ScriptName
// Make sure you have a ScriptName script in your
// project, else this will not work.
[CustomEditor(typeof(DirtController))]
[CanEditMultipleObjects]
public class DirtControllerInspector : Editor
{
    SerializedProperty dirtIntensityProp;

    void OnEnable()
    {
        // Setup the SerializedProperties.
        dirtIntensityProp = serializedObject.FindProperty ("dirtIntensity");
    }
    public override void OnInspectorGUI()
    {
        DirtController dc = (DirtController)target;

        serializedObject.Update ();

        base.OnInspectorGUI();

        if (!dirtIntensityProp.hasMultipleDifferentValues)
        {
            dc.Dirty();
        }
    }
}
