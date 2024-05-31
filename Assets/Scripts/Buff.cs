using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Buff
{
}


// ���� ����/�����
public class BuffPublic : Buff
{
    [Serializable]
    public enum Type
    {
        Str,                    // ��
        Dex,                    // ��ø
        Def,                    // ��
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


// �÷��̾� ����/�����
public class BuffPlayer : Buff
{
    [Serializable]
    public enum Type
    {
        Confused,                       // ȥ�� = ī�� ��� 0 ~ 3 ����
        Entangled,                      // �ӹ� = ���� �Ұ�
        Frail,                          // �ջ� = ȹ�� �� 25% ����
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


// �� ����/�����
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
