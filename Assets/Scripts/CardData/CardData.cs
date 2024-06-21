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
    public CARD_TYPE type = CARD_TYPE.NONE;                       // Ÿ�� (����, ��ų, �Ŀ�, ����, �����̻�)

    public int id;                                                // ī�� ID

    // id�� �ٲ� ���� - ���̺� �����
    public string idName = string.Empty;

    //[SerializeField]
    public CARD_RARITY rarity = CARD_RARITY.NONE;                 // ��͵�
    
    //[SerializeField]
    public int enhanced = 0;                                    // ��ȭ ��ġ

    //[SerializeField]
    public int energy = 0;                                      // �Ҹ��ϴ� ������ ��

    [SerializeField]
    public List<ICardAction> actionList = null;

    //[SerializeReference]
    //[SerializeField]
    public List<BuffData> buffList = null;                          // ī�� ���� �����ϴ� ����

    //[SerializeField]
    public List<CardFeature> featureList = null;                // ī�� Ư�� (�ֹ߼�, �Ҹ� ��)


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
