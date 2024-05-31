using UnityEngine;

[CreateAssetMenu(fileName = "SkillCard", menuName = "Scriptable Object/Card/SkillCard")]
public class SkillCard : CardData
{
    private void OnEnable()
    {
        type = Type.Skill;
    }
}
