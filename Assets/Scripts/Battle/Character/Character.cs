using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Character
{
    [SerializeField]
    private int maxHp = 0;
    public int MaxHp
    {
        get => maxHp;
        private set
        {
            maxHp = value;
            characterInfo.SetHp(currHp, maxHp);
        }
    }

    [SerializeField]
    private int currHp = 0;
    public int CurrHp
    {
        get => currHp;
        private set
        {
            currHp = value;
            characterInfo.SetHp(currHp, maxHp);
            if (currHp <= 0)
            {
                Die();
            }
        }
    }

    [SerializeField]
    private int defense;
    public int Defense
    {
        get => defense;
        private set
        {
            defense = value;
            characterInfo.SetDefense(defense);
        }
    }

    [SerializeField]
    private List<BuffData> buffList = new List<BuffData>();

    private bool isDead = false;
    private System.Action actionDie = null;

    private CharacterInfo characterInfo = null;


    public void SetCharacterInfo(CharacterInfo _characterInfo)
    {
        characterInfo = _characterInfo;
    }

    public CharacterInfo GetCharacterInfo() => characterInfo;

    public void Init(int _currHp, int _maxHp, int _defense, List<BuffData> _buffList)
    {
        CurrHp = _currHp;
        MaxHp = _maxHp;
        Defense = _defense;
        
        characterInfo.Init(_currHp, _maxHp, _buffList);
    }

    /// <summary>
    /// 최대 체력 증가
    /// </summary>
    /// <param name="_addMaxHp"></param>
    public void AddMaxHp(int _addMaxHp)
    {
        MaxHp += _addMaxHp;
    }

    /// <summary>
    /// 체력 회복
    /// </summary>
    /// <param name="_hp"></param>
    public void Recovery(int _hp)
    {
        int cal = CurrHp + _hp;
        if (cal > MaxHp)
        {
            cal = MaxHp;
        }
        CurrHp = cal;
    }

    /// <summary>
    /// 데미지 적용
    /// </summary>
    /// <param name="_damage"></param>
    public void Damage(int _damage)
    {
        Defense -= _damage;
        if (Defense < 0)
        {
            _damage = Defense * (-1);
            Defense = 0;
            CurrHp -= _damage;
        }
    }

    public void SetActionDie(System.Action _actionDie)
    {
        actionDie = _actionDie;
    }

    /// <summary>
    /// 죽음
    /// </summary>
    public void Die()
    {
        characterInfo.gameObject.SetActive(false);              // 티나게 해놓은 임시

        isDead = true;
        actionDie?.Invoke();
    }

    public bool IsDead() => isDead;

    /// <summary>
    /// 방어도 획득
    /// </summary>
    /// <param name="_defense"></param>
    public void AddDefense(int _defense)
    {
        Defense += _defense;
    }

    /// <summary>
    /// 단일 버프 추가
    /// </summary>
    /// <param name="_buff"></param>
    public void AddBuff(BuffData _buff)
    {
        bool isExist = false;
        for (int i = 0; i < buffList.Count; i++)
        {
            if (buffList[i].type == _buff.type)
            {
                BuffData newBuff = buffList[i];
                newBuff.value += _buff.value;
                buffList[i] = newBuff;
                isExist = true;
                break;
            }
        }

        if (!isExist)
        {
            buffList.Add(_buff);
        }

        characterInfo.SetBuffs(buffList);
    }

    /// <summary>
    /// 버프 여러 개 추가
    /// </summary>
    /// <param name="_newBuffs"></param>
    public void AddBuff(List<BuffData> _newBuffs)
    {
        for (int i = 0; i < _newBuffs.Count; ++i)
        {
            AddBuff(_newBuffs[i]);
        }
    }
}
