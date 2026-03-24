using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private CombatSystem _combat;

    public void Init(CombatSystem combat)
    {
        _combat = combat;
    }

    public void TakeTurn(Unit enemy, Unit player)
    {
        bool isLowHP = (float)enemy.currentHP / enemy.maxHP < 0.3f;
        Ability healAbility = enemy.abilities.Find(a => a.healAmount > 0);

        if (isLowHP && healAbility != null)
        {
            _combat.UseAbility(healAbility, enemy, enemy);
            return;
        }

        Ability damageAbility = enemy.abilities.Find(a => a.baseDamage > 0);
        if (damageAbility != null && Random.value < 0.3f)
            _combat.UseAbility(damageAbility, enemy, player);
        else
            _combat.CalculatePhysicalDamage(enemy, player);
    }
}