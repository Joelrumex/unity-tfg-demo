using UnityEngine;

[CreateAssetMenu(fileName = "NewAbility", menuName = "RPG/Ability")]
public class Ability : ScriptableObject
{
    public string abilityName = "Attack";
    public int baseDamage = 0;
    public int healAmount = 0;
    public bool targetsEnemy = true;   // false = targets ally
    [TextArea] public string description;
}