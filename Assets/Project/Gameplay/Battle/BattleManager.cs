using System.Collections;
using UnityEngine;

public enum BattleState { START, PLAYER_TURN, ENEMY_TURN, WIN, LOSE }

public class BattleManager : MonoBehaviour
{
    [Header("References")]
    public TurnManager turnManager;
    public CombatSystem combatSystem;
    public EnemyAI enemyAI;
    public BattleUI battleUI;
    public QTEManager qteManager;    // ← add this

    [Header("Units")]
    public Unit player;
    public Unit enemy;
    public BattleState state;
    void Start()
    {
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        state = BattleState.START;
        player.InitUnit();
        enemy.InitUnit();
        turnManager.Init(player, enemy);
        enemyAI.Init(combatSystem);
        battleUI.Init(this);

        // Subscribe to QTE result
        qteManager.OnQTEComplete += OnQTEComplete;

        battleUI.UpdateHPBars(player, enemy);
        yield return new WaitForSeconds(1f);
        StartNextTurn();
    }

    void StartNextTurn()
    {
        if (enemy.IsDead())  { EndBattle(BattleState.WIN);  return; }
        if (player.IsDead()) { EndBattle(BattleState.LOSE); return; }

        if (turnManager.IsPlayerTurn())
        {
            state = BattleState.PLAYER_TURN;
            battleUI.ShowActionMenu();
        }
        else
        {
            state = BattleState.ENEMY_TURN;
            StartCoroutine(RunEnemyTurn());
        }
    }

    IEnumerator RunEnemyTurn()
    {
        battleUI.HideActionMenu();
        yield return new WaitForSeconds(0.5f);

        // Store pending damage — apply after QTE resolves
        _pendingEnemyDamage = enemyAI.CalculateDamage(enemy, player);

        // Launch QTE
        qteManager.StartQTE();
        // Execution continues in OnQTEComplete callback
    }

    private int _pendingEnemyDamage = 0;

    void OnQTEComplete(QTEResult result)
    {
        // Reduce damage based on QTE performance
        int finalDamage = result switch
        {
            QTEResult.Perfect => 0,                              // Full block
            QTEResult.Good    => Mathf.RoundToInt(_pendingEnemyDamage * 0.5f),  // Half damage
            QTEResult.Failed  => _pendingEnemyDamage,            // Full damage
            _                 => _pendingEnemyDamage
        };

        if (finalDamage > 0)
            player.TakeDamage(finalDamage);   // Bypass the lunge animation

        battleUI.UpdateHPBars(player, enemy);
        turnManager.AdvanceTurn();
        StartNextTurn();
    }

    public void PlayerAttack()
    {
        if (state != BattleState.PLAYER_TURN) return;
        combatSystem.CalculatePhysicalDamage(player, enemy);
        battleUI.UpdateHPBars(player, enemy);
        battleUI.HideActionMenu();
        turnManager.AdvanceTurn();
        StartNextTurn();
    }

    public void PlayerUseAbility(Ability ability)
    {
        if (state != BattleState.PLAYER_TURN) return;
        Unit target = ability.targetsEnemy ? enemy : player;
        combatSystem.UseAbility(ability, player, target);
        battleUI.UpdateHPBars(player, enemy);
        battleUI.HideActionMenu();
        turnManager.AdvanceTurn();
        StartNextTurn();
    }

    void EndBattle(BattleState result)
    {
        state = result;
        qteManager.OnQTEComplete -= OnQTEComplete;   // Clean up listener
        battleUI.HideActionMenu();
        battleUI.ShowResult(result == BattleState.WIN ? "Victory!" : "Defeat...");
    }
}