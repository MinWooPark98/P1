using UnityEngine;

[System.Serializable]
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
}
