using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardAction;
using System;
using System.Reflection;
using UnityEditor;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemCardEditorAction : MonoBehaviour, IPointerClickHandler
{
    private Dictionary<string, ItemCardAction> dictField = new Dictionary<string, ItemCardAction>();
    [SerializeField]
    private RectTransform parent = null;
    [SerializeField]
    private ItemCardAction prefabItemAction = null;
    [SerializeField]
    private GameObject objSelected = null;
    private bool isSelected = false;

    private CardAction.CardAction actionData = null;

    private System.Action actionClicked = null;


    private void Awake()
    {
        
    }

    public void Set(CardAction.CardAction _action)
    {
        actionData = _action;

        FieldInfo[] fields = Utils.GetFields(actionData.GetType());        
        foreach (FieldInfo field in fields)
        {
            object value = field.GetValue(actionData);
            ItemCardAction item = Instantiate(prefabItemAction, parent);
            item.Set(field.Name, value);
            dictField.Add(field.Name, item);
        }
    }

    public CardAction.CardAction GetAction()
    {
        FieldInfo[] fields = Utils.GetFields(actionData.GetType());
        
        foreach (var field in fields)
        {
            foreach (var elem in dictField)
            {
                if (field.Name == elem.Key)
                {
                    field.SetValue(actionData, elem.Value.GetValue());
                    continue;
                }
            }
        }

        return actionData;
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

    public void OnPointerClick(PointerEventData eventData)
    {
        actionClicked?.Invoke();
    }
}
