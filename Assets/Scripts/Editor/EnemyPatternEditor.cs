using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

[CustomEditor(typeof(EnemyPattern), true), CanEditMultipleObjects]
public class EnemyPatternEditor : Editor
{
    private EnemyPattern enemyPattern;

    private void OnEnable()
    {
        enemyPattern = target as EnemyPattern;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawFields(serializedObject.targetObject, serializedObject.targetObject.GetType());

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawFields(object _targetObject, Type _type)
    {
        // 부모 클래스가 있으면, 부모 클래스의 변수부터 자식 클래스의 변수 순서로 진행
        if (_type.BaseType != null && _type.BaseType == typeof(EnemyPattern))
        {
            DrawFields(_targetObject, _type.BaseType);
        }

        FieldInfo[] fields = _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        foreach (FieldInfo field in fields)
        {
            if (field.IsPublic || field.GetCustomAttribute<SerializeField>() != null)
            {
                SerializedProperty property = serializedObject.FindProperty(field.Name);
                if (property != null)
                {
                    EditorGUILayout.PropertyField(property, new GUIContent(field.Name), true);

                    // 특정 프로퍼티만 버튼을 추가하는 등의 작업을 하기 위해 이름으로 구분해서 별도 작업
                    switch (property.name)
                    {
                        case "buffList":
                            {
                                if (property.isArray && property.arraySize == 0)
                                {
                                    property.isExpanded = false;
                                }

                                EditorGUILayout.Space(6);

                                GUILayout.BeginHorizontal();
                                GUILayoutOption[] optionButtons = new[] { GUILayout.ExpandWidth(true) };
                                
                                if (GUILayout.Button("Add BuffPlayer", optionButtons))
                                {
                                    enemyPattern.AddBuff(new BuffData(BUFF_TARGET.Player, null, BUFF_TYPE.Artifact, -1, false, false));
                                    property.isExpanded = true;
                                }
                                if (GUILayout.Button("Add BuffEnemy", optionButtons))
                                {
                                    enemyPattern.AddBuff(new BuffData(BUFF_TARGET.Enemy, null, BUFF_TYPE.Artifact, -1, false, false));
                                    property.isExpanded = true;
                                }
                                GUILayout.EndHorizontal();

                                if (GUILayout.Button("Removel All Buffs"))
                                {
                                    enemyPattern.ClearBuffList();
                                }

                                EditorGUILayout.Space(14);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
