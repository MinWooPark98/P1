using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuffListner
{
    public void OnChangedBuff(List<BuffData> _buffs);
}
