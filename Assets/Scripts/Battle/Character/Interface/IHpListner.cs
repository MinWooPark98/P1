using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHpListner
{
    public void OnChangedHp(int currHp, int maxHp);
}
