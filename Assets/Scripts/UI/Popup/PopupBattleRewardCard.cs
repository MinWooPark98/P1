using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupBattleRewardCard : Popup
{
    [SerializeField]
    private GridLayoutGroup grid = null;

    [SerializeField]
    private ItemCard prefabCard = null;

    private List<ItemCard> cardList = new List<ItemCard>();
    private float CARD_SCALE = 1f;

    protected override void Awake()
    {
        SetOptions();
        base.Awake();
    }

    protected override void Start()
    {
        KeyboardManager.Instance.AddActionEscape(ButtonClose);
        base.Start();
    }

    protected override void OnDestroy()
    {
        KeyboardManager.Instance.RemoveActionEscape(ButtonClose);
        base.OnDestroy();
    }

    protected override void Update()
    {
        for (int i = 0; i < cardList.Count; i++)
        {
            cardList[i].transform.localScale = (i == GetIndexCardLookingAt() ? 1.2f : 1f) * CARD_SCALE * Vector3.one;
        }
        base.Update();
    }

    public void SetOptions()
    {
        var cardRewardList = PlayerDataManager.Instance.GetCardRewardList();
        CARD_SCALE = cardRewardList.Count == 3 ? 1f : 0.8f;
        grid.cellSize = CARD_SCALE * new Vector2(StaticData.CARD_SIZE_WIDTH, StaticData.CARD_SIZE_HEIGHT);
        grid.spacing = CARD_SCALE * new Vector2(160f, 0f);
        for (int i = 0; i < cardRewardList.Count; i++)
        {
            ItemCard card = Instantiate(prefabCard, grid.transform);
            card.Set(CardManager.Instance.GetCardData(cardRewardList[i]));
            card.SetState(ItemCard.CARD_STATE.NORMAL);
            card.SetActionClicked(
                () =>
                {
                    CardData data = card.GetData();
                    CardManager.Instance.AcquireCard(data);
                    if (data.rarity == CARD_RARITY.COMMON)
                    {
                        PlayerDataManager.Instance.IncreaseCardRewardOffset();
                    }
                    else if (data.rarity == CARD_RARITY.RARE)
                    {
                        PlayerDataManager.Instance.ResetCardRewardOffset();
                    }
                    PlayerDataManager.Instance.ClearCardRewardList();
                    ButtonClose();
                });
            cardList.Add(card);
        }
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
