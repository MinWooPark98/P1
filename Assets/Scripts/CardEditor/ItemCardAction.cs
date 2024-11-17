using CardAction;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCardAction : MonoBehaviour
{
    public enum Type
    {
        None = -1,
        Text,
        DropDown,
        InputField,
        Toggle,
    }

    [SerializeField]
    private TMP_Text textName = null;

    [SerializeField]
    private TMP_Text textValue = null;
    [SerializeField]
    private TMP_Dropdown dropDownValue = null;
    [SerializeField]
    private TMP_InputField inputFieldValue = null;
    [SerializeField]
    private Toggle toggleValue = null;

    private Type usingType = Type.None;
    private System.Type valueType = null;
    

    public void Set(string _name, System.Object _value)
    {
        textName.text = _name;

        // 임시 조치 (현재 Buffs를 에디터 씬에서 설정하지 못하는데, 이 때 빈 List가 오면 중간에 null로 바뀜)
        if (_value == null)
        {
            return;
        }

        valueType = _value.GetType();

        if (valueType == typeof(CardAction.CardAction.ActionType))
        {
            Use(Type.Text);
            textValue.text = _value.ToString();
            return;
        }

        if (_value is int intValue)
        {
            Use(Type.InputField);
            inputFieldValue.text = intValue.ToString();
            inputFieldValue.onValueChanged.AddListener((text) => inputFieldValue.text = Utils.GetIntFromText(text));
        }
        else if (_value is float floatValue)
        {
            Use(Type.InputField);
            inputFieldValue.text = floatValue.ToString();
        }
        else if (_value is string stringValue)
        {
            Use(Type.InputField);
            inputFieldValue.text = stringValue;
        }
        else if (_value is bool boolValue)
        {
            Use(Type.Toggle);
            toggleValue.isOn = boolValue;
            inputFieldValue.onValueChanged.AddListener((text) => inputFieldValue.text = Utils.GetIntFromText(text));
        }
        else if (_value is Enum enumValue)
        {
            Use(Type.DropDown);
            dropDownValue.options.Clear();
            bool isNone = false;
            foreach (var value in Enum.GetValues(valueType))
            {
                if ((int)value == -1)
                {
                    isNone = true;
                    break;
                }
            }
            string[] listTypeName = Enum.GetNames(valueType);
            for (int i = 0; i < listTypeName.Length; i++)
            {
                if (isNone)
                {
                    if ((int)Enum.Parse(valueType, listTypeName[i]) == -1)
                    {
                        continue;
                    }
                }
                dropDownValue.options.Add(new TMP_Dropdown.OptionData(listTypeName[i]));
            }
            dropDownValue.value = (int)_value;
        }
    }

    public System.Object GetValue()
    {
        System.Object value = null;

        switch (usingType)
        {
            case Type.Text:
                value = Parse(textValue.text);
                break;
            case Type.DropDown:
                value = dropDownValue.value;
                break;
            case Type.InputField:
                value = Parse(inputFieldValue.text);
                break;
            case Type.Toggle:
                value = toggleValue.isOn;
                break;
            default:
                return null;
        }

        return value;
    }

    private System.Object Parse(string _string)
    {
        if (valueType == typeof(int))
        {
            return Int32.Parse(_string);
        }
        
        if (valueType == typeof(float))
        {
            return float.Parse(_string);
        }
        
        if (valueType == typeof(string))
        {
            return _string;
        }
        
        if (valueType == typeof(bool))
        {
            bool.Parse(_string);
        }
        
        if (valueType == typeof(Enum))
        {
            return Enum.Parse(valueType, _string);
        }

        return null;
    }

    private void Use(Type _type)
    {
        usingType = _type;
        switch (_type)
        {
            case Type.Text:
                textValue.transform.parent.gameObject.SetActive(true);
                break;
            case Type.DropDown:
                dropDownValue.gameObject.SetActive(true);
                break;
            case Type.InputField:
                inputFieldValue.gameObject.SetActive(true);
                break;
            case Type.Toggle:
                toggleValue.gameObject.SetActive(true);
                break;
        }
    }
}
