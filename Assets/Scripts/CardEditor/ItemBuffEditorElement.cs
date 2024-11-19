using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemBuffEditorElement : MonoBehaviour
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
    [SerializeField]
    private TMP_Text toggleText = null;

    private Type usingType = Type.None;
    private System.Type valueType = null;


    public void Set(string _name, System.Object _value, BUFF_TARGET _target, System.Action _actionTargetChanged)
    {
        textName.text = _name;

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
            toggleText.text = boolValue.ToString();
            toggleValue.onValueChanged.AddListener((bValue) => toggleText.text = bValue.ToString());
        }
        else if (_value is Enum enumValue)
        {
            Use(Type.DropDown);
            dropDownValue.options.Clear();
            string[] listTypeName = Enum.GetNames(valueType);
            for (int i = 0; i < listTypeName.Length; i++)
            {
                int parse = (int)Enum.Parse(valueType, listTypeName[i]);
                if (parse == -1)
                {
                    continue;
                }

                if (valueType == typeof(BUFF_TYPE))
                {
                    if (_target == BUFF_TARGET.Player)
                    {
                        if (parse >= 13000 && parse < 14000)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (parse >= 12000 && parse < 13000)
                        {
                            continue;
                        }
                    }
                }
                dropDownValue.options.Add(new TMP_Dropdown.OptionData(listTypeName[i]));
            }

            if (valueType == typeof(BUFF_TYPE))
            {
                BUFF_TYPE type = (BUFF_TYPE)_value;
                if (_target == BUFF_TARGET.Player)
                {
                    if ((int)_value >= 13000 && (int)_value < 14000)
                    {
                        _value = BUFF_TYPE.Artifact;
                    }
                }
                else
                {
                    if ((int)_value >= 12000 && (int)_value < 13000)
                    {
                        _value = BUFF_TYPE.Artifact;
                    }
                }
            }

            for (int i = 0; i < dropDownValue.options.Count; i++)
            {
                if (dropDownValue.options[i].text == _value.ToString())
                {
                    dropDownValue.value = i;
                    break;
                }
            }

            if (valueType == typeof(BUFF_TARGET))
            {
                dropDownValue.onValueChanged.AddListener(
                    (value) =>
                    {
                        _actionTargetChanged?.Invoke();
                    });
            }
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
                value = Parse(dropDownValue.options[dropDownValue.value].text);
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
            
        if (valueType.IsEnum)
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
