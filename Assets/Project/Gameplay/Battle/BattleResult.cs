using UnityEngine;

public enum BattleOutcome { None, Killed, Mercy }

[CreateAssetMenu(fileName = "BattleResult", menuName = "RPG/BattleResult")]
public class BattleResult : ScriptableObject
{
    public BattleOutcome lastOutcome = BattleOutcome.None;

    // Mercy reward
    public float shieldChance = 0.35f;      // 35% chance to block an attack

    // Kill reward — flat stat bonuses applied on next battle start
    public int bonusAttack  = 5;
    public int bonusDefense = 3;
    public int bonusSpeed   = 2;

    public void RecordMercy() => lastOutcome = BattleOutcome.Mercy;
    public void RecordKill()  => lastOutcome = BattleOutcome.Killed;
    public void Clear()       => lastOutcome = BattleOutcome.None;
}