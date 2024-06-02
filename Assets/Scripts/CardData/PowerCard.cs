using UnityEngine;

[CreateAssetMenu(fileName = "PowerCard", menuName = "Scriptable Object/Card/PowerCard")]
public class PowerCard : CardData
{
    private void OnEnable()
    {
        type = CARD_TYPE.POWER;
    }
}
