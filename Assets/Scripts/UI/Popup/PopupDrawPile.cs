using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupDrawPile : Popup
{
    [SerializeField]
    private ItemCard prefabCard = null;

    [SerializeField]
    private Transform parentCards = null;

    private List<ItemCard> cardList = new List<ItemCard>();

    public void SetCards(List<CardData> _cardDataList)
    {
        _cardDataList = Utils.ShuffleList(_cardDataList);
        for (int i = 0; i < _cardDataList.Count; i++)
        {
            ItemCard itemCard = Instantiate(prefabCard, parentCards);
            itemCard.Set(_cardDataList[i]);
            cardList.Add(itemCard);
        }
    }

    protected override void Update()
    {
        for (int i = 0; i < cardList.Count; i++)
        {
            cardList[i].transform.localScale = i == GetIndexCardLookingAt() ? Vector3.one : Vector3.one * 0.8f;
        }

        base.Update();
    }

    private int GetIndexCardLookingAt()
    {
        for (int i = 0; i < cardList.Count; i++)
        {
            if (cardList[i].GetLookingAt())
            {
                return i;
            }
        }

        return -1;
    }
}
