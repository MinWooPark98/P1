using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    private int maxHp = 0;

    [SerializeField]
    private int currHp = 0;

    [SerializeField]
    private int energy = 3;

    [SerializeField]
    private List<BuffData> buffList = new List<BuffData>();


    [SerializeField]
    private CharacterInfo characterInfo = null;


    private void Update()
    {
        if (maxHp > 0)
        {
            characterInfo.SetHp(currHp, maxHp);
        }
        characterInfo.SetEnergy(energy);
    }

    /// <summary>
    /// 최대 체력 입력
    /// </summary>
    /// <param name="_maxHp"></param>
    public void SetMaxHp(int _maxHp)
    {
        maxHp = _maxHp;
    }

    /// <summary>
    /// 최대 체력 증가
    /// </summary>
    /// <param name="_addMaxHp"></param>
    public void AddMaxHp(int _addMaxHp)
    {
        maxHp += _addMaxHp;
    }

    /// <summary>
    /// 체력 회복
    /// </summary>
    /// <param name="_hp"></param>
    public void Recovery(int _hp)
    {
        currHp += _hp;
        if (currHp > maxHp)
        {
            currHp = maxHp;
        }
    }

    /// <summary>
    /// 데미지 적용
    /// </summary>
    /// <param name="_damage"></param>
    public void Damage(int _damage)
    {
        currHp -= _damage;
        if (currHp <= 0)
        {
            currHp = 0;
            gameObject.SetActive(false);
        }
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
    }

    /// <summary>
    /// 버프 여러 개 추가
    /// </summary>
    /// <param name="_newBuffs"></param>
    public void AddBuffs(List<BuffData> _newBuffs)
    {
        for (int i = 0; i < _newBuffs.Count; ++i)
        {
            AddBuff(_newBuffs[i]);
        }
    }

    /// <summary>
    /// 에너지 값 가져오기
    /// </summary>
    /// <returns></returns>
    public int GetEnergy()
    {
        return energy;
    }

    /// <summary>
    /// 에너지 증가
    /// </summary>
    /// <param name="_energy"></param>
    public void AddEnergy(int _energy)
    {
        energy += _energy;
    }

    /// <summary>
    /// 에너지 감소
    /// </summary>
    /// <param name="_energy"></param>
    public void ReduceEnergy(int _energy)
    {
        energy -= _energy;
    }

    /// <summary>
    /// 에너지 비우기
    /// </summary>
    public void ClearEnergy()
    {
        energy = 0;
    }
}
