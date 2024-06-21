using UnityEngine;

[CreateAssetMenu(fileName = "StatusCard", menuName = "Scriptable Object/Card/StatusCard")]
public class StatusCard : CardData
{
    private void OnEnable()
    {
        type = CARD_TYPE.STATUS;
    }
}
