using UnityEngine;

[CreateAssetMenu(fileName = "AttackCard", menuName = "Scriptable Object/Card/AttackCard")]
public class AttackCard : CardData
{
    public int damage;

    private void OnEnable()
    {
        type = CARD_TYPE.ATTACK;
    }
}
