using System.Collections.Generic;
using UnityEngine;

public abstract class CardData : ScriptableObject
{
    public enum Type
    {
        None = -1,
        Attack,             // 공격 카드
        Skill,              // 스킬 카드
        Power,              // 파워 카드
        Curse,              // 저주 카드
        CC,                 // 상태이상 카드
    }

    public enum Rarity
    {
        None = -1,
        Starter,            // 시작 시 기본적으로 주어지는 카드
        Common,             // 일반
        Uncommon,           // 특별
        Rare,               // 희귀
        Special,            // 그 외
    }

    public enum Characteristic
    {
        Exhaust,            // 소멸
        Ethereal,           // 휘발성
        Unplayable,         // 사용불가
    }

    [ReadOnly]
    public Type type = Type.None;
    public Rarity rarity = Rarity.None;
    [SerializeReference]
    public List<Buff> buffList = null;
}
