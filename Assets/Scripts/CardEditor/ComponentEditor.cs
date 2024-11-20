using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ComponentEditor : MonoBehaviour
{
    private Dictionary<string, ComponentEditorElement> dictField = new Dictionary<string, ComponentEditorElement>();
    [SerializeField]
    private RectTransform parent = null;
    [SerializeField]
    private ComponentEditorElement prefabElement = null;
    [SerializeField]
    private GameObject objSelected = null;
    private bool isSelected = false;

    private object data;

    private System.Action actionClicked = null;

    public void Set(object _data)
    {
        data = _data;

        dictField.Clear();

        FieldInfo[] fields = Utils.GetFields(data.GetType());
        foreach (FieldInfo field in fields)
        {
            object value = field.GetValue(data);
            ComponentEditorElement item = Instantiate(prefabElement, parent);
            item.Set(field.Name, value);
            dictField.Add(field.Name, item);
        }
    }

    public object GetData()
    {
        FieldInfo[] fields = Utils.GetFields(data.GetType());

        object dataBoxed = data;
        foreach (var field in fields)
        {
            foreach (var elem in dictField)
            {
                if (field.Name == elem.Key)
                {
                    field.SetValue(dataBoxed, elem.Value.GetValue());
                    continue;
                }
            }
        }
        return data;
    }

    public void SetActionClicked(System.Action _actionClicked)
    {
        actionClicked = _actionClicked;
    }

    public void Select()
    {
        isSelected = true;
        objSelected.SetActive(true);
    }

    public void Deselect()
    {
        isSelected = false;
        objSelected.SetActive(false);
    }

    public void ButtonClick()
    {
        actionClicked?.Invoke();
    }
}
