using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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

        switch (state)
        {
            case CARD_STATE.DISCARD:
                BattleManager.Instance.DiscardCard(data);
                Destroy(gameObject);
                break;
            default:
                break;
        }
    }

    public bool GetLookingAt() => isLookingAt;

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

    public CardData GetData() => data;

    private void Update()
    {
    }

    public void SetActionClicked(System.Action _actionClicked)
    {
        actionClicked = _actionClicked;
    }

    public void ButtonClick()
    {
        actionClicked?.Invoke();
    }

    public bool GetUsable()
    {
        return data.energy <= BattleManager.Instance.GetEnergy();
    }

    // 타겟 하나를 지정하는 카드의 경우
    public void Use(Character target = null)
    {
        if (BattleManager.Instance.GetState() != BattleManager.BATTLE_STATE.PLAYERTURN)
        {
            return;
        }

        if (GetUsable() == false)
        {
            return;
        }

        // 에너지 소모
        BattleManager.Instance.UseEnergy(data.energy);

        Character player = BattleManager.Instance.GetPlayer();
        List<Character> enemyList = BattleManager.Instance.GetEnemyList();

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
                        for (int i = 0; i < enemyList.Count; i++)
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
                case BUFF_TARGET.Player:
                    {
                        player.AddBuff(data.buffList[i]);
                    }
                    break;
                case BUFF_TARGET.Enemy:
                    {
                        if (data.buffList[i].applyAllEnemies)
                        {
                            for (int j = 0; j < enemyList.Count; i++)
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
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (state == CARD_STATE.NORMAL)
        {
            isLookingAt = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isLookingAt = false;
    }
}
