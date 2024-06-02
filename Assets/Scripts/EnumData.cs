using System;

public enum CARD_TYPE
{
    NONE = -1,
    ATTACK,             // ���� ī��
    SKILL,              // ��ų ī��
    POWER,              // �Ŀ� ī��
    CURSE,              // ���� ī��
    CC,                 // �����̻� ī��
}

public enum CARD_RARITY
{
    NONE = -1,
    STARTER,            // ���� �� �⺻������ �־����� ī��
    COMMON,             // �Ϲ�
    UNCOMMON,           // Ư��
    RARE,               // ���
    SPECIAL,            // �� ��
}

public enum CARD_FEATURE
{
    EXHAUST,            // �Ҹ�
    ETHEREAL,           // �ֹ߼�
    UNPLAYABLE,         // ���Ұ�
}



/// <summary>
/// �����Ϳ��� ���� ����� ���� �ϱ� ���� enum :: �ΰ��ӿ��� ������� ����
/// </summary>
public enum BUFF_TARGET
{
    PLAYER,                     // �÷��̾� ������� ������ �ְ� ���� ���  <==  ���� ���� + �÷��̾� ���� ����
    ENEMY,                      // �� ������� ������ �ְ� ���� ���  <==  ���� ���� + �� ���� ����
}

public enum BUFF_TYPE
{
    // ���� ���� (11000 ~ 11999)
    STR = 11000,                    // ��
    DEX = 11001,                    // ��ø
    DEF = 11002,                    // ��
    VULNERABLE = 11003,             // ���
    WEAK = 11004,                   // ��ȭ

    // �÷��̾� ���� ���� (12000 ~ 12999)
    CONFUSED = 12000,                       // ȥ�� = ī�� ��� 0 ~ 3 ����
    ENTANGLED = 12001,                      // �ӹ� = ���� �Ұ�
    FRAIL = 12002,                          // �ջ� = ȹ�� �� 25% ����

    // �� ���� ���� (13000 ~ 13999)
    testEnemy1 = 13000,
    testEnemy2 = 13001,
}

//public enum BUFF_PUBLIC
//{
//    STR,                    // ��
//    DEX,                    // ��ø
//    DEF,                    // ��
//    VULNERABLE,             // ���
//    WEAK,                   // ��ȭ
//    MAX
//}

//public enum BUFF_PLAYER
//{
//    CONFUSED,                       // ȥ�� = ī�� ��� 0 ~ 3 ����
//    ENTANGLED,                      // �ӹ� = ���� �Ұ�
//    FRAIL,                          // �ջ� = ȹ�� �� 25% ����
//    MAX,
//}

//public enum BUFF_ENEMY
//{
//    testEnemy1,
//    testEnemy2,
//    MAX,
//}
