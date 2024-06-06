using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleManager : MonoBehaviour
{
    public void ExhaustCard(CardData _cardData)
    {
        if (exhaustedCards == null)
        {
            exhaustedCards = new List<CardData>();
        }
        exhaustedCards.Add(_cardData);
    }

    /// <summary>
    /// 드로우할 카드 숫자 지정
    /// </summary>
    /// <param name="_count"></param>
    public void AddDrawCount(int _count)
    {
        drawCount += _count;
    }

    /// <summary>
    /// 드로우할 카드 숫자가 0보다 크면 드로우 진행 (UPDATE_PLAYERTURN에서 실행)
    /// </summary>
    private void DrawCard()
    {
        if (drawCount <= 0)
        {
            LogManager.LogError("drawCount <= 0 인데, DrawCard 실행");
            return;
        }

        if (drawPile.Count == 0)
        {
            if (discardPile.Count == 0)
            {
                drawCount = 0;
                return;
            }
            drawPile.AddRange(discardPile);
            discardPile.Clear();
        }

        drawCount--;
        CardData drawCardData = drawPile[0];
        handCards.Add(drawCardData);
        drawPile.RemoveAt(0);
        popupBattle.DrawCard(drawCardData);
    }

    /// <summary>
    /// 카드를 묘지로 보낸다
    /// </summary>
    public void DiscardCard(ItemCard _itemCard)
    {
        CardData dataDiscard = _itemCard.GetData();
        popupBattle.DiscardCard(_itemCard);
        handCards.Remove(dataDiscard);
        discardPile.Add(dataDiscard);
    }

    //public void SelectCard(ItemCard _itemCard)
    //{
    //    selectedCard = _itemCard;
    //    selectedCard.Select();
    //}

    //public void DeselectCard()
    //{
    //    selectedCard.Deselect();
    //    selectedCard = null;
    //}

    //public ItemCard GetSelectedCard()
    //{
    //    return selectedCard;
    //}

    ///// <summary>
    ///// 카드, 아이템 사용이 가능한지 여부 (현재 선택된 카드/아이템이 없어야 함)
    ///// </summary>
    //public bool IsSelectable()
    //{
    //    return selectedCard == null /*&& selectedItem == null*/;
    //}

    //public void SetTargeting(bool _isTargeting)
    //{
    //    isTargeting = _isTargeting;
    //}

    public void SetEnergy(int _energy)
    {
        energy = _energy;
        popupBattle.SetEnergy(energy, refillEnergy);
    }

    public void UseEnergy(int _energyCost)
    {
        if (energy < _energyCost)
        {
            LogManager.LogError("에너지 부족 - 이 메소드까지 오면 안 됨");
            return;
        }
        SetEnergy(energy - _energyCost);
    }

    public void AddEnergy(int _energy)
    {
        SetEnergy(energy + _energy);
    }

    public int GetEnergy() => energy;

    //public void UseCard(Character _target)
    //{
    //    selectedCard.Use(_target);
    //}

    public void TurnEnd()
    {
        if (currState != BATTLE_STATE.PLAYERTURN //||
            //selectedCard != null //||
            /*selectedItem == null*/)
        {
            return;
        }

        ChangeState(BATTLE_STATE.ENEMYTURN);
    }

    public Character GetPlayer() => player;
    public List<Character> GetEnemyList() => enemyList;
}
