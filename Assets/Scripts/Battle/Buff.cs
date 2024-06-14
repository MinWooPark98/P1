using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;


[Serializable]
public struct BuffData
{
    [ReadOnly]
    public BUFF_TARGET target;
    public string iconName;
    public BUFF_TYPE type;
    public int value;
    public bool applySelf;
    public bool applyAllEnemies;                    // �÷��̾ �� ��ü���� ������ ���ΰ� (���� ���ο��Ը� ���� ���� ����)

    /// <summary>
    /// ����Ƽ �����Ϳ��� ���� ����� ���� �ϱ� ���� �뵵�� ������, �� �ܿ��� ������� ����
    /// </summary>
    /// <param name="_target"></param>
    /// <param name="_type"></param>
    /// <param name="_value"></param>
    /// <param name="_applySelf"></param>
    /// <param name="_iconName"></param>
    public BuffData(BUFF_TARGET _target, string _iconName, BUFF_TYPE _type, int _value, bool _applySelf, bool _applyAllEnemies)
    {
        target = _target;
        iconName = _iconName;
        type = _type;
        value = _value;
        applySelf = _applySelf;
        applyAllEnemies = _applyAllEnemies;
    }
}

//[Serializable]
//public abstract class Buff
//{
//    [SerializeField]
//    public string iconName;
//    //[SerializeField]
//    public int value;
//}


// ���� ����/�����
//public class BuffPublic : Buff
//{
//    //[SerializeField]
//    public BUFF_PUBLIC type;
//    //[SerializeField]
//    public bool applySelf;                 // ���ο��� �����ϴ��� (true: ����, false: ��)

//    public BuffPublic(BUFF_PUBLIC _type, int _value, bool _applySelf, string _iconName)
//    {
//        this.type = _type;
//        this.value = _value;
//        this.applySelf = _applySelf;
//        this.iconName = _iconName;
//    }
//}


//// �÷��̾� ����/�����
//public class BuffPlayer : Buff
//{
//    //[SerializeField]
//    public BUFF_PLAYER type;

//    public BuffPlayer(BUFF_PLAYER _type, int _value, string _iconName)
//    {
//        this.type = _type;
//        this.value = _value;
//        this.iconName = _iconName;
//    }
//}


//// �� ����/�����
//public class BuffEnemy : Buff
//{
//    //[SerializeField]
//    public BUFF_ENEMY type;

//    public BuffEnemy(BUFF_ENEMY _type, int _value, string _iconName)
//    {
//        this.type = _type;
//        this.value = _value;
//        this.iconName = _iconName;
//    }
//}


//public struct Buf
//{
//    [Serializable]
//    public enum BufType
//    {
//        None,
//        Test,
//    }

//    [SerializeField]
//    public BufType type;
//    [SerializeField]
//    public string iconName;
//    //[SerializeField]
//    public int value;
//}

//[CustomEditor(typeof(Buf)), CanEditMultipleObjects]
//public class BufEditor : Editor
//{
//    private Buf buf;

//    public override void OnInspectorGUI()
//    {
//        serializedObject.Update();

//        DrawFields(serializedObject.targetObject, serializedObject.targetObject.GetType());

//        serializedObject.ApplyModifiedProperties();
//    }
//    private void DrawFields(object targetObject, Type type)
//    {
//        // �θ� Ŭ������ ������, �θ� Ŭ������ �������� �ڽ� Ŭ������ ���� ������ ����
//        if (type.BaseType != null && type.BaseType == typeof(CardData))
//        {
//            DrawFields(targetObject, type.BaseType);
//        }

//        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
//        foreach (FieldInfo field in fields)
//        {
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
//                                EditorGUILayout.Space(6);

//                                GUILayout.BeginHorizontal();
//                                GUILayoutOption[] optionButtons = new[] { GUILayout.ExpandWidth(true) };
//                                if (GUILayout.Button("Add BuffPublic", optionButtons))
//                                {
//                                    cardData.AddBuff(new BuffPublic(BUFF_PUBLIC.STR, -1, false, null));
//                                    property.isExpanded = true;
//                                }
//                                if (GUILayout.Button("Add BuffPlayer", optionButtons))
//                                {
//                                    cardData.AddBuff(new BuffPlayer(BUFF_PLAYER.CONFUSED, -1, null));
//                                    property.isExpanded = true;
//                                }
//                                if (GUILayout.Button("Add BuffEnemy", optionButtons))
//                                {
//                                    cardData.AddBuff(new BuffEnemy(BUFF_ENEMY.testEnemy2, -1, null));
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
//}
