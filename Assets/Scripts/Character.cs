using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Character : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
                gameObject.SetActive(false);
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

    [SerializeField]
    private CharacterInfo characterInfo = null;

    [SerializeField]
    private System.Action actionClicked = null;

    private bool onPointer = false;


    public void Init(int _currHp, int _maxHp, int _defense, List<BuffData> _buffList)
    {
        CurrHp = _currHp;
        MaxHp = _maxHp;
        Defense = _defense;
        
        characterInfo.Init(_currHp, _maxHp, _buffList);
    }

    /// <summary>
    /// �ִ� ü�� ����
    /// </summary>
    /// <param name="_addMaxHp"></param>
    public void AddMaxHp(int _addMaxHp)
    {
        MaxHp += _addMaxHp;
    }

    /// <summary>
    /// ü�� ȸ��
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
    /// ������ ����
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

    /// <summary>
    /// �� ȹ��
    /// </summary>
    /// <param name="_defense"></param>
    public void AddDefense(int _defense)
    {
        Defense += _defense;
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
    public void AddBuff(List<BuffData> _newBuffs)
    {
        for (int i = 0; i < _newBuffs.Count; ++i)
        {
            AddBuff(_newBuffs[i]);
        }
    }
    public void SetActionClicked(System.Action _actionClicked)
    {
        actionClicked = _actionClicked;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointer = true;
        characterInfo.ShowBuff(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointer = false;
        characterInfo.ShowBuff(false);
    }

    public bool GetOnPointer() => onPointer;
}
