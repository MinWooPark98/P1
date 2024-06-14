using System;

public enum CARD_TYPE
{
    NONE = -1,
    ATTACK,             // 공격 카드
    SKILL,              // 스킬 카드
    POWER,              // 파워 카드
    CURSE,              // 저주 카드
    CC,                 // 상태이상 카드
}

public enum CARD_RARITY
{
    NONE = -1,
    STARTER,            // 시작 시 기본적으로 주어지는 카드
    COMMON,             // 일반
    UNCOMMON,           // 특별
    RARE,               // 희귀
    SPECIAL,            // 그 외
}

public enum CARD_FEATURE
{
    EXHAUST,            // 소멸
    ETHEREAL,           // 휘발성
    UNPLAYABLE,         // 사용불가
}



/// <summary>
/// 에디터에서 버프 만들기 쉽게 하기 위한 enum :: 인게임에선 사용하지 않음
/// </summary>
public enum BUFF_TARGET
{
    Player,                     // 플레이어 대상으로 버프를 주고 싶은 경우  <==  공용 버프 + 플레이어 전용 버프
    Enemy,                      // 적 대상으로 버프를 주고 싶은 경우  <==  공용 버프 + 적 전용 버프
}

public enum BUFF_TYPE
{
    // 공용 (11000 ~ 11999)
    // 버프
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

    // 공용 디버프
    Frail,                          // 손상 = 획득 방어도 25% 감소
    Vulnerable,                     // 취약
    Weak,                           // 약화


    // 플레이어 전용 (12000 ~ 12999)
    // 버프
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

    // 디버프
    Confused,                       // 혼란 = 카드 비용 0 ~ 3 랜덤
    Entangled,                      // 속박 = 공격 불가


    // 적 전용 (13000 ~ 13999)
    // 버프 
    Angry = 13000,                  // 분노                            
    BackAttack,
    BeatOfDeath,
    Curiosity,
    CurlUp,                         // 몸 말기
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
    Thievery,                       // 도둑질
    TimeWarp,
    Unawakened,
}

//public enum BUFF_PUBLIC
//{
//    STR,                    // 힘
//    DEX,                    // 민첩
//    DEF,                    // 방어도
//    VULNERABLE,             // 취약
//    WEAK,                   // 약화
//    MAX
//}

//public enum BUFF_PLAYER
//{
//    CONFUSED,                       // 혼란 = 카드 비용 0 ~ 3 랜덤
//    ENTANGLED,                      // 속박 = 공격 불가
//    FRAIL,                          // 손상 = 획득 방어도 25% 감소
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
    None,                   // 조건 없음
    TurnCount,              // n턴째
    HpMoreThan,             // (0 ~ 1)
    HpLessThan,             // (0 ~ 1)
}

