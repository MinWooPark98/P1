using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupRestSite : Popup
{
    public void ButtonRest()
    {
        PlayerDataManager.Instance.SetCurrHp(PlayerDataManager.Instance.GetCurrHp() + (int)(PlayerDataManager.Instance.GetMaxHp() * 0.3f));
        ButtonClose();
    }

    public void ButtonEnhanceCard()
    {
        PopupEnhanceCard popupEnhanceCard = UIManager.Instance.MakePopup<PopupEnhanceCard>();
        popupEnhanceCard.SetCards();
        popupEnhanceCard.SetAction(
            () =>
            {
                if (popupEnhanceCard.IsEnhanced() == true)
                {
                    ButtonClose();
                }
            });
    }
}
