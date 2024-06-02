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
    PLAYER,                     // 플레이어 대상으로 버프를 주고 싶은 경우  <==  공용 버프 + 플레이어 전용 버프
    ENEMY,                      // 적 대상으로 버프를 주고 싶은 경우  <==  공용 버프 + 적 전용 버프
}

public enum BUFF_TYPE
{
    // 공용 버프 (11000 ~ 11999)
    STR = 11000,                    // 힘
    DEX = 11001,                    // 민첩
    DEF = 11002,                    // 방어도
    VULNERABLE = 11003,             // 취약
    WEAK = 11004,                   // 약화

    // 플레이어 전용 버프 (12000 ~ 12999)
    CONFUSED = 12000,                       // 혼란 = 카드 비용 0 ~ 3 랜덤
    ENTANGLED = 12001,                      // 속박 = 공격 불가
    FRAIL = 12002,                          // 손상 = 획득 방어도 25% 감소

    // 적 전용 버프 (13000 ~ 13999)
    testEnemy1 = 13000,
    testEnemy2 = 13001,
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
