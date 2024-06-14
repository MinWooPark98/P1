using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField]
    private EnemyInfo info = null;

    private EnemyPatternState currPatternState = null;

    public void SetInfo(string _infoName)
    {
        info = Resources.Load(string.Format("Scriptables/EnemyInfo/" + _infoName)) as EnemyInfo;
    }

    public void ActivePattern()
    {
        EnemyPattern pattern = GetPattern();
        Character player = BattleManager.Instance.GetPlayer();
        List<Enemy> enemyList = BattleManager.Instance.GetEnemyList();

        LogManager.Log(pattern.name + " / " + pattern.damage);
        if (pattern.damage >= 0)
        {
            player.Damage(pattern.damage);
        }

        for (int i = 0; i < pattern.buffList.Count; i++)
        {
            switch (pattern.buffList[i].target)
            {
                case BUFF_TARGET.Player:
                    {
                        player.AddBuff(pattern.buffList[i]);
                    }
                    break;
                case BUFF_TARGET.Enemy:
                    {
                        AddBuff(pattern.buffList);
                    }
                    break;
            }
        }

        SetNextPatternState();
    }

    private void SetNextPatternState()
    {
        if (currPatternState == null)
        {
            currPatternState = info.patternStates[0];
            return;
        }

        // priority가 작은 것부터 먼저 검사
        List<EnemyPatternTransition> listTransitionSorted = currPatternState.transitions.OrderBy((transition) => transition.condition.priority).ToList();
        bool isTransitted = false;
        for (int i = 0; i < listTransitionSorted.Count; i++)
        {
            if (isTransitted)
            {
                break;
            }

            switch (listTransitionSorted[i].condition.condition)
            {
                case ENEMY_PATTERN_CONDITION.None:
                    {
                        currPatternState = listTransitionSorted[i].targetState;
                        isTransitted = true;
                    }
                    break;
                case ENEMY_PATTERN_CONDITION.TurnCount:
                    {
                        if (BattleManager.Instance.GetTurnCount() == listTransitionSorted[i].condition.value)
                        {
                            currPatternState = listTransitionSorted[i].targetState;
                            isTransitted = true;
                        }
                    }
                    break;
                case ENEMY_PATTERN_CONDITION.HpMoreThan:
                    {
                        if (CurrHp >= listTransitionSorted[i].condition.value)
                        {
                            currPatternState = listTransitionSorted[i].targetState;
                            isTransitted = true;
                        }
                    }
                    break;
                case ENEMY_PATTERN_CONDITION.HpLessThan:
                    {
                        if (CurrHp < listTransitionSorted[i].condition.value)
                        {
                            currPatternState = listTransitionSorted[i].targetState;
                            isTransitted = true;
                        }
                    }
                    break;
            }
        }
    }

    private EnemyPattern GetPattern()
    {
        if (currPatternState == null)
        {
            SetNextPatternState();
        }

        if (currPatternState.GetType() == typeof(EnemyNormalPatternState))
        {
            EnemyNormalPatternState normalPattern = (EnemyNormalPatternState)currPatternState;
            return normalPattern.pattern;
        }

        if (currPatternState.GetType() == typeof(EnemyRandomPatternState))
        {
            EnemyRandomPatternState randomPattern = (EnemyRandomPatternState)currPatternState;
            int countPatterns = randomPattern.randomPatterns.Count;
            float totalWeight = 0f;
            for (int i = 0; i < countPatterns; i++)
            {
                totalWeight += randomPattern.randomPatterns[i].weight;
            }

            float ranWeight = Random.Range(0f, totalWeight);
            for (int i = 0; i < countPatterns; i++)
            {
                ranWeight -= randomPattern.randomPatterns[i].weight;
                if (ranWeight <= 0f)
                {
                    return randomPattern.randomPatterns[i].pattern;
                }
            }

            return randomPattern.randomPatterns[countPatterns - 1].pattern;
        }

        return null;
    }
}
