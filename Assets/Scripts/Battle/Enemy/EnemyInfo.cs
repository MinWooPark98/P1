using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyInfo", menuName = "Scriptable Object/EnemyInfo")]
public class EnemyInfo : ScriptableObject
{
    public string name;
    public string desc;
    public List<EnemyPatternState> patternStates = new List<EnemyPatternState>();
}
