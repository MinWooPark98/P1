using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PopupCardEditorMenu : Popup
{
    [SerializeField]
    private ScrollRect scrollRect = null;
    [SerializeField]
    private ItemCard prefabItemCard = null;


    private List<ItemCard> cardList = new List<ItemCard>();


    protected override void Awake()
    {
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

        var allCardList = Resources.LoadAll<CardData>("Scriptables/CardData").ToList();
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
                });
        }
    }
}
