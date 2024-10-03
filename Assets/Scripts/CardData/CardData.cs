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
    public CARD_TYPE type = CARD_TYPE.NONE;                       // Ÿ�� (����, ��ų, �Ŀ�, ����, �����̻�)
    public int id;                                                // ī�� ID (0 ~ 999: IronClad)
    public string idName = string.Empty;
    public CARD_RARITY rarity = CARD_RARITY.NONE;                 // ��͵�   
    public int enhanced = 0;                                    // ��ȭ ��ġ
    public int energy = 0;                                      // �Ҹ��ϴ� ������ ��
    public List<CardAction.CardAction> actionList = new List<CardAction.CardAction>();
    public List<BuffData> buffList = new List<BuffData>();                          // ī�� ���� �����ϴ� ����
    public List<CardFeature> featureList = new List<CardFeature>();                // ī�� Ư�� (�ֹ߼�, �Ҹ� ��)

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
