using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupExhaustPile : Popup
{
    [SerializeField]
    private ItemCard prefabCard = null;

    [SerializeField]
    private GridLayoutGroup grid = null;

    private List<ItemCard> cardList = new List<ItemCard>();

    [SerializeField]
    private float CARD_SCALE = 0.75f;
    [SerializeField]
    private Vector2 extraSpacing = new Vector2(20f, 20f);

    public void SetCards(List<CardData> _cardDataList)
    {
        _cardDataList = Utils.ShuffleList(_cardDataList);
        for (int i = 0; i < _cardDataList.Count; i++)
        {
            ItemCard itemCard = Instantiate(prefabCard, grid.transform);
            itemCard.Set(_cardDataList[i]);
            itemCard.SetState(ItemCard.CARD_STATE.NORMAL);
            cardList.Add(itemCard);
        }
    }

    protected override void Awake()
    {
        grid.cellSize = CARD_SCALE * new Vector2(StaticData.CARD_SIZE_WIDTH, StaticData.CARD_SIZE_HEIGHT);
        grid.spacing = (1 - CARD_SCALE) * 0.5f * new Vector2(StaticData.CARD_SIZE_WIDTH, StaticData.CARD_SIZE_HEIGHT) + extraSpacing;
    }

    protected override void Update()
    {
        for (int i = 0; i < cardList.Count; i++)
        {
            cardList[i].transform.localScale = i == GetIndexCardLookingAt() ? Vector3.one : Vector3.one * CARD_SCALE;
        }

        base.Update();
    }

    private int GetIndexCardLookingAt()
    {
        for (int i = 0; i < cardList.Count; i++)
        {
            if (cardList[i].GetLookingAt() == true)
            {
                return i;
            }
        }

        return -1;
    }
}
