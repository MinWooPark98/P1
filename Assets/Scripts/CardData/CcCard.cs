using UnityEngine;

[CreateAssetMenu(fileName = "CcCard", menuName = "Scriptable Object/Card/CcCard")]
public class CcCard : CardData
{
    private void OnEnable()
    {
        type = CARD_TYPE.CC;
    }
}
