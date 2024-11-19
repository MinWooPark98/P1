using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemBuffEditor : MonoBehaviour
{
    private Dictionary<string, ItemBuffEditorElement> dictField = new Dictionary<string, ItemBuffEditorElement>();
    [SerializeField]
    private RectTransform parent = null;
    [SerializeField]
    private ItemBuffEditorElement prefabElement = null;
    [SerializeField]
    private GameObject objSelected = null;
    private bool isSelected = false;

    private BuffData data;

    private System.Action actionClicked = null;

    public void Set(BuffData _data)
    {
        data = _data;
        Utils.DeleteChild(parent);
        dictField.Clear();

        FieldInfo[] fields = Utils.GetFields(data.GetType());
        foreach (FieldInfo field in fields)
        {
            object value = field.GetValue(data);
            ItemBuffEditorElement item = Instantiate(prefabElement, parent);
            item.Set(field.Name, value, data.target, () => { Set(GetData()); });
            dictField.Add(field.Name, item);
        }
    }

    public BuffData GetData()
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
        data = (BuffData)dataBoxed;
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

    public void ButtonItem()
    {
        actionClicked?.Invoke();
    }
}
