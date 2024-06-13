using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyPattern", menuName = "Scriptable Object/EnemyPattern")]
public class EnemyPattern : ScriptableObject
{
    public int damage;
    public List<BuffData> buffList;

    public void AddBuff(BuffData _buff)
    {
        buffList.Add(_buff);
    }

    public void ClearBuffList()
    {
        buffList.Clear();
    }
}
