using CardAction;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCardActionEditorElement : MonoBehaviour
{
    public enum Type
    {
        None = -1,
        Text,
        DropDown,
        InputField,
        Toggle,
        BuffEditor,
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
    private GameObject buffValue = null;
    [SerializeField]
    private Transform parentBuffItem = null;
    [SerializeField]
    private ItemBuffEditor prefabBuffItem = null;
    private List<ItemBuffEditor> listItemBuff = new List<ItemBuffEditor>();

    private ItemBuffEditor itemSelected = null;

    private Type usingType = Type.None;
    private System.Type valueType = null;


    private void OnDestroy()
    {
        KeyboardManager.Instance.RemoveObjInputNeed(this);
    }
    
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

            for (int i = 0; i < dropDownValue.options.Count; i++)
            {
                if (dropDownValue.options[i].text == _value.ToString())
                {
                    dropDownValue.value = i;
                    break;
                }
            }
        }
        else if (_value is List<BuffData> listBuff)
        {
            Use(Type.BuffEditor);
            if (listBuff.Count > 0)
            {
                for (int i = 0; i < listBuff.Count; i++)
                {
                    ItemBuffEditor newBuff = Instantiate(prefabBuffItem, parentBuffItem);
                    newBuff.Set(listBuff[i]);
                    newBuff.SetActionClicked(
                        () =>
                        {
                            SelectItem(newBuff);
                        });
                    listItemBuff.Add(newBuff);
                }
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
                value = dropDownValue.value;
                break;
            case Type.InputField:
                value = Parse(inputFieldValue.text);
                break;
            case Type.Toggle:
                value = toggleValue.isOn;
                break;
            case Type.BuffEditor:
                List<BuffData> list = new List<BuffData>();
                for (int i = 0; i < listItemBuff.Count; i++)
                {
                    list.Add(listItemBuff[i].GetData());
                }
                value = list;
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
            case Type.BuffEditor:
                buffValue.gameObject.SetActive(true);
                break;
        }
    }

    private void Update()
    {
        if (itemSelected != null)
        {
            if (KeyboardManager.Instance.GetKeyDown(this, KeyCode.Delete))
            {
                KeyboardManager.Instance.RemoveObjInputNeed(this);
                listItemBuff.Remove(itemSelected);
                Destroy(itemSelected.gameObject);
                SelectItem(null);
            }
        }
    }

    public void SelectItem(ItemBuffEditor _item)
    {
        if (itemSelected != null)
        {
            KeyboardManager.Instance.RemoveObjInputNeed(this);
            itemSelected.Deselect();
        }

        itemSelected = itemSelected == _item ? null : _item;

        if (itemSelected != null)
        {
            KeyboardManager.Instance.AddObjInputNeed(this);
            itemSelected.Select();
        }
    }

    public void ButtonAddBuff()
    {
        if (usingType != Type.BuffEditor)
        {
            return;
        }

        ItemBuffEditor newBuff = Instantiate(prefabBuffItem, parentBuffItem);
        newBuff.Set(new BuffData());
        newBuff.SetActionClicked(
            () =>
            {
                SelectItem(newBuff);
            });
        listItemBuff.Add(newBuff);
    }
}
