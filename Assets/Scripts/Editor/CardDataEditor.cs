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

    // CardData와 그 자식 클래스들의 변수를 모두 프로퍼티로 만듬
    private void DrawFields(object _targetObject, Type _type)
    {
        // 부모 클래스가 있으면, 부모 클래스의 변수부터 자식 클래스의 변수 순서로 진행
        if (_type.BaseType != null && _type.BaseType == typeof(CardData))
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
                                    cardData.AddBuff(new BuffData(BUFF_TARGET.Player, null, BUFF_TYPE.Str, -1, false, false));
                                    property.isExpanded = true;
                                }
                                if (GUILayout.Button("Add BuffEnemy", optionButtons))
                                {
                                    cardData.AddBuff(new BuffData(BUFF_TARGET.Enemy, null, BUFF_TYPE.Str, -1, false, false));
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
