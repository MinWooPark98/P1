using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class EnemyPatternState
{
    public bool selfTransition = false;
    public List<Transition> transitions = new List<Transition>();

    public Rect rect;
}

[System.Serializable]
public class EnemyNormalPatternState : EnemyPatternState
{
    public EnemyPattern pattern;

}

[System.Serializable]
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

[System.Serializable]
public class Transition
{
    public EnemyPatternState targetState;
}
