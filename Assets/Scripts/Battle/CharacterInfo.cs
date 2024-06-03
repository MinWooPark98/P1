using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfo : MonoBehaviour
{
    [SerializeField]
    private TMP_Text textHp = null;

    [SerializeField]
    private Image imgHpBarBack = null;

    [SerializeField]
    private Image imgHpBar = null;

    [SerializeField]
    private TMP_Text textDefense = null;

    [SerializeField]
    private GameObject objBuff = null;

    [SerializeField]
    private TMP_Text textBuff = null;

    private bool isApplyingHp = false;
    private float lerpPercentage = 0f;


    public void Init(int _currHp, int _maxHp, List<BuffData> _buffList)
    {
        //textHp.text = string.Format(_currHp + " / " + _maxHp);
        //float hpPercentage = GetHpPercentage(_currHp, _maxHp);
        //imgHpBarBack.fillAmount = hpPercentage;
        //imgHpBar.fillAmount = hpPercentage;
        SetHp(_currHp, _maxHp);
        SetBuffs(_buffList);
    }

    public void SetHp(int _currHp, int _maxHp)
    {
        textHp.text = string.Format(_currHp + " / " + _maxHp);
        imgHpBarBack.fillAmount = GetHpPercentage(_currHp, _maxHp);
        isApplyingHp = true;
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

    public void SetBuffs(List<BuffData> _buffs)
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

                strBuff += string.Format(_buffs[i].value > 0 ? string.Format(strBuff + _buffs[i].type) : string.Format(strBuff + _buffs[i].type + " " + _buffs[i].value)); 
            }
        }
        textBuff.text = strBuff;
    }

    private void Update()
    {
        if (isApplyingHp)
        {
            lerpPercentage += Time.deltaTime;
            if (lerpPercentage > 1f)
            {
                lerpPercentage = 1f;
                isApplyingHp = false;
            }
            //imgHpBar.fillAmount = Mathf.Lerp(imgHpBar.fillAmount, imgHpBarBack.fillAmount, lerpPercentage);
        }
    }
}
