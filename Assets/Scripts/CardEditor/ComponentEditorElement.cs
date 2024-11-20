using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComponentEditorElement : MonoBehaviour
{
    public enum Type
    {
        None = -1,
        Text,
        DropDown,
        InputField,
        Toggle,
        Component,
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
    [SerializeField]
    private GameObject componentValue = null;
    [SerializeField]
    private ComponentEditor prefabComponent = null;
    [SerializeField]
    private Transform parentComponent = null;
    private List<ComponentEditor> listComponent = new List<ComponentEditor>();
    private System.Type typeComponent;

    private ComponentEditor componentSelected = null;

    private Type usingType = Type.None;
    private System.Type valueType = null;


    public void Set(string _name, System.Object _value)
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
                dropDownValue.options.Add(new TMP_Dropdown.OptionData(listTypeName[i]));
            }

            for (int i = 0; i < dropDownValue.options.Count; i++)
            {
                if (dropDownValue.options[i].text == _value.ToString())
                {
                    dropDownValue.value = i;
                    break;
                }
            }
        }
        else if (_value.GetType().IsGenericType)
        {
            if (_value.GetType().GetGenericTypeDefinition() == typeof(List<>))
            {
                Use(Type.Component);
                typeComponent = _value.GetType().GetGenericArguments()[0];          // 제네릭 인수가 1개인 상황만 상정하고 제작
                if (_value is IEnumerable listData && listData != null)
                {
                    foreach (var data in listData)
                    {
                        ComponentEditor newData = Instantiate(prefabComponent, parentComponent);
                        newData.Set(data);
                        newData.SetActionClicked(
                            () =>
                            {
                                SelectComponent(newData);
                            });
                        listComponent.Add(newData);
                    }
                }
            }
        }
    }

    public object GetValue()
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
            case Type.Component:
                System.Type listType = typeof(List<>).MakeGenericType(typeComponent);
                value = Activator.CreateInstance(listType);
                var addMethod = listType.GetMethod("Add");
                addMethod.Invoke(value, listComponent.Select((component) => component.GetData()).ToArray());
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
            case Type.Component:
                componentValue.gameObject.SetActive(true);
                break;
        }
    }

    private void Update()
    {
        if (componentSelected != null)
        {
            if (KeyboardManager.Instance.GetKeyDown(this, KeyCode.Delete))
            {
                KeyboardManager.Instance.RemoveObjInputNeed(this);
                listComponent.Remove(componentSelected);
                Destroy(componentSelected.gameObject);
                SelectComponent(null);
            }
        }
    }

    public void SelectComponent(ComponentEditor _component)
    {
        if (componentSelected != null)
        {
            KeyboardManager.Instance.RemoveObjInputNeed(this);
            componentSelected.Deselect();
        }

        componentSelected = componentSelected == _component ? null : _component;

        if (componentSelected != null)
        {
            KeyboardManager.Instance.AddObjInputNeed(this);
            componentSelected.Select();
        }
    }

    public void ButtonAddComponent()
    {
        if (usingType != Type.Component)
        {
            return;
        }

        ComponentEditor newComponent = Instantiate(prefabComponent, parentComponent);
        newComponent.Set(Activator.CreateInstance(typeComponent));
        newComponent.SetActionClicked(
            () =>
            {
                SelectComponent(newComponent);
            });
        listComponent.Add(newComponent);
    }
}
