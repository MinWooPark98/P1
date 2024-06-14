using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "RandomPatternState", menuName = "Scriptable Object/EnemyPatternState/State/Normal")]
public class EnemyRandomPatternState : EnemyPatternState
{
    [System.Serializable]
    public class RandomPattern
    {
        public float weight;            // ����ġ
        public EnemyPattern pattern;
    }

    public List<RandomPattern> randomPatterns;
}

