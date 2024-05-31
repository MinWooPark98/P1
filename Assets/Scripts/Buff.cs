using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Buff
{
}


// 공용 버프/디버프
public class BuffPublic : Buff
{
    [Serializable]
    public enum Type
    {
        Str,                    // 힘
        Dex,                    // 민첩
        Def,                    // 방어도
        MAX
    }

    [SerializeField]
    private Type type;
    [SerializeField]
    private int value;

    public BuffPublic(Type type, int value)
    {
        this.type = type;
        this.value = value;
    }
}


// 플레이어 버프/디버프
public class BuffPlayer : Buff
{
    [Serializable]
    public enum Type
    {
        Confused,                       // 혼란 = 카드 비용 0 ~ 3 랜덤
        Entangled,                      // 속박 = 공격 불가
        Frail,                          // 손상 = 획득 방어도 25% 감소
        MAX,
    }

    [SerializeField]
    private Type type;
    [SerializeField]
    private int value;

    public BuffPlayer(Type type, int value)
    {
        this.type = type;
        this.value = value;
    }
}


// 적 버프/디버프
public class BuffEnemy : Buff
{
    [Serializable]
    public enum Type
    {
        testEnemy1,
        testEnemy2,
        MAX,
    }

    [SerializeField]
    private Type type;
    [SerializeField]
    private int value;

    public BuffEnemy(Type type, int value)
    {
        this.type = type;
        this.value = value;
    }
}
