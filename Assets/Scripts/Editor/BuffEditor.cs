using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(BuffData))]
public class BuffDataEditor : PropertyDrawer
{
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        EditorGUI.BeginProperty(_position, _label, _property);

        _position = EditorGUI.IndentedRect(_position);
        SerializedProperty endProperty = _property.GetEndProperty();

        SerializedProperty buffTarget = _property.FindPropertyRelative("target");
        SerializedProperty buffType = _property.FindPropertyRelative("type");
        SerializedProperty applySelf = _property.FindPropertyRelative("applySelf");
        SerializedProperty applyAllEnemies = _property.FindPropertyRelative("applyAllEnemies");

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float yOffset = _position.y;

        SerializedProperty currentProperty = _property.Copy();
        currentProperty.NextVisible(true);

        while (!SerializedProperty.EqualContents(currentProperty, endProperty))
        {
            if (currentProperty.propertyPath == buffTarget.propertyPath)
            {
                EditorGUI.PropertyField(new Rect(_position.x, yOffset, _position.width, lineHeight), currentProperty);
                yOffset += lineHeight + EditorGUIUtility.standardVerticalSpacing;

                currentProperty.NextVisible(false);
            }
            else if (currentProperty.propertyPath == buffType.propertyPath)
            {
                BUFF_TYPE[] allBuffTypes = (BUFF_TYPE[])Enum.GetValues(typeof(BUFF_TYPE));
                BUFF_TYPE buffTypeValue = (BUFF_TYPE)buffType.intValue;
                string[] buffTypeOptions = null;
                switch ((BUFF_TARGET)buffTarget.intValue)
                {
                    case BUFF_TARGET.PLAYER:
                        {
                            buffTypeOptions = Array.FindAll(allBuffTypes, (value) => (((int)value >= 11000 && (int)value < 12000) || ((int)value >= 12000 && (int)value < 13000))).Select((value) => value.ToString()).ToArray();
                        }
                        break;
                    case BUFF_TARGET.ENEMY:
                        {
                            buffTypeOptions = Array.FindAll(allBuffTypes, (value) => (((int)value >= 11000 && (int)value < 12000) || ((int)value >= 13000 && (int)value < 14000))).Select((value) => value.ToString()).ToArray();
                        }
                        break;
                }

                int selectedIndex = System.Array.IndexOf(buffTypeOptions, buffTypeValue.ToString());

                if (selectedIndex < 0)
                {
                    selectedIndex = 0;
                }
                selectedIndex = EditorGUI.Popup(new Rect(_position.x, yOffset, _position.width, lineHeight), "type", selectedIndex, buffTypeOptions);
                buffType.enumValueIndex = System.Array.IndexOf(allBuffTypes.Select((value) => value.ToString()).ToArray(), buffTypeOptions[selectedIndex]);

                yOffset += lineHeight + EditorGUIUtility.standardVerticalSpacing;

                currentProperty.NextVisible(false);
            }
            else if (currentProperty.propertyPath == applyAllEnemies.propertyPath &&
                    (buffTarget.intValue == (int)BUFF_TARGET.PLAYER || applySelf.boolValue == true))
            {
                currentProperty.NextVisible(false);
                continue;
            }
            else
            {
                EditorGUI.PropertyField(new Rect(_position.x, yOffset, _position.width, lineHeight), currentProperty, true);
                yOffset += lineHeight + EditorGUIUtility.standardVerticalSpacing;
                currentProperty.NextVisible(false);
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label)
    {
        float totalHeight = 0;
        SerializedProperty endProperty = _property.GetEndProperty();

        SerializedProperty currentProperty = _property.Copy();
        currentProperty.NextVisible(true);

        while (!SerializedProperty.EqualContents(currentProperty, endProperty))
        {
            totalHeight += EditorGUI.GetPropertyHeight(currentProperty, true) + EditorGUIUtility.standardVerticalSpacing;
            currentProperty.NextVisible(false);
        }

        return totalHeight;
    }
}
#endif
