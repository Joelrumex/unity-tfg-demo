using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    private CombatSystem _combat;

    public void Init(CombatSystem combat) => _combat = combat;

    // Returns raw damage without applying it
    public int CalculateDamage(Unit enemy, Unit player)
    {
        bool isLowHP = (float)enemy.currentHP / enemy.maxHP < 0.3f;
        Ability healAbility = enemy.abilities.Find(a => a.healAmount > 0);

        if (isLowHP && healAbility != null)
        {
            _combat.UseAbility(healAbility, enemy, enemy);
            return 0;   // No damage this turn — enemy healed
        }

        Ability damageAbility = enemy.abilities.Find(a => a.baseDamage > 0);
        float variance = Random.Range(0.9f, 1.1f);

        int raw = (damageAbility != null && Random.value < 0.3f)
            ? Mathf.RoundToInt((enemy.attack + damageAbility.baseDamage) * variance)
            : Mathf.RoundToInt(enemy.attack * variance);

        return Mathf.Max(1, raw - player.defense);
    }
}