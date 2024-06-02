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
            LogManager.LogError("drawCount <= 0 �ε�, DrawCard ����");
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
        CardData drawCard = drawPile[0];
        handCards.Add(drawCard);
        drawPile.RemoveAt(0);

        PopupBattle.Instance.DrawCard(drawCard);
    }

    public void TurnEnd()
    {
        if (currState != BATTLE_STATE.PLAYERTURN)
        {
            return;
        }

        ChangeState(BATTLE_STATE.ENEMYTURN);
    }
}
