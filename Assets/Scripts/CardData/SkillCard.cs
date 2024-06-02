using UnityEngine;

[CreateAssetMenu(fileName = "SkillCard", menuName = "Scriptable Object/Card/SkillCard")]
public class SkillCard : CardData
{
    public int block;                       // ¹æ¾îµµ

    private void OnEnable()
    {
        type = CARD_TYPE.SKILL;
    }
}
