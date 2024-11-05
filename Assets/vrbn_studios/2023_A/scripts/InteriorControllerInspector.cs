using UnityEngine;
using System.Collections;
using UnityEditor;

// Creates a custom Label on the inspector for all the scripts named ScriptName
// Make sure you have a ScriptName script in your
// project, else this will not work.
[CustomEditor(typeof(InteriorController))]
[CanEditMultipleObjects]
public class InteriorControllerInspector : Editor
{
    SerializedProperty enableInteriorMappingProp;

    void OnEnable()
    {
        // Setup the SerializedProperties.
        enableInteriorMappingProp = serializedObject.FindProperty ("enableInteriorMapping");
    }
    public override void OnInspectorGUI()
    {
        InteriorController ic = (InteriorController)target;

        serializedObject.Update ();

        base.OnInspectorGUI();

        if (!enableInteriorMappingProp.hasMultipleDifferentValues)
        {
            ic.Dirty();
        }
    }
}
