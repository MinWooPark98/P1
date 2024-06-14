using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class EnemyPatternState : ScriptableObject
{
    public List<EnemyPatternTransition> transitions = new List<EnemyPatternTransition>();
}
