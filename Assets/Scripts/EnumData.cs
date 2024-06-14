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
    Player,                     // �÷��̾� ������� ������ �ְ� ���� ���  <==  ���� ���� + �÷��̾� ���� ����
    Enemy,                      // �� ������� ������ �ְ� ���� ���  <==  ���� ���� + �� ���� ����
}

public enum BUFF_TYPE
{
    // ���� (11000 ~ 11999)
    // ����
    Artifact = 11000,
    Barricade,
    Buffer,
    Dexterity,
    DrawCard,
    Energized,
    Focus,
    Intangible,
    Mantra,
    Metallicize,
    NextTurnBlock,
    PlatedArmor,
    Regenerate,
    Ritual,
    Strength,
    Thorns,
    Vigor,

    // ���� �����
    Frail,                          // �ջ� = ȹ�� �� 25% ����
    Vulnerable,                     // ���
    Weak,                           // ��ȭ


    // �÷��̾� ���� (12000 ~ 12999)
    // ����
    Accuracy = 12000,
    AfterImage,
    Amplify,
    BattleHymn,
    Berserk,
    Blasphemer,
    Blur,
    Brutality,
    Burst,
    Collect,
    Combust,
    Corruption,
    CreativeAI,
    DarkEmbrace,
    DemonForm,
    Deva,
    Devotion,
    DoubleDamage,
    Duplicatioin,
    EchoForm,
    Electro,
    Envenom,
    Equilibrium,

    // �����
    Confused,                       // ȥ�� = ī�� ��� 0 ~ 3 ����
    Entangled,                      // �ӹ� = ���� �Ұ�


    // �� ���� (13000 ~ 13999)
    // ���� 
    Angry = 13000,                  // �г�                            
    BackAttack,
    BeatOfDeath,
    Curiosity,
    CurlUp,                         // �� ����
    Enrage,
    Explosive,
    Fanding,
    Invincible,
    LifeLink,
    Malleable,
    Minion,
    ModeShift,
    PainfulStabs,
    Reactive,
    SharpHide,
    Shifting,
    Split,
    SporeCloud,
    Statsis,
    StrengthUp,
    Thievery,                       // ������
    TimeWarp,
    Unawakened,
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

public enum ENEMY_PATTERN_CONDITION
{
    None,                   // ���� ����
    TurnCount,              // n��°
    HpMoreThan,             // (0 ~ 1)
    HpLessThan,             // (0 ~ 1)
}

