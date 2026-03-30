using System.Collections;
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

    [Header("Mercy System")]
    public List<ActOption> actOptions = new List<ActOption>();  // Assign in Inspector per enemy
    public float mercyBar = 0f;             // 0 to 100
    public bool mercyAvailable = false;

    [Header("2D References")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    public virtual void InitUnit()
    {
        currentHP = maxHP;
        mercyBar = 0f;
        mercyAvailable = false;

        // Reset all ACT use counts for a fresh battle
        foreach (var act in actOptions)
            act.useCount = 0;
    }

    // Returns true if mercy bar just became full
    public bool ApplyMercyGain(ActOption act)
    {
        act.useCount++;

        // Only grant mercy if the action has been used enough times
        if (act.useCount >= act.requiredUses)
        {
            mercyBar = Mathf.Min(100f, mercyBar + act.mercyGain);
            if (mercyBar >= 100f && !mercyAvailable)
            {
                mercyAvailable = true;
                return true;   // Signal that mercy just unlocked
            }
        }
        return false;
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
    }

    public bool IsDead() => currentHP <= 0;

    IEnumerator HurtFlash()
    {
        if (spriteRenderer == null) yield break;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        spriteRenderer.color = Color.white;
    }
}
