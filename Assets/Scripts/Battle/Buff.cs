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
    public bool applyAllEnemies;                    // 플레이어가 적 전체에게 적용할 것인가 (적은 본인에게만 버프 적용 가능)

    /// <summary>
    /// 유니티 에디터에서 버프 만들기 쉽게 하기 위한 용도의 생성자, 이 외에는 사용하지 않음
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


// 공용 버프/디버프
//public class BuffPublic : Buff
//{
//    //[SerializeField]
//    public BUFF_PUBLIC type;
//    //[SerializeField]
//    public bool applySelf;                 // 본인에게 적용하는지 (true: 본인, false: 적)

//    public BuffPublic(BUFF_PUBLIC _type, int _value, bool _applySelf, string _iconName)
//    {
//        this.type = _type;
//        this.value = _value;
//        this.applySelf = _applySelf;
//        this.iconName = _iconName;
//    }
//}


//// 플레이어 버프/디버프
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


//// 적 버프/디버프
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
//        // 부모 클래스가 있으면, 부모 클래스의 변수부터 자식 클래스의 변수 순서로 진행
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

//                    // 특정 프로퍼티만 버튼을 추가하는 등의 작업을 하기 위해 이름으로 구분해서 별도 작업
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
