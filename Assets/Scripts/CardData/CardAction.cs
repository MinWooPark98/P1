using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CardAction
{
    [System.Serializable]
    public abstract class CardAction
    {
        [System.Serializable]
        public enum ActionType
        {
            None = -1,
            SingleAttack,
            MultipleAttack,
            Defense,
            DrawCard,
        }

        [System.Serializable]
        public enum TargetType
        {
            None = -1,
            MySelf,
            SingleEnemy,
            RandomEnemy,
            AllEnemies,
        }

        public ActionType actionType = ActionType.None;
        public TargetType targetType;

        public List<BuffData> buffs = new List<BuffData>();

        /// <summary>
        /// SingleEnemy일 때만 target 입력
        /// </summary>
        /// <param name="_target"></param>
        public abstract void Action(Character _target = null);
        public abstract string GetDesc();
    }

    [System.Serializable]
    public class SingleAttack : CardAction
    {
        public int damage;

        public override void Action(Character _target)
        {
            List<Enemy> enemyList = BattleManager.Instance.GetEnemyList();

            switch (targetType)
            {
                case CardAction.TargetType.MySelf:
                    {
                        Character player = BattleManager.Instance.GetPlayer();
                        player.Damage(damage);
                        player.AddBuff(buffs);
                    }
                    break;
                case CardAction.TargetType.SingleEnemy:
                    {
                        _target.Damage(damage);
                        _target.AddBuff(buffs);
                    }
                    break;
                case CardAction.TargetType.RandomEnemy:
                    {
                        int random = Random.Range(0, enemyList.Count);
                        enemyList[random].Damage(damage);
                        enemyList[random].AddBuff(buffs);
                    }
                    break;
                case CardAction.TargetType.AllEnemies:
                    {
                        for (int i = 0; i < enemyList.Count; i++)
                        {
                            enemyList[i].Damage(damage);
                            enemyList[i].AddBuff(buffs);
                        }
                    }
                    break;
            }
        }

        public override string GetDesc()
        {
            return string.Format("피해를 {0} 줍니다", damage);
        }
    }

    [System.Serializable]
    public class MultipleAttack : CardAction
    {
        public int damage;
        public int count;

        public override  void Action(Character _target)
        {
            List<Enemy> enemyList = BattleManager.Instance.GetEnemyList();

            switch (targetType)
            {
                case CardAction.TargetType.MySelf:
                    {
                        Character player = BattleManager.Instance.GetPlayer();
                        for (int i = 0; i < count; i++)
                        {
                            player.Damage(damage);
                            player.AddBuff(buffs);
                        }
                    }
                    break;
                case CardAction.TargetType.SingleEnemy:
                    {
                        for (int i = 0; i < count; i++)
                        {
                            _target.Damage(damage);
                            _target.AddBuff(buffs);
                        }
                    }
                    break;
                case CardAction.TargetType.RandomEnemy:
                    {
                        for (int i = 0; i < count; i ++)
                        {
                            int random = Random.Range(0, enemyList.Count);
                            enemyList[random].Damage(damage);
                            enemyList[random].AddBuff(buffs);
                        }
                    }
                    break;
                case CardAction.TargetType.AllEnemies:
                    {
                        for (int i = 0; i < enemyList.Count; i++)
                        {
                            for (int j = 0; j < count; j++)
                            {
                                enemyList[i].Damage(damage);
                                enemyList[i].AddBuff(buffs);
                            }
                        }
                    }
                    break;
            }
        }

        public override string GetDesc()
        {
            return string.Format("피해를 {0}만큼 {1}회 줍니다", damage, count);
        }
    }

    [System.Serializable]
    public class Defense : CardAction
    {
        public int defense;

        public override void Action(Character _target = null)
        {
            Character player = BattleManager.Instance.GetPlayer();
            player.AddDefense(defense);
            player.AddBuff(buffs);
        }

        public override string GetDesc()
        {
            return string.Format("방어력을 {0}만큼 획득합니다", defense);
        }
    }

    [System.Serializable]
    public class DrawCard : CardAction
    {
        public int drawCount;

        public override void Action(Character _target = null)
        {
            BattleManager.Instance.AddDrawCount(drawCount);
        }

        public override string GetDesc()
        {
            return string.Format("카드를 {0}장 뽑습니다", drawCount);
        }
    }
}

