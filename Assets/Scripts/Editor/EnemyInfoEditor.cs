using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyInfo))]
public class EnemyInfoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty iterator = serializedObject.GetIterator();
        bool enterChildren = true;
        while (iterator.NextVisible(enterChildren))
        {
            enterChildren = false;
            if (iterator.propertyPath == "patternStates")
                continue;
            EditorGUILayout.PropertyField(iterator, true);
        }

        if (GUILayout.Button("Open WindowEnemyPattern"))
        {
            WindowEnemyPattern.Open((EnemyInfo)target);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
