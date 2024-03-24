using Sequences;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LoopScaleSequenceBundle))]
public class LoopRotateSequenceBundleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LoopScaleSequenceBundle config = (LoopScaleSequenceBundle)target;

        config.SimpleSetting = GUILayout.Toggle(config.SimpleSetting, $"SimpleSetting");
        if (config.SimpleSetting)
        {
            config.FromfScale = EditorGUILayout.FloatField("From Scale", config.FromfScale);
            config.TofScale = EditorGUILayout.FloatField("To Scale", config.TofScale);
        }
        else
        {
            config.FromScale = EditorGUILayout.Vector3Field("From Scale", config.FromScale);
            config.ToScale = EditorGUILayout.Vector3Field("To Scale", config.ToScale);
        }
        config.ScaleDuration = EditorGUILayout.FloatField("Scale Duration", config.ScaleDuration);

        serializedObject.ApplyModifiedProperties();

        EditorUtility.SetDirty(config);
    }
}