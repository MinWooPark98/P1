using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardData : ScriptableObject
{
    [Serializable]
    public class CardFeature
    {
        [SerializeField]
        private CARD_FEATURE type;
        [SerializeField]
        private int value;

        public CardFeature(CARD_FEATURE _type, int _value)
        {
            this.type = _type;
            this.value = _value;
        }
    }
    [ReadOnly]
    //[SerializeField]
    public CARD_TYPE type = CARD_TYPE.NONE;                       // 타입 (공격, 스킬, 파워, 저주, 상태이상)

    // id로 바꿀 거임 - 테이블 만들면
    public string idName = string.Empty;

    // id로 바꿀 거임 - 테이블 만들면
    public string idDesc = string.Empty;

    //[SerializeField]
    public CARD_RARITY rarity = CARD_RARITY.NONE;                 // 희귀도
    
    //[SerializeField]
    public int enhanced = 0;                                    // 강화 수치

    //[SerializeField]
    public int energy = 0;                                      // 소모하는 에너지 값
    
    //[SerializeReference]
    //[SerializeField]
    public List<BuffData> buffList = null;                          // 카드 사용시 적용하는 버프

    //[SerializeField]
    public List<CardFeature> featureList = null;                // 카드 특성 (휘발성, 소멸 등)

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
