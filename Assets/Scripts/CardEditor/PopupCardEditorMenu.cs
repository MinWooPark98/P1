using CardAction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CardAction.CardAction;

public class PopupCardEditorMenu : Popup
{
    [SerializeField]
    private ScrollRect scrollRect = null;
    [SerializeField]
    private ItemCard prefabItemCard = null;

    [SerializeField]
    private TMP_Dropdown dropDownCardType = null;


    private List<ItemCard> cardList = new List<ItemCard>();


    protected override void Awake()
    {
        dropDownCardType.options.Clear();
        string[] listCardType = Enum.GetNames(typeof(CARD_TYPE));
        for (int i = 0; i < listCardType.Length; i++)
        {
            CARD_TYPE cardType = CARD_TYPE.NONE;
            if (Enum.TryParse(listCardType[i], out cardType))
            {
                if (cardType != CARD_TYPE.NONE)
                {
                    dropDownCardType.options.Add(new TMP_Dropdown.OptionData(listCardType[i]));
                }
            }
        }

        UpdateCards();

        base.Awake();
    }

    protected override void Update()
    {
        for (int i = 0; i < cardList.Count; i++)
        {
            cardList[i].transform.localScale = (cardList[i].GetLookingAt() ? 1f : 0.8f) * Vector3.one;
        }
        base.Update();
    }

    private void UpdateCards()
    {
        for (int i = 0; i < cardList.Count; i++)
        {
            Destroy(cardList[i].gameObject);
        }
        cardList.Clear();

        var allCardList = CardManager.Instance.GetAllCardList();
        for (int i = 0; i < allCardList.Count; i++)
        {
            ItemCard item = Instantiate(prefabItemCard, scrollRect.content);
            item.Set(allCardList[i]);
            item.transform.localScale = 0.8f * Vector3.one;
            item.SetState(ItemCard.CARD_STATE.NORMAL);
            item.SetActionClicked(
                () =>
                {
                    PopupCardEditor popupCardEditor = UIManager.Instance.MakePopup<PopupCardEditor>();
                    popupCardEditor.Set(item.GetData());
                    popupCardEditor.SetAction(UpdateCards);
                });
            cardList.Add(item);
        }
    }

    public void ButtonNewCard()
    {
        if (Enum.IsDefined(typeof(CARD_TYPE), dropDownCardType.value) == false)
        {
            LogManager.Log("잘못된 카드 타입");
            return;
        }

        CardData newCard = new CardData();
        newCard.type = (CARD_TYPE)dropDownCardType.value;
        CardManager.Instance.AddNewCard(newCard);
        ItemCard item = Instantiate(prefabItemCard, scrollRect.content);
        item.Set(newCard);
        item.transform.localScale = 0.8f * Vector3.one;
        item.SetState(ItemCard.CARD_STATE.NORMAL);
        item.SetActionClicked(
            () =>
            {
                PopupCardEditor popupCardEditor = UIManager.Instance.MakePopup<PopupCardEditor>();
                popupCardEditor.Set(item.GetData());
                popupCardEditor.SetAction(UpdateCards);
            });
        cardList.Add(item);
        StartCoroutine(routine_Reposition());
    }

    private IEnumerator routine_Reposition()
    {
        yield return null;
        scrollRect.normalizedPosition = new Vector2(scrollRect.normalizedPosition.x, 0f);
    }
}
