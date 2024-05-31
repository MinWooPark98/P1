using UnityEngine;

[CreateAssetMenu(fileName = "CurseCard", menuName = "Scriptable Object/Card/CurseCard")]
public class CurseCard : CardData
{
    private void OnEnable()
    {
        type = Type.Curse;
    }
}
