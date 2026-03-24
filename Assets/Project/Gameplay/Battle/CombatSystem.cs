using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public int CalculatePhysicalDamage(Unit attacker, Unit defender)
    {
        float variance = Random.Range(0.9f, 1.1f);
        int raw = Mathf.RoundToInt(attacker.attack * variance);
        return defender.TakeDamage(raw);
    }

    public void UseAbility(Ability ability, Unit caster, Unit target)
    {
        if (ability.baseDamage > 0)
        {
            float variance = Random.Range(0.9f, 1.1f);
            int raw = Mathf.RoundToInt((caster.attack + ability.baseDamage) * variance);
            int dealt = target.TakeDamage(raw);
            Debug.Log($"{caster.unitName} used {ability.abilityName} for {dealt} damage on {target.unitName}!");
        }

        if (ability.healAmount > 0)
        {
            target.Heal(ability.healAmount);
            Debug.Log($"{caster.unitName} used {ability.abilityName} and healed {target.unitName} for {ability.healAmount}!");
        }
    }
}