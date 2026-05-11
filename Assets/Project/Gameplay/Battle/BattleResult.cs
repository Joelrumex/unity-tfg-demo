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


    private static int _totalKills = 0;
    private static int _totalMercies = 0;
    public int TotalKills => _totalKills;
    public int TotalMercies => _totalMercies;


    [Header("Mercy Ghost")]
    public Sprite merciedEnemySprite;
    public Vector2 merciedEnemyGhostScale = Vector2.one;

    public void RecordKill()
    {
        lastOutcome = BattleOutcome.Killed;
        merciedEnemySprite = null;
        _totalKills++;
    }

    public void RecordMercy(Sprite enemySprite, Vector2 ghostScale)
    {
        lastOutcome = BattleOutcome.Mercy;
        merciedEnemySprite = enemySprite;
        merciedEnemyGhostScale = ghostScale;
        _totalMercies++;
    }


    public bool IsPacifistEnding() => _totalKills == 0 && _totalMercies > 0;
    public bool IsGenocideEnding() => _totalKills > 0;

    public void FullReset()
    {
        lastOutcome = BattleOutcome.None;
        merciedEnemySprite = null;
        _totalKills = 0;
        _totalMercies = 0;
    }
}