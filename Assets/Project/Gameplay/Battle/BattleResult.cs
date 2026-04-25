using UnityEngine;

public enum BattleOutcome { None, Killed, Mercy }

[CreateAssetMenu(fileName = "BattleResult", menuName = "RPG/BattleResult")]
public class BattleResult : ScriptableObject
{
    public BattleOutcome lastOutcome = BattleOutcome.None;
    public float shieldChance = 0.35f;
    public int bonusAttack = 5;
    public int bonusDefense = 3;
    public int bonusSpeed = 2;

    [Header("Mercy Ghost")]
    public Sprite merciedEnemySprite;
    public Vector2 merciedEnemyGhostScale = Vector2.one;

    public void RecordMercy(Sprite enemySprite, Vector2 ghostScale)
    {
        lastOutcome = BattleOutcome.Mercy;
        merciedEnemySprite = enemySprite;
        merciedEnemyGhostScale = ghostScale;
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