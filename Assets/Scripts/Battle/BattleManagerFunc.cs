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

    public void TurnEnd()
    {
        if (currState != BATTLE_STATE.PLAYERTURN)
        {
            return;
        }

        ChangeState(BATTLE_STATE.ENEMYTURN);
    }

    public List<CardData> GetDrawPile() => drawPile;

    public Character GetPlayer() => player;
    public List<Character> GetEnemyList() => enemyList;
}
