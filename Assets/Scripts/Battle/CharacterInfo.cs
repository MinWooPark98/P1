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
    private TMP_Text textEnergy = null;

    [SerializeField]
    private TMP_Text textBuff = null;

    [SerializeField]
    private Image imgHpBarBack = null;

    [SerializeField]
    private Image imgHpBar = null;

    private bool isApplyingHp = false;
    private float lerpPercentage = 0f;


    public void SetHp(int _currHp, int _maxHp)
    {
        textHp.text = string.Format(_currHp + " / " + _maxHp);
        imgHpBarBack.fillAmount = (float)_currHp / _maxHp;
        isApplyingHp = true;
    }

    public void SetEnergy(int _energy)
    {
        textEnergy.text = _energy.ToString();
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
