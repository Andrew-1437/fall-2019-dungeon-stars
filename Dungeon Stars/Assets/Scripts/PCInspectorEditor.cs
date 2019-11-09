using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerController))]
public class PCInspectorEditor : Editor
{
    SerializedProperty enableHeat;
    SerializedProperty heatDisperse;
    SerializedProperty primaryUsesHeat;
    SerializedProperty primHeatGen;
    SerializedProperty secondaryUsesHeat;
    SerializedProperty secHeatGen;
    SerializedProperty heatWarnAudio;

    private void OnEnable()
    {
        enableHeat = serializedObject.FindProperty("enableHeat");
        heatDisperse = serializedObject.FindProperty("heatDisperse");
        primaryUsesHeat = serializedObject.FindProperty("primaryUsesHeat");
        primHeatGen = serializedObject.FindProperty("primHeatGen");
        secondaryUsesHeat = serializedObject.FindProperty("secondaryUsesHeat");
        secHeatGen = serializedObject.FindProperty("secHeatGen");
        heatWarnAudio = serializedObject.FindProperty("heatWarnAudio");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        serializedObject.Update();

        //PlayerController pc = (PlayerController)target;

        GUILayout.Label("");
        enableHeat.boolValue = EditorGUILayout.Toggle("Enable Heat", enableHeat.boolValue);

        if (enableHeat.boolValue)
        {
            heatDisperse.floatValue = EditorGUILayout.FloatField("Heat Dispersion / Tick", heatDisperse.floatValue);
            primaryUsesHeat.boolValue = EditorGUILayout.Toggle("Primary Uses Heat", primaryUsesHeat.boolValue);
            if (primaryUsesHeat.boolValue)
                primHeatGen.floatValue = EditorGUILayout.FloatField("Heat Gen / Shot", primHeatGen.floatValue);
            secondaryUsesHeat.boolValue = EditorGUILayout.Toggle("Secondary Uses Heat", secondaryUsesHeat.boolValue);
            if (secondaryUsesHeat.boolValue)
                secHeatGen.floatValue = EditorGUILayout.FloatField("Heat Gen / Shot", secHeatGen.floatValue);
            heatWarnAudio.objectReferenceValue = (AudioSource)EditorGUILayout.ObjectField("Heat Warning Audio", heatWarnAudio.objectReferenceValue, typeof(AudioSource));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
