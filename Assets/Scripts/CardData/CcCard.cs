using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CcCard", menuName = "Scriptable Object/Card/CcCard")]
public class CcCard : CardData
{
    private void OnEnable()
    {
        type = Type.CC;
    }
}
