using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemCard : Button
{
    [SerializeField]
    private Image imgBackground = null;

    [SerializeField]
    private TMP_Text textName;

    [SerializeField]
    private TMP_Text textDesc;

    private CardData data = null;

    private System.Action actionClicked = null;
   // private bool isLookingAt = false;
    //private bool isSelected = false;
    //private bool isTargeting = false;
    
    public enum CARD_STATE
    {
        NONE = -1,
        DRAW,
        NORMAL,
        SELECTED,
        TARGETING,
        USING,
        DISCARD,
        EXHAUSTED,
    }


    private CARD_STATE state = CARD_STATE.NONE;
    private bool isLookingAt = false;

    public CARD_STATE GetState() => state;
    public void SetState(CARD_STATE _state)
    {
        if (state == _state)
        {
            return;
        }

        state = _state;
        if (state > CARD_STATE.NORMAL)
        {
            imgBackground.color = Color.green;
        }
        else
        {
            imgBackground.color = Color.white;
        }
    }

    public bool GetLookingAt() => isLookingAt;

    public void Set(CardData _data)
    {
        data = _data;

        textName.text = string.Format(data.idName + (data.enhanced > 0 ? "+" + data.enhanced.ToString() : string.Empty));
        string strDesc = string.Empty;
        if (data.actionList != null)
        {
            for (int i = 0; i < data.actionList.Count; i++)
            {
                string textAction = data.actionList[i].GetDesc();
                strDesc += string.Format(textAction + (string.IsNullOrEmpty(textAction) == false ? "\n" : string.Empty));
            }
        }
        textDesc.text = strDesc;
        switch (data.type)
        {
            case CARD_TYPE.ATTACK:
                {
                    textName.color = Color.red;
                }
                break;
            case CARD_TYPE.SKILL:
                {
                    textName.color = Color.yellow;
                }
                break;
            case CARD_TYPE.POWER:
                {
                    textName.color = Color.blue;
                }
                break;
            case CARD_TYPE.CURSE:
                {
                    textName.color = Color.black;
                }
                break;
            case CARD_TYPE.STATUS:
                {
                    textName.color = Color.gray;
                }
                break;
            default:
                break;
        }
    }

    public CardData GetData() => data;

    private void Update()
    {
        isLookingAt = state == CARD_STATE.NORMAL && IsHighlighted();
    }

    public void SetActionClicked(System.Action _actionClicked)
    {
        actionClicked = _actionClicked;
    }

    public void ButtonClick()
    {
        actionClicked?.Invoke();
    }

    /// <summary>
    /// 100: 사용 가능 / 101: 에너지 부족, 102: 사용불가 특성 카드
    /// </summary>
    /// <returns></returns>
    public int GetUsable()
    {
        if (data.energy > BattleManager.Instance.GetEnergy())
        {
            return 101;
        }

        if (data.featureList.Exists((feature) => feature.type == CARD_FEATURE.UNPLAYABLE))
        {
            return 102;
        }

        return 100;
    }

    // 타겟 하나를 지정하는 카드의 경우
    public void Use(Character target = null)
    {
        if (BattleManager.Instance.GetState() != BattleManager.BATTLE_STATE.PLAYERTURN)
        {
            return;
        }

        if (GetUsable() != 100)
        {
            return;
        }

        SetState(CARD_STATE.USING);

        // 에너지 소모
        BattleManager.Instance.UseEnergy(data.energy);

        Character player = BattleManager.Instance.GetPlayer();
        List<Enemy> enemyList = BattleManager.Instance.GetEnemyList();

        for (int i = 0; i < data.actionList.Count; i++)
        {
            data.actionList[i].Action(target);
        }    

        //switch (data.type)
        //{
        //    case CARD_TYPE.ATTACK:
        //        {
        //            AttackCard card = (AttackCard)data;
        //            if (target != null)
        //            {
        //                target.Damage(card.damage);
        //            }
        //            else
        //            {
        //                for (int i = 0; i < enemyList.Count; i++)
        //                {
        //                    enemyList[i].Damage(card.damage);
        //                }
        //            }
        //        }
        //        break;
        //    case CARD_TYPE.SKILL:
        //        {
        //            SkillCard card = (SkillCard)data;
        //            player.AddDefense(card.block);
        //        }
        //        break;
        //    case CARD_TYPE.POWER:
        //        break;
        //    case CARD_TYPE.CURSE:
        //        break;
        //    case CARD_TYPE.STATUS:
        //        break;
        //}

        //for (int i = 0; i < data.buffList.Count; i++)
        //{
        //    switch (data.buffList[i].target)
        //    {
        //        case BUFF_TARGET.Player:
        //            {
        //                player.AddBuff(data.buffList[i]);
        //            }
        //            break;
        //        case BUFF_TARGET.Enemy:
        //            {
        //                if (data.buffList[i].applyAllEnemies)
        //                {
        //                    for (int j = 0; j < enemyList.Count; i++)
        //                    {
        //                        enemyList[i].AddBuff(data.buffList);
        //                    }
        //                }
        //                else
        //                {
        //                    target.AddBuff(data.buffList);
        //                }
        //            }
        //            break;
        //    }
        //}
    }

    public void EndUsing()
    {
        if (data.featureList.Exists((feature) => feature.type == CARD_FEATURE.ETHEREAL))
        {
            SetState(CARD_STATE.EXHAUSTED);
            return;
        }

        SetState(CARD_STATE.DISCARD);
    }
}
