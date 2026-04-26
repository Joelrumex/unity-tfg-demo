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
    public List<ActOption> actOptions = new List<ActOption>();
    public ObserveOption observeOption;    // ← drag per-enemy observe asset here
    public float mercyBar = 0f;
    public bool mercyAvailable = false;
    public Vector2 ghostScale = Vector2.one;

    [Header("2D References")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    public virtual void InitUnit()
    {
        if (isPlayerUnit)
        {
            currentHP = PlayerManager.Instance.playerHealth;
        }
        else
        {
            currentHP = maxHP;
        }

        mercyBar = 0f;
        mercyAvailable = false;

        foreach (var act in actOptions)
            act.useCount = 0;
    }

    // Returns true if mercy bar just became full
    public MercyPhase GetCurrentPhase()
    {
        if (mercyBar < 33f) return MercyPhase.Phase1;
        if (mercyBar < 67f) return MercyPhase.Phase2;
        return MercyPhase.Phase3;
    }

    // Returns only the actions available in the current phase
    public List<ActOption> GetCurrentPhaseActions()
    {
        MercyPhase current = GetCurrentPhase();
        return actOptions.FindAll(a => a.phase == current);
    }

    // Returns the observe hints for the current phase
    public string[] GetObserveHints()
    {
        if (observeOption == null) return new[] { "Nothing seems out of the ordinary..." };

        return GetCurrentPhase() switch
        {
            MercyPhase.Phase1 => observeOption.phase1Hints,
            MercyPhase.Phase2 => observeOption.phase2Hints,
            _ => observeOption.phase3Hints
        };
    }

    public bool ApplyMercyGain(ActOption act)
    {
        act.useCount++;
        if (act.useCount < act.requiredUses) return false;

        mercyBar = Mathf.Min(100f, mercyBar + act.mercyGain);
        if (mercyBar >= 100f && !mercyAvailable)
        {
            mercyAvailable = true;
            return true;
        }
        return false;
    }


    public int TakeDamage(int rawDamage)
    {
        int damage = Mathf.Max(1, rawDamage - defense);
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        if (isPlayerUnit)
        {
            PlayerManager.Instance.playerHealth = currentHP;
        }
        Debug.Log($"{unitName} took {damage} damage. HP: {currentHP}/{maxHP}");
        return damage;
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);
        if (isPlayerUnit)
        {
            PlayerManager.Instance.playerHealth = currentHP;
        }
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
