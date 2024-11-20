using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using CardAction;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;

public class PopupCardEditorActions : Popup
{
    [SerializeField]
    private ScrollRect scrollRect = null;
    [SerializeField]
    private ComponentEditor prefabComponent = null;

    [SerializeField]
    private TMP_Dropdown dropDownAddAction = null;

    private List<ComponentEditor> listComponentAction = new List<ComponentEditor>();
    private ComponentEditor componentSelected = null;

    private CardData cardData = null;



    protected override void Awake()
    {
        dropDownAddAction.options.Clear();

        string[] listActionType = Enum.GetNames(typeof(CardAction.CardAction.ActionType));
        for (int i = 0; i < listActionType.Length; i++)
        {
            CardAction.CardAction.ActionType actionType = CardAction.CardAction.ActionType.None;
            if (Enum.TryParse(listActionType[i], out actionType))
            {
                if (actionType != CardAction.CardAction.ActionType.None)
                {
                    dropDownAddAction.options.Add(new TMP_Dropdown.OptionData(listActionType[i]));
                }
            }
        }

        base.Awake();
    }

    protected override void OnDestroy()
    {
        KeyboardManager.Instance.RemoveObjInputNeed(this);
        base.OnDestroy();
    }

    protected override void Update()
    {
        if (componentSelected != null)
        {
            if (KeyboardManager.Instance.GetKeyDown(this, KeyCode.Delete))
            {
                KeyboardManager.Instance.RemoveObjInputNeed(this);
                listComponentAction.Remove(componentSelected);
                Destroy(componentSelected.gameObject);
                SelectAction(null);
            }
        }
        base.Update();
    }


    public void Set(CardData _cardData)
    {
        cardData = _cardData;

        if (cardData.actionList != null)
        {
            for (int i = 0; i < cardData.actionList.Count; i++)
            {
                ComponentEditor component = Instantiate(prefabComponent, scrollRect.content);
                component.Set(cardData.actionList[i]);
                component.SetActionClicked(
                    () =>
                    {
                        SelectAction(component);
                    });
                listComponentAction.Add(component);
            }
        }
    }

    public void SelectAction(ComponentEditor _component)
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

    public void ButtonAddAction()
    {
        CardAction.CardAction.ActionType actionType = CardAction.CardAction.ActionType.None;
        if (Enum.TryParse(dropDownAddAction.options[dropDownAddAction.value].text, out actionType) == false)
        {
            return;
        }

        if (actionType == CardAction.CardAction.ActionType.None)
        {
            return;
        }

        CardAction.CardAction action = null;
        switch (actionType)
        {
            case CardAction.CardAction.ActionType.SingleAttack:
                action = new SingleAttack();
                break;
            case CardAction.CardAction.ActionType.MultipleAttack:
                action = new MultipleAttack();
                break;
            case CardAction.CardAction.ActionType.Defense:
                action = new Defense();
                break;
            case CardAction.CardAction.ActionType.DrawCard:
                action = new DrawCard();
                break;
            default:
                break;
        }

        if (action == null)
        {
            return;
        }

        action.actionType = actionType;

        ComponentEditor component = Instantiate(prefabComponent, scrollRect.content);
        component.Set(action);
        component.SetActionClicked(
            () =>
            {
                SelectAction(component);
            });
        listComponentAction.Add(component);
    }

    public void ButtonSave()
    {
        cardData.actionList.Clear();

        for (int i = 0; i < listComponentAction.Count; i++)
        {
            cardData.actionList.Add((CardAction.CardAction)listComponentAction[i].GetData());
        }

        ButtonClose();
    }
}
