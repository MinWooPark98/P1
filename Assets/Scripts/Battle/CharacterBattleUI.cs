using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterBattleUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IHpListner, IBuffListner, IDefenseListner
{
    [SerializeField]
    private TMP_Text textHp = null;

    [SerializeField]
    private Image imgHpBarBack = null;

    [SerializeField]
    private Image imgHpBar = null;

    private float HpApplyDuration = 2f;

    [SerializeField]
    private TMP_Text textDefense = null;

    [SerializeField]
    private GameObject objBuff = null;

    [SerializeField]
    private TMP_Text textBuff = null;

    private bool isApplyingHp = false;
    private float lerpPercentage = 0f;

    [SerializeField]
    private System.Action actionClicked = null;

    private bool onPointer = false;


    public void Init(int _currHp, int _maxHp, IReadOnlyList<BuffData> _buffList)
    {
        //textHp.text = string.Format(_currHp + " / " + _maxHp);
        //float hpPercentage = GetHpPercentage(_currHp, _maxHp);
        //imgHpBarBack.fillAmount = hpPercentage;
        //imgHpBar.fillAmount = hpPercentage;
        SetHp(_currHp, _maxHp);
        SetBuffs(_buffList);
    }

    private void SetHp(int _currHp, int _maxHp)
    {
        if (_currHp <= 0)           // юс╫ц
        {
            gameObject.SetActive(false);
            return;
        }
        textHp.text = string.Format(_currHp + " / " + _maxHp);
        imgHpBar.fillAmount = GetHpPercentage(_currHp, _maxHp);
        isApplyingHp = true;
        lerpPercentage = 0f;
    }

    private float GetHpPercentage(int _currHp, int _maxHp)
    {
        return (float)_currHp / _maxHp;
    }

    public void SetDefense(int _defense)
    {
        textDefense.text = _defense.ToString();
    }

    public void ShowBuff(bool _show)
    {
        objBuff.SetActive(_show);
    }

    public void SetBuffs(IReadOnlyList<BuffData> _buffs)
    {
        string strBuff = string.Empty;
        if (_buffs == null || _buffs.Count == 0)
        {
            strBuff = "None";
        }
        else
        {
            for (int i = 0; i < _buffs.Count; i++)
            {
                if (i > 0)
                {
                    strBuff = string.Format(strBuff + "\n");
                }

                strBuff = _buffs[i].value < 0 ? string.Format(strBuff + _buffs[i].type) : string.Format(strBuff + _buffs[i].type + " " + _buffs[i].value); 
            }
        }
        textBuff.text = strBuff;
    }

    private void Update()
    {
        if (isApplyingHp)
        {
            lerpPercentage += Time.deltaTime / HpApplyDuration;
            if (lerpPercentage > 1f)
            {
                lerpPercentage = 1f;
                isApplyingHp = false;
            }
            imgHpBarBack.fillAmount = Mathf.Lerp(imgHpBarBack.fillAmount, imgHpBar.fillAmount, lerpPercentage);
        }
    }

    public void SetActionClicked(System.Action _actionClicked)
    {
        actionClicked = _actionClicked;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointer = true;
        ShowBuff(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointer = false;
        ShowBuff(false);
    }

    public bool GetOnPointer() => onPointer;

    public void OnChangedHp(int _currHp, int _maxHp)
    {
        SetHp(_currHp, _maxHp);
    }

    public void OnChangedBuff(List<BuffData> _buffs)
    {
        SetBuffs(_buffs);
    }

    public void OnChangedDefense(int _defense)
    {
        SetDefense(_defense);
    }
}
