using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CardAction
{
    public interface ICardAction
    {
        public enum ActionType
        {
            Attack,

        }

        public enum TargetType
        {
            MySelf,
            SingleEnemy,
            RandomEnemy,
            AllEnemies,
        }

        public ActionType TypeAction { get; }
        public TargetType TypeTarget { get; }

        /// <summary>
        /// SingleEnemy일 때만 target 입력
        /// </summary>
        /// <param name="_target"></param>
        public void Action(Character _target = null);
        public string GetDesc();
    }

    [System.Serializable]
    public struct Attack : ICardAction
    {
        public ICardAction.ActionType TypeAction => ICardAction.ActionType.Attack;
        public ICardAction.TargetType TypeTarget => type;

        public ICardAction.TargetType type;
        public int damage;

        public void Action(Character _target)
        {
            List<Enemy> enemyList = BattleManager.Instance.GetEnemyList();

            switch (type)
            {
                case ICardAction.TargetType.MySelf:
                    {
                        Character player = BattleManager.Instance.GetPlayer();
                        player.Damage(damage);
                    }
                    break;
                case ICardAction.TargetType.SingleEnemy:
                    {
                        _target.Damage(damage);
                    }
                    break;
                case ICardAction.TargetType.RandomEnemy:
                    {
                        int random = Random.Range(0, enemyList.Count);
                        enemyList[random].Damage(damage);
                    }
                    break;
                case ICardAction.TargetType.AllEnemies:
                    {
                        for (int i = 0; i < enemyList.Count; i++)
                        {
                            enemyList[i].Damage(damage);
                        }
                    }
                    break;
            }
        }

        public string GetDesc()
        {
            return string.Format("피해를 {0} 줍니다", damage);
        }
    }
}

