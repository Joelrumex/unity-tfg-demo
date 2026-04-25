using UnityEngine;

public enum BattleOutcome { None, Killed, Mercy }

[CreateAssetMenu(fileName = "BattleResult", menuName = "RPG/BattleResult")]
public class BattleResult : ScriptableObject
{
    public BattleOutcome lastOutcome = BattleOutcome.None;
    public float shieldChance = 0.35f;
    public int bonusAttack  = 5;
    public int bonusDefense = 3;
    public int bonusSpeed   = 2;

    public Sprite merciedEnemySprite; 

    public void RecordMercy(Sprite enemySprite)
    {
        lastOutcome = BattleOutcome.Mercy;
        merciedEnemySprite = enemySprite;
    }

    public void RecordKill()
    {
        lastOutcome = BattleOutcome.Killed;
        merciedEnemySprite = null;
    }

    public void Clear()
    {
        lastOutcome = BattleOutcome.None;
        merciedEnemySprite = null;
    }
}