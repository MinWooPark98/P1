using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyPatternState : ScriptableObject
{
    public bool selfTransition = false;
    public List<EnemyPatternTransition> transitions = new List<EnemyPatternTransition>();

    public Rect rect;
}

[CreateAssetMenu(fileName = "NormalPatternState", menuName = "Scriptable Object/EnemyPatternState/State/Normal")]
public class EnemyNormalPatternState : EnemyPatternState
{
    public EnemyPattern pattern;

}

[CreateAssetMenu(fileName = "RandomPatternState", menuName = "Scriptable Object/EnemyPatternState/State/Normal")]
public class EnemyRandomPatternState : EnemyPatternState
{
    [System.Serializable]
    public class RandomPattern
    {
        public float weight;            // °¡ÁßÄ¡
        public EnemyPattern pattern;
    }

    public List<RandomPattern> randomPatterns;
}


[CreateAssetMenu(fileName = "EnemyPatternTransition", menuName = "Scriptable Object/EnemyPatternState/Transition")]
public class EnemyPatternTransition : ScriptableObject
{
    [System.Serializable]
    public class Condition
    {
        public ENEMY_PATTERN_CONDITION condition;
        public float value;
    }

    public Condition condition;
    public EnemyPatternState targetState;

    public Vector2 startPos;
    public Vector2 endPos;
}
