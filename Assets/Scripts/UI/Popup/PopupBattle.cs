using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupBattle : Popup
{
    [SerializeField]
    private TMP_Text textEnergy = null;

    [SerializeField]
    private CharacterInfo playerInfo = null;

    [SerializeField]
    private List<CharacterInfo> enemyInfos = null;

    [SerializeField]
    private ObjectHand objHand = null;

    [SerializeField]
    private Transform drawPile = null;


    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    public void DrawCard(CardData _data)
    {
        objHand.DrawCard(_data, drawPile);        
    }

    public void DiscardCard(ItemCard _itemCard)
    {
        objHand.DiscardCard(_itemCard);
    }

    public void SetEnergy(int _energy, int _refillEnergy)
    {
        textEnergy.text = string.Format(_energy + "/" +  _refillEnergy);
    }

    public void TurnEnd()
    {
        BattleManager.Instance.TurnEnd();
    }

    public CharacterInfo GetPlayerInfo() => playerInfo;

    public List<CharacterInfo> GetEnemyInfos() => enemyInfos;

    public void ButtonDrawPile()
    {
        PopupDrawPile popupDrawPile = UIManager.Instance.MakePopup<PopupDrawPile>();
        popupDrawPile.SetCards(BattleManager.Instance.GetDrawPile());
    }
}
