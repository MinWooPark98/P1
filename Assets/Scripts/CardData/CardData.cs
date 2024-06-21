using System;
using System.Collections.Generic;
using UnityEngine;
using CardAction;

public abstract class CardData : ScriptableObject
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
    [ReadOnly]
    //[SerializeField]
    public CARD_TYPE type = CARD_TYPE.NONE;                       // 타입 (공격, 스킬, 파워, 저주, 상태이상)

    public int id;                                                // 카드 ID

    // id로 바꿀 거임 - 테이블 만들면
    public string idName = string.Empty;

    //[SerializeField]
    public CARD_RARITY rarity = CARD_RARITY.NONE;                 // 희귀도
    
    //[SerializeField]
    public int enhanced = 0;                                    // 강화 수치

    //[SerializeField]
    public int energy = 0;                                      // 소모하는 에너지 값

    [SerializeField]
    public List<ICardAction> actionList = null;

    //[SerializeReference]
    //[SerializeField]
    public List<BuffData> buffList = null;                          // 카드 사용시 적용하는 버프

    //[SerializeField]
    public List<CardFeature> featureList = null;                // 카드 특성 (휘발성, 소멸 등)


    public void AddAction(ICardAction.ActionType _type)
    {
        ICardAction cardAction = null;
        switch (_type)
        {
            case ICardAction.ActionType.Attack:
                cardAction = new Attack();
                break;
        }

        if (cardAction != null)
        {
            if (actionList == null)
            {
                actionList = new List<ICardAction>();
            }
            actionList.Add(cardAction);
        }
    }

    public void ClearActionList()
    {
        actionList.Clear();
    }

    public void AddBuff(BuffData _buff)
    {
        buffList.Add(_buff);
    }

    public void ClearBuffList()
    {
        buffList.Clear();
    }

    public void AddFeature(CardFeature _feature)
    {
        featureList.Add(_feature);
    }

    public void ClearFeatureList()
    {
        featureList.Clear();
    }
}
