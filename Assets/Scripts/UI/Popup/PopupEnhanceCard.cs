using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PopupEnhanceCard : Popup
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

    private bool isEnhanced = false;

    public bool IsEnhanced() => isEnhanced;

    public void SetCards()
    {
        var deckList = CardManager.Instance.GetDeckList();
        deckList = Utils.ShuffleList(deckList);
        for (int i = 0; i < deckList.Count; i++)
        {
            int index = i;
            ItemCard itemCard = Instantiate(prefabCard, grid.transform);
            itemCard.Set(deckList[i]);
            itemCard.SetState(ItemCard.CARD_STATE.NORMAL);
            itemCard.SetActionClicked(
                () =>
                {
                    PopupEnhanceCardExpected popupEnhanceCardExpected = UIManager.Instance.MakePopup<PopupEnhanceCardExpected>();
                    popupEnhanceCardExpected.Set(itemCard.GetData());
                    popupEnhanceCardExpected.SetActionEnhance(
                        () =>
                        {
                            CardManager.Instance.GetDeckList()[index] = CardManager.Instance.GetAllCardList().Find((card) => card.id == itemCard.GetData().id + 1);
                            isEnhanced = true;
                            ButtonClose();
                        });
                });
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
