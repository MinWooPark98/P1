using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleManager : MonoBehaviour
{
    /// <summary>
    /// ��ο��� ī�� ���� ����
    /// </summary>
    /// <param name="_count"></param>
    public void AddDrawCount(int _count)
    {
        drawCount += _count;
    }

    /// <summary>
    /// ��ο��� ī�� ���ڰ� 0���� ũ�� ��ο� ���� (UPDATE_PLAYERTURN���� ����)
    /// </summary>
    private void DrawCard()
    {
        if (drawCount <= 0)
        {
            LogManager.LogError("drawCount <= 0 �ε�, DrawCard ����", this, "DrawCard");
            return;
        }
        
        if (handCards.Count >= StaticData.MAX_HAND_COUNT)
        {
            LogManager.Log("���� �� ��");
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
    /// ī�带 ������ ������
    /// </summary>
    public void DiscardCard(int _index)
    {
        discardPile.Add(handCards[_index]);
        handCards.RemoveAt(_index);
    }

    /// <summary>
    /// ī�带 �Ҹ��Ų��
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
            LogManager.LogError("������ ���� - �� �޼ҵ���� ���� �� ��", this, "UseEnergy");
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
