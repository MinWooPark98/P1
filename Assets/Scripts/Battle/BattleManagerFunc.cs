using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleManager : MonoBehaviour
{
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
            LogManager.LogError("drawCount <= 0 인데, DrawCard 실행", this, "DrawCard");
            return;
        }
        
        if (handCards.Count >= StaticData.MAX_HAND_COUNT)
        {
            LogManager.Log("손이 꽉 참");
            drawCount = 0;
            return;
        }

        if (drawPile.Count == 0)
        {
            if (discardPile.Count == 0)
            {
                drawCount = 0;
                return;
            }
            drawPile = Utils.ShuffleList(discardPile);
            discardPile = new List<CardData>();
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
    public void DiscardCard(int _index)
    {
        discardPile.Add(handCards[_index]);
        handCards.RemoveAt(_index);
    }

    /// <summary>
    /// 카드를 소멸시킨다
    /// </summary>
    /// <param name="_cardData"></param>
    public void ExhaustCard(CardData _cardData)
    {
        exhaustedCards.Add(_cardData);
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
            LogManager.LogError("에너지 부족 - 이 메소드까지 오면 안 됨", this, "UseEnergy");
            return;
        }
        SetEnergy(energy - _energyCost);
    }

    public void AddEnergy(int _energy)
    {
        SetEnergy(energy + _energy);
    }

    public void LoseEnergy(int _energyLose)
    {
        int energyRemained = energy - _energyLose;
        if (energyRemained < 0)
        {
            energyRemained = 0;
        }
        SetEnergy(energyRemained);
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

    public void EndBattle()
    {
        MapManager.Instance.ExitRoom();
        gameObject.SetActive(false);
    }

    public int GetTurnCount() => turnCount;

    public List<CardData> GetDrawPile() => drawPile;
    public List<CardData> GetDiscardPile() => discardPile;
    public List<CardData> GetExhaustPile() => exhaustedCards;

    public Character GetPlayer() => player;
    public List<Enemy> GetEnemyList() => enemyList;
}
