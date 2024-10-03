//using UnityEngine;
//using UnityEditor;
//using System.Reflection;
//using System;

//[CustomEditor(typeof(CardData), true), CanEditMultipleObjects]
//public class CardDataEditor : Editor
//{
//    private CardData cardData;

//    private void OnEnable()
//    {
//        cardData = target as CardData;
//    }

//    public override void OnInspectorGUI()
//    {
//        serializedObject.Update();

//        DrawFields(serializedObject.targetObject, serializedObject.targetObject.GetType());

//        serializedObject.ApplyModifiedProperties();
//    }

//    // CardData�� �� �ڽ� Ŭ�������� ������ ��� ������Ƽ�� ����
//    private void DrawFields(object _targetObject, Type _type)
//    {
//        // �θ� Ŭ������ ������, �θ� Ŭ������ �������� �ڽ� Ŭ������ ���� ������ ����
//        if (_type.BaseType != null && _type.BaseType == typeof(CardData))
//        {
//            DrawFields(_targetObject, _type.BaseType);
//        }

//        FieldInfo[] fields = _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
//        foreach (FieldInfo field in fields)
//        {
//            if (field.Name == "actionList")
//            {
//                EditorGUILayout.Space(20);

//                EditorGUILayout.LabelField("actionList", EditorStyles.boldLabel);

//                EditorGUILayout.Space(6);

//                if (cardData.actionList != null)
//                {
//                    for (int i = 0; i < cardData.actionList.Count; i++)
//                    {
//                        GUILayout.BeginVertical(GUI.skin.box);
//                        cardData.actionList[i] = DrawCardActions(cardData.actionList[i]);
//                        if (GUILayout.Button("Remove"))
//                        {
//                            cardData.actionList.RemoveAt(i);
//                        }
//                        GUILayout.EndVertical();
//                    }
//                }

//                EditorGUILayout.Space(6);

//                GUILayout.BeginHorizontal();

//                GUILayoutOption[] optionButtons = new[] { GUILayout.ExpandWidth(true) };
//                CardAction.ICardAction.ActionType type = CardAction.ICardAction.ActionType.SingleAttack;
//                type = (CardAction.ICardAction.ActionType)EditorGUILayout.EnumPopup(type, optionButtons);

//                if (GUILayout.Button("Add CardAction", optionButtons))
//                {
//                    cardData.AddAction(type);
//                }
//                GUILayout.EndHorizontal();

//                if (GUILayout.Button("Removel All CardActions"))
//                {
//                    cardData.ClearActionList();
//                }

//                EditorGUILayout.Space(14);

//                continue;
//            }

//            if (field.IsPublic || field.GetCustomAttribute<SerializeField>() != null)
//            {
//                SerializedProperty property = serializedObject.FindProperty(field.Name);
//                if (property != null)
//                {
//                    EditorGUILayout.PropertyField(property, new GUIContent(field.Name), true);

//                    // Ư�� ������Ƽ�� ��ư�� �߰��ϴ� ���� �۾��� �ϱ� ���� �̸����� �����ؼ� ���� �۾�
//                    switch (property.name)
//                    {
//                        case "buffList":
//                            {
//                                if (property.isArray && property.arraySize == 0)
//                                {
//                                    property.isExpanded = false;
//                                }

//                                EditorGUILayout.Space(6);

//                                GUILayout.BeginHorizontal();
//                                GUILayoutOption[] optionButtons = new[] { GUILayout.ExpandWidth(true) };
                                
//                                if (GUILayout.Button("Add BuffPlayer", optionButtons))
//                                {
//                                    cardData.AddBuff(new BuffData(BUFF_TARGET.Player, null, BUFF_TYPE.Artifact, -1, false, false));
//                                    property.isExpanded = true;
//                                }
//                                if (GUILayout.Button("Add BuffEnemy", optionButtons))
//                                {
//                                    cardData.AddBuff(new BuffData(BUFF_TARGET.Enemy, null, BUFF_TYPE.Artifact, -1, false, false));
//                                    property.isExpanded = true;
//                                }
//                                GUILayout.EndHorizontal();

//                                if (GUILayout.Button("Removel All Buffs"))
//                                {
//                                    cardData.ClearBuffList();
//                                }

//                                EditorGUILayout.Space(14);
//                            }
//                            break;
//                        case "featureList":
//                            {
//                                if (property.isArray && property.arraySize == 0)
//                                {
//                                    property.isExpanded = false;
//                                }

//                                EditorGUILayout.Space(6);

//                                if (GUILayout.Button("Add FeatureList"))
//                                {
//                                    cardData.AddFeature(new CardData.CardFeature(CARD_FEATURE.EXHAUST, -1));
//                                    property.isExpanded = true;
//                                }

//                                if (GUILayout.Button("Removel All Features"))
//                                {
//                                    cardData.ClearFeatureList();
//                                }

//                                EditorGUILayout.Space(14);
//                            }
//                            break;
//                        default:
//                            break;
//                    }
//                }
//            }
//        }
//    }

//    private T DrawCardActions<T>(T _action) where T : CardAction.ICardAction
//    {
//        FieldInfo[] fields = _action.GetType().GetFields();
//        foreach (FieldInfo field in fields)
//        {
//            object value = field.GetValue(_action);
//            if (value is int intValue)
//            {
//                intValue = EditorGUILayout.IntField(field.Name, intValue);
//                field.SetValue(_action, intValue);
//            }
//            else if (value is float floatValue)
//            {
//                floatValue = EditorGUILayout.FloatField(field.Name, floatValue);
//                field.SetValue(_action, floatValue);
//            }
//            else if (value is string stringValue)
//            {
//                stringValue = EditorGUILayout.TextField(field.Name, stringValue);
//                field.SetValue(_action, stringValue);
//            }
//            else if (value is bool boolValue)
//            {
//                boolValue = EditorGUILayout.Toggle(field.Name, boolValue);
//                field.SetValue(_action, boolValue);
//            }
//            else if (value is CardAction.ICardAction.TargetType targetTypeValue)
//            {
//                targetTypeValue = (CardAction.ICardAction.TargetType)EditorGUILayout.EnumPopup(field.Name, targetTypeValue);
//                field.SetValue(_action, targetTypeValue);
//            }
//        }

//        return _action;
//    }
//}
