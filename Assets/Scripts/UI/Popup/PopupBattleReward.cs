using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupBattleReward : Popup
{
    [SerializeField]
    private Button btnRewardCard = null;

    public void ButtonCardReward()
    {
        PopupBattleRewardCard popupBattleRewardCard = UIManager.Instance.MakePopup<PopupBattleRewardCard>();
        popupBattleRewardCard.SetAction(
            () =>
            {
                if (PlayerDataManager.Instance.GetCardRewardList().Count == 0)
                {
                    btnRewardCard.interactable = false;
                }
            });
    }
}
