using Sequences;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ToScaleSequenceBundle))]
public class ToScaleSequenceBundleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ToScaleSequenceBundle config = (ToScaleSequenceBundle)target;

        config.SimpleSetting = GUILayout.Toggle(config.SimpleSetting, $"SimpleSetting");
        if (config.SimpleSetting)
        {
            config.ToFScale = EditorGUILayout.FloatField("To Scale", config.ToFScale);
        }
        else
        {
            config.ToScale = EditorGUILayout.Vector3Field("To Scale", config.ToScale);
        }
        config.ScaleDuration = EditorGUILayout.FloatField("Scale Duration", config.ScaleDuration);

        serializedObject.ApplyModifiedProperties();

        EditorUtility.SetDirty(config);
    }
}