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
    public CARD_TYPE type = CARD_TYPE.NONE;                       // Ÿ�� (����, ��ų, �Ŀ�, ����, �����̻�)

    // id�� �ٲ� ���� - ���̺� �����
    public string idName = string.Empty;

    // id�� �ٲ� ���� - ���̺� �����
    public string idDesc = string.Empty;

    //[SerializeField]
    public CARD_RARITY rarity = CARD_RARITY.NONE;                 // ��͵�
    
    //[SerializeField]
    public int enhanced = 0;                                    // ��ȭ ��ġ

    //[SerializeField]
    public int energy = 0;                                      // �Ҹ��ϴ� ������ ��
    
    //[SerializeReference]
    //[SerializeField]
    public List<BuffData> buffList = null;                          // ī�� ���� �����ϴ� ����

    //[SerializeField]
    public List<CardFeature> featureList = null;                // ī�� Ư�� (�ֹ߼�, �Ҹ� ��)

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
