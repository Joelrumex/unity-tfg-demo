using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Identity")]
    public string unitName = "Unit";
    public bool isPlayerUnit = true;

    [Header("Stats")]
    public int maxHP = 100;
    public int currentHP;
    public int attack = 20;
    public int defense = 10;
    public int speed = 10;

    [Header("Abilities")]
    public List<Ability> abilities = new List<Ability>();

    // Called once at battle start
    public virtual void InitUnit()
    {
        currentHP = maxHP;
    }

    public int TakeDamage(int rawDamage)
    {
        int damage = Mathf.Max(1, rawDamage - defense);
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        Debug.Log($"{unitName} took {damage} damage. HP: {currentHP}/{maxHP}");
        return damage;
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);
        Debug.Log($"{unitName} healed {amount}. HP: {currentHP}/{maxHP}");
    }

    public bool IsDead() => currentHP <= 0;
}