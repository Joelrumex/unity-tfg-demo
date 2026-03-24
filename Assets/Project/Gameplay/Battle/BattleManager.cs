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

        battleUI.UpdateHPBars(player, enemy);

        yield return new WaitForSeconds(1f);
        StartNextTurn();
    }

    void StartNextTurn()
    {
        if (enemy.IsDead()) { EndBattle(BattleState.WIN);  return; }
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
        yield return new WaitForSeconds(0.8f);
        enemyAI.TakeTurn(enemy, player);
        battleUI.UpdateHPBars(player, enemy);
        yield return new WaitForSeconds(0.5f);
        turnManager.AdvanceTurn();
        StartNextTurn();
    }

    // Called by UI Attack button
    public void PlayerAttack()
    {
        if (state != BattleState.PLAYER_TURN) return;
        combatSystem.CalculatePhysicalDamage(player, enemy);
        battleUI.UpdateHPBars(player, enemy);
        AfterPlayerAction();
    }

    // Called by UI Ability buttons
    public void PlayerUseAbility(Ability ability)
    {
        if (state != BattleState.PLAYER_TURN) return;
        // Damage abilities target the enemy; heal abilities target the player
        Unit target = ability.targetsEnemy ? enemy : player;
        combatSystem.UseAbility(ability, player, target);
        battleUI.UpdateHPBars(player, enemy);
        AfterPlayerAction();
    }

    void AfterPlayerAction()
    {
        battleUI.HideActionMenu();
        turnManager.AdvanceTurn();
        StartNextTurn();
    }

    void EndBattle(BattleState result)
    {
        state = result;
        battleUI.HideActionMenu();
        battleUI.ShowResult(result == BattleState.WIN ? "Victory!" : "Defeat...");
    }
}