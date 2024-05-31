using System.Collections.Generic;
using UnityEngine;

public abstract class CardData : ScriptableObject
{
    public enum Type
    {
        None = -1,
        Attack,             // ���� ī��
        Skill,              // ��ų ī��
        Power,              // �Ŀ� ī��
        Curse,              // ���� ī��
        CC,                 // �����̻� ī��
    }

    public enum Rarity
    {
        None = -1,
        Starter,            // ���� �� �⺻������ �־����� ī��
        Common,             // �Ϲ�
        Uncommon,           // Ư��
        Rare,               // ���
        Special,            // �� ��
    }

    public enum Characteristic
    {
        Exhaust,            // �Ҹ�
        Ethereal,           // �ֹ߼�
        Unplayable,         // ���Ұ�
    }

    [ReadOnly]
    public Type type = Type.None;
    public Rarity rarity = Rarity.None;
    [SerializeReference]
    public List<Buff> buffList = null;
}
