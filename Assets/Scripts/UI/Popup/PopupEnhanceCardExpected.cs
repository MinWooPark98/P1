using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupEnhanceCardExpected : Popup
{
    [SerializeField]
    private ItemCard itemCard = null;

    private CardData cardData = null;
    private CardData cardDataEnhanced = null;

    private System.Action actionEnhance = null;

    public void Set(CardData _cardData)
    {
        cardData = _cardData;
        // �ӽ�
        cardDataEnhanced = _cardData;

        itemCard.Set(cardData);
    }

    public void ShowEnhancedResult(bool _show)
    {
        if (_show)
        {
            itemCard.Set(cardDataEnhanced);
        }
        else
        {
            itemCard.Set(cardData);
        }
    }

    public void SetActionEnhance(System.Action _actionEnhance)
    {
        actionEnhance = _actionEnhance;
    }

    public void ButtonEnhance()
    {
        actionEnhance?.Invoke();
        LogManager.Log("��ȭ");
        ButtonClose();
    }
}
