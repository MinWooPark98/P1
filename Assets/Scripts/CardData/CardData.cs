using System;
using System.Collections.Generic;
using UnityEngine;
using CardAction;

[Serializable]
public class CardData
{
    [Serializable]
    public struct CardFeature
    {
        [SerializeField]
        public CARD_FEATURE type;
        [SerializeField]
        public int value;

        public CardFeature(CARD_FEATURE _type, int _value)
        {
            this.type = _type;
            this.value = _value;
        }
    }
    public CARD_TYPE type = CARD_TYPE.NONE;                       // 타입 (공격, 스킬, 파워, 저주, 상태이상)
    public int id;                                                // 카드 ID (0 ~ 999: IronClad)
    public string idName = string.Empty;
    public CARD_RARITY rarity = CARD_RARITY.NONE;                 // 희귀도   
    public int enhanced = 0;                                    // 강화 수치
    public int energy = 0;                                      // 소모하는 에너지 값
    public List<CardAction.CardAction> actionList = new List<CardAction.CardAction>();
    public List<BuffData> buffList = new List<BuffData>();                          // 카드 사용시 적용하는 버프
    public List<CardFeature> featureList = new List<CardFeature>();                // 카드 특성 (휘발성, 소멸 등)

    public bool IsTargetCard()
    {
        for (int i = 0; i < actionList.Count; i++)
        {
            if (actionList[i].targetType == CardAction.CardAction.TargetType.SingleEnemy)
            {
                return true;
            }
        }

        return false;
    }
}
