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
    public QTEManager qteManager;
    public MercySystem mercySystem;   // ← add this

    [Header("Units")]
    public Unit player;
    public Unit enemy;

    public BattleState state;
    private int _pendingEnemyDamage = 0;

    IEnumerator SetupBattle()
    {
        state = BattleState.START;
        player.InitUnit();
        enemy.InitUnit();
        turnManager.Init(player, enemy);
        enemyAI.Init(combatSystem);
        battleUI.Init(this);

        qteManager.OnQTEComplete += OnQTEComplete;

        // Wire mercy events
        mercySystem.OnActUsed         += lines => battleUI.ShowDialogue(lines);
        mercySystem.OnMercyBarUpdated += val   => battleUI.UpdateMercyBar(val);
        mercySystem.OnMercyUnlocked   +=  ()   => battleUI.ShowMercyUnlocked();
        mercySystem.OnMercyGranted    +=  ()   => EndBattle(BattleState.WIN);

        battleUI.UpdateHPBars(player, enemy);
        battleUI.UpdateMercyBar(0f);

        yield return new WaitForSeconds(1f);
        StartNextTurn();
    }

    void StartNextTurn()
    {
        if (state == BattleState.ENEMY_TURN) return;
        if (enemy.IsDead())  { EndBattle(BattleState.WIN);  return; }
        if (player.IsDead()) { EndBattle(BattleState.LOSE); return; }

        if (turnManager.IsPlayerTurn())
        {
            state = BattleState.PLAYER_TURN;
            battleUI.ShowMainMenu(enemy.mercyAvailable);
        }
        else
        {
            state = BattleState.ENEMY_TURN;
            StartCoroutine(RunEnemyTurn());
        }
    }

    // ── Player actions ──────────────────────────────────────

    public void PlayerAttack()
    {
        if (state != BattleState.PLAYER_TURN) return;
        combatSystem.CalculatePhysicalDamage(player, enemy);
        battleUI.UpdateHPBars(player, enemy);
        EndPlayerTurn();
    }

    public void PlayerUseAbility(Ability ability)
    {
        if (state != BattleState.PLAYER_TURN) return;
        Unit target = ability.targetsEnemy ? enemy : player;
        combatSystem.UseAbility(ability, player, target);
        battleUI.UpdateHPBars(player, enemy);
        EndPlayerTurn();
    }

    // Called when player selects an ACT option from the menu
    public void PlayerUseAct(ActOption act)
    {
        if (state != BattleState.PLAYER_TURN) return;
        mercySystem.UseActOption(act, enemy);
        // After dialogue is shown, player turn ends (BattleUI calls EndPlayerTurn via callback)
        StartCoroutine(EndTurnAfterDialogue());
    }

    // Called when player selects MERCY
    public void PlayerGrantMercy()
    {
        if (state != BattleState.PLAYER_TURN) return;
        if (!enemy.mercyAvailable) return;
        mercySystem.GrantMercy();
    }

    IEnumerator EndTurnAfterDialogue()
    {
        // Wait for dialogue to finish displaying before advancing turn
        yield return new WaitForSeconds(2.5f);
        battleUI.HideDialogue();
        EndPlayerTurn();
    }

    void EndPlayerTurn()
    {
        battleUI.HideMainMenu();
        turnManager.AdvanceTurn();
        StartNextTurn();
    }

    // ── Enemy turn ──────────────────────────────────────────

    IEnumerator RunEnemyTurn()
    {
        if (state != BattleState.ENEMY_TURN) yield break;
        battleUI.HideMainMenu();
        yield return new WaitForSeconds(0.5f);

        _pendingEnemyDamage = enemyAI.CalculateDamage(enemy, player);
        if (_pendingEnemyDamage > 0)
            qteManager.StartQTE();
        else
        {
            // Enemy healed itself — no QTE needed
            turnManager.AdvanceTurn();
            StartNextTurn();
        }
    }

    void OnQTEComplete(QTEResult result)
    {
        int finalDamage = result switch
        {
            QTEResult.Perfect => 0,
            QTEResult.Good    => Mathf.RoundToInt(_pendingEnemyDamage * 0.5f),
            _                 => _pendingEnemyDamage
        };

        if (finalDamage > 0) player.TakeDamage(finalDamage);
        battleUI.UpdateHPBars(player, enemy);
        state = BattleState.START;   // Clear ENEMY_TURN lock before advancing
        turnManager.AdvanceTurn();
        StartNextTurn();
    }

    // ── End ─────────────────────────────────────────────────

    void EndBattle(BattleState result)
    {
        state = result;
        qteManager.OnQTEComplete -= OnQTEComplete;
        battleUI.HideMainMenu();
        bool won = result == BattleState.WIN;
        battleUI.ShowResult(won ? "You showed mercy.\nBattle over." : "Defeat...");
    }
}