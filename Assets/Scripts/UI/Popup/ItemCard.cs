using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCard : MonoBehaviour
{
    [SerializeField]
    private Image imgBackground = null;

    [SerializeField]
    private TMP_Text textName;

    [SerializeField]
    private TMP_Text textDesc;

    [SerializeField]
    private Button btnCard = null;

    private CardData data = null;

    private System.Action actionClicked = null;
    private bool isSelected = false;

    public void Set(CardData _data)
    {
        data = _data;

        textName.text = string.Format(data.idName + (data.enhanced > 0 ? "+" + data.enhanced.ToString() : string.Empty));
        switch (data.type)
        {
            case CARD_TYPE.ATTACK:
                {
                    textName.color = Color.red;
                    AttackCard _newData = (AttackCard)data;
                    textDesc.text = string.Format(_newData.idDesc, _newData.damage);
                }
                break;
            case CARD_TYPE.SKILL:
                {
                    textName.color = Color.yellow;
                    SkillCard _newData = (SkillCard)data;
                    textDesc.text = string.Format(_newData.idDesc, _newData.block);
                }
                break;
            case CARD_TYPE.POWER:
                {
                    textName.color = Color.blue;
                    textDesc.text = data.idDesc;
                }
                break;
            case CARD_TYPE.CURSE:
                {
                    textName.color = Color.black;
                    textDesc.text = data.idDesc;
                }
                break;
            case CARD_TYPE.CC:
                {
                    textName.color = Color.gray;
                    textDesc.text = data.idDesc;
                }
                break;
            default:
                break;
        }
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
    /// 카드 선택 연출
    /// </summary>
    public void Select()
    {
        isSelected = true;
        if (Equals(data.GetType(), typeof(AttackCard)) &&
            ((AttackCard)data).atkAllEnemies == false)
        {
            BattleManager.Instance.SetTargeting(true);
        }
        imgBackground.color = Color.green;
    }

    /// <summary>
    ///  카드 선택 취소 연출
    /// </summary>
    public void Deselect()
    {
        isSelected = false;
        imgBackground.color = Color.white;
        BattleManager.Instance.SetTargeting(false);
    }

    // 타겟 하나를 지정하는 카드의 경우
    public void Use(Character target = null)
    {
        if (BattleManager.Instance.GetState() != BattleManager.BATTLE_STATE.PLAYERTURN ||
            BattleManager.Instance.GetSelectedCard() != this)
        {
            return;
        }

        if (!BattleManager.Instance.IsEnergyEnough(data.energy))
        {
            BattleManager.Instance.DeselectCard();
            LogManager.Log($"에너지 부족해서 스킬 사용 취소 // 필요 에너지: {data.energy} / 보유 에너지: {BattleManager.Instance.GetEnergy()}");
            return;
        }

        BattleManager.Instance.UseEnergy(data.energy);

        Character player = BattleManager.Instance.GetPlayer();
        Character[] enemyList = BattleManager.Instance.GetEnemyList();

        switch (data.type)
        {
            case CARD_TYPE.ATTACK:
                {
                    AttackCard card = (AttackCard)data;
                    if (target != null)
                    {
                        target.Damage(card.damage);
                    }
                    else
                    {
                        for (int i = 0; i < enemyList.Length; i++)
                        {
                            enemyList[i].Damage(card.damage);
                        }
                    }
                }
                break;
            case CARD_TYPE.SKILL:
                {
                    SkillCard card = (SkillCard)data;
                    player.AddDefense(card.block);
                }
                break;
            case CARD_TYPE.POWER:
                break;
            case CARD_TYPE.CURSE:
                break;
            case CARD_TYPE.CC:
                break;
        }

        for (int i = 0; i < data.buffList.Count; i++)
        {
            switch (data.buffList[i].target)
            {
                case BUFF_TARGET.PLAYER:
                    {
                        player.AddBuff(data.buffList[i]);
                    }
                    break;
                case BUFF_TARGET.ENEMY:
                    {
                        if (data.buffList[i].applyAllEnemies)
                        {
                            for (int j = 0; j < enemyList.Length; i++)
                            {
                                enemyList[i].AddBuff(data.buffList);
                            }
                        }
                        else
                        {
                            target.AddBuff(data.buffList);
                        }
                    }
                    break;
            }
        }

        BattleManager.Instance.DeselectCard();
        gameObject.SetActive(false);
    }

    // 타겟을 지정하지 않는 카드의 경우
    //public void Use()
    //{
    //    if (BattleManager.Instance.GetState() != BattleManager.BATTLE_STATE.PLAYERTURN ||
    //        BattleManager.Instance.GetSelectedCard() != this)
    //    {
    //        return;
    //    }

    //    switch (data.type)
    //    {
    //        case CARD_TYPE.ATTACK:
    //            {

    //            }
    //            break;
    //        case CARD_TYPE.SKILL:
    //            break;
    //        case CARD_TYPE.POWER:
    //            break;
    //        case CARD_TYPE.CURSE:
    //            break;
    //        case CARD_TYPE.CC:
    //            break;
    //    }
    //}
}
