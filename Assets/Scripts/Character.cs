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
    /// �ִ� ü�� �Է�
    /// </summary>
    /// <param name="_maxHp"></param>
    public void SetMaxHp(int _maxHp)
    {
        maxHp = _maxHp;
    }

    /// <summary>
    /// �ִ� ü�� ����
    /// </summary>
    /// <param name="_addMaxHp"></param>
    public void AddMaxHp(int _addMaxHp)
    {
        maxHp += _addMaxHp;
    }

    /// <summary>
    /// ü�� ȸ��
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
    /// ������ ����
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
    /// ���� ���� �߰�
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
    /// ���� ���� �� �߰�
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
    /// ������ �� ��������
    /// </summary>
    /// <returns></returns>
    public int GetEnergy()
    {
        return energy;
    }

    /// <summary>
    /// ������ ����
    /// </summary>
    /// <param name="_energy"></param>
    public void AddEnergy(int _energy)
    {
        energy += _energy;
    }

    /// <summary>
    /// ������ ����
    /// </summary>
    /// <param name="_energy"></param>
    public void ReduceEnergy(int _energy)
    {
        energy -= _energy;
    }

    /// <summary>
    /// ������ ����
    /// </summary>
    public void ClearEnergy()
    {
        energy = 0;
    }
}
