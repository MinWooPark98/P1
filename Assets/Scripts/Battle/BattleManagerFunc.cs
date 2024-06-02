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
