using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

[CustomEditor(typeof(CardData), true), CanEditMultipleObjects]
public class CardDataEditor : Editor
{
    private CardData cardData;

    private void OnEnable()
    {
        cardData = target as CardData;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawFields(serializedObject.targetObject, serializedObject.targetObject.GetType());

        //GUILayout.BeginVertical();

        //EditorGUILayout.PropertyField(type, true);
        //EditorGUILayout.Space(10);
        //EditorGUILayout.PropertyField(buffList, true);
        EditorGUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayoutOption[] optionButtons = new[] { GUILayout.ExpandWidth(true) };
        if (GUILayout.Button("Add BuffPublic", optionButtons))
        {
            cardData.buffList.Add(new BuffPublic(BuffPublic.Type.Str, 0));
        }
        if (GUILayout.Button("Add BuffPlayer", optionButtons))
        {
            cardData.buffList.Add(new BuffPlayer(BuffPlayer.Type.Confused, 0));
        }
        if (GUILayout.Button("Add BuffEnemy", optionButtons))
        {
            cardData.buffList.Add(new BuffEnemy(BuffEnemy.Type.testEnemy2, 0));
        }
        GUILayout.EndHorizontal();
        //GUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }

    // CardData와 그 자식 클래스들의 변수를 모두 프로퍼티로 만듬
    private void DrawFields(object targetObject, Type type)
    {
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        foreach (FieldInfo field in fields)
        {
            if (field.IsPublic || field.GetCustomAttribute<SerializeField>() != null)
            {
                SerializedProperty property = serializedObject.FindProperty(field.Name);
                if (property != null)
                {
                    EditorGUILayout.PropertyField(property, new GUIContent(field.Name), true);
                }
            }
        }

        if (type.BaseType != null && type.BaseType != typeof(MonoBehaviour))
        {
            DrawFields(targetObject, type.BaseType);
        }
    }
}
