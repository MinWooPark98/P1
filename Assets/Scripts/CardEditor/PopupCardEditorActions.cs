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
    private ItemCardActionEditor prefabItem = null;

    [SerializeField]
    private TMP_Dropdown dropDownAddAction = null;

    private List<ItemCardActionEditor> listAction = new List<ItemCardActionEditor>();
    private ItemCardActionEditor itemSelected = null;

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
        if (itemSelected != null)
        {
            if (KeyboardManager.Instance.GetKeyDown(this, KeyCode.Delete))
            {
                KeyboardManager.Instance.RemoveObjInputNeed(this);
                listAction.Remove(itemSelected);
                Destroy(itemSelected.gameObject);
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
                ItemCardActionEditor item = Instantiate(prefabItem, scrollRect.content);
                item.Set(cardData.actionList[i]);
                item.SetActionClicked(
                    () =>
                    {
                        SelectAction(item);
                    });
                listAction.Add(item);
            }
        }
    }

    public void SelectAction(ItemCardActionEditor _item)
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

        ItemCardActionEditor item = Instantiate(prefabItem, scrollRect.content);    
        item.Set(action);
        item.SetActionClicked(
            () =>
            {
                SelectAction(item);
            });
        listAction.Add(item);
    }

    public void ButtonSave()
    {
        cardData.actionList.Clear();

        for (int i = 0; i < listAction.Count; i++)
        {
            cardData.actionList.Add(listAction[i].GetAction());
        }

        ButtonClose();
    }
}
