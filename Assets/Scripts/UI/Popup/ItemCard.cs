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

    private CardData data = null;


    public void Set(CardData _data)
    {
        data = _data;

        textName.text = string.Format(data.idName + (data.enhanced > 0 ? "+" + data.enhanced.ToString() : string.Empty));
        switch (data.type)
        {
            case CARD_TYPE.ATTACK:
                {
                    AttackCard _newData = (AttackCard)data;
                    textDesc.text = string.Format(_newData.idDesc, _newData.damage);
                }
                break;
            case CARD_TYPE.SKILL:
                {
                    SkillCard _newData = (SkillCard)data;
                    textDesc.text = string.Format(_newData.idDesc, _newData.block);
                }
                break;
            case CARD_TYPE.POWER:
                {
                    textDesc.text = data.idDesc;
                }
                break;
            case CARD_TYPE.CURSE:
                {
                    textDesc.text = data.idDesc;
                }
                break;
            case CARD_TYPE.CC:
                {
                    textDesc.text = data.idDesc;
                }
                break;
            default:
                break;
        }
    }

    public void Use()
    {
        if (BattleManager.Instance.GetState() != BattleManager.BATTLE_STATE.PLAYERTURN)
        {
            return;
        }

        switch (data.type)
        {
            case CARD_TYPE.ATTACK:
                imgBackground.color = Color.red;
                // 상대 캐릭터 선택 시 공격하게 해야 하는데, 어떻게 해야 할 지 고민중
                break;
            case CARD_TYPE.SKILL:
                break;
            case CARD_TYPE.POWER:
                break;
            case CARD_TYPE.CURSE:
                break;
            case CARD_TYPE.CC:
                break;
        }
    }

}
