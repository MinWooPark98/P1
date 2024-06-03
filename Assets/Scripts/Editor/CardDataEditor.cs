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

        serializedObject.ApplyModifiedProperties();
    }

    // CardData�� �� �ڽ� Ŭ�������� ������ ��� ������Ƽ�� ����
    private void DrawFields(object targetObject, Type type)
    {
        // �θ� Ŭ������ ������, �θ� Ŭ������ �������� �ڽ� Ŭ������ ���� ������ ����
        if (type.BaseType != null && type.BaseType == typeof(CardData))
        {
            DrawFields(targetObject, type.BaseType);
        }

        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        foreach (FieldInfo field in fields)
        {
            if (field.IsPublic || field.GetCustomAttribute<SerializeField>() != null)
            {
                SerializedProperty property = serializedObject.FindProperty(field.Name);
                if (property != null)
                {
                    EditorGUILayout.PropertyField(property, new GUIContent(field.Name), true);

                    // Ư�� ������Ƽ�� ��ư�� �߰��ϴ� ���� �۾��� �ϱ� ���� �̸����� �����ؼ� ���� �۾�
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
                                    cardData.AddBuff(new BuffData(BUFF_TARGET.PLAYER, null, BUFF_TYPE.STR, -1, false, false));
                                    property.isExpanded = true;
                                }
                                if (GUILayout.Button("Add BuffEnemy", optionButtons))
                                {
                                    cardData.AddBuff(new BuffData(BUFF_TARGET.ENEMY, null, BUFF_TYPE.STR, -1, false, false));
                                    property.isExpanded = true;
                                }
                                GUILayout.EndHorizontal();

                                if (GUILayout.Button("Removel All Buffs"))
                                {
                                    cardData.ClearBuffList();
                                }

                                EditorGUILayout.Space(14);
                            }
                            break;
                        case "featureList":
                            {
                                if (property.isArray && property.arraySize == 0)
                                {
                                    property.isExpanded = false;
                                }

                                EditorGUILayout.Space(6);

                                if (GUILayout.Button("Add FeatureList"))
                                {
                                    cardData.AddFeature(new CardData.CardFeature(CARD_FEATURE.EXHAUST, -1));
                                    property.isExpanded = true;
                                }

                                if (GUILayout.Button("Removel All Features"))
                                {
                                    cardData.ClearFeatureList();
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