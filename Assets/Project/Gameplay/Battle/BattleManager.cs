using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; 

public enum BattleState { START, PLAYER_TURN, ENEMY_TURN, WIN, LOSE }

public class BattleManager : MonoBehaviour
{
    [Header("References")]
    public TurnManager turnManager;
    public CombatSystem combatSystem;
    public EnemyAI enemyAI;
    public BattleUI battleUI;
    public QTEManager qteManager;
    public MercySystem mercySystem;
    public BattleResult battleResult; 

    [Header("Units")]
    public Unit player;
    public Unit enemy;

    public BattleState state;
    private int _pendingEnemyDamage = 0;

    void Start()
    {
        Debug.Log("BattleManager Start called");
        StartCoroutine(SetupBattle());
    }
    IEnumerator SetupBattle()
    {
        state = BattleState.START;
        player.InitUnit();
        enemy.InitUnit();

        ApplyPreviousBattleEffects();

        turnManager.Init(player, enemy);
        enemyAI.Init(combatSystem);
        battleUI.Init(this);

        qteManager.OnQTEComplete += OnQTEComplete;

        // Wire mercy events
        mercySystem.OnActUsed         += lines => battleUI.ShowDialogue(lines);
        mercySystem.OnMercyBarUpdated += val   => battleUI.UpdateMercyBar(val);
        mercySystem.OnMercyUnlocked   +=  ()   => battleUI.ShowMercyUnlocked();
        mercySystem.OnMercyGranted    +=  ()   => EndBattle(BattleState.WIN,wasMercy: true);

        battleUI.UpdateHPBars(player, enemy);
        battleUI.UpdateMercyBar(0f);

        battleUI.ShowBattleStartBonus(battleResult.lastOutcome);


        yield return new WaitForSeconds(1f);
         battleResult.Clear();
        StartNextTurn();
    }

    void ApplyPreviousBattleEffects()
    {
        switch (battleResult.lastOutcome)
        {
            case BattleOutcome.Mercy:
                // Shield is handled in OnQTEComplete — no stat change needed
                Debug.Log($"Mercy bonus active: {battleResult.shieldChance * 100}% shield chance");
                break;

            case BattleOutcome.Killed:
                player.attack  += battleResult.bonusAttack;
                player.defense += battleResult.bonusDefense;
                player.speed   += battleResult.bonusSpeed;
                Debug.Log($"Kill bonus applied: +{battleResult.bonusAttack} ATK, " +
                          $"+{battleResult.bonusDefense} DEF, +{battleResult.bonusSpeed} SPD");
                break;
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

    void StartNextTurn()
    {
        if (state == BattleState.ENEMY_TURN) return;
        if (enemy.IsDead())  { EndBattle(BattleState.WIN,  wasMercy: false); return; }
        if (player.IsDead()) { EndBattle(BattleState.LOSE, wasMercy: false); return; }

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
            state = BattleState.START;
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

        if (finalDamage > 0 && battleResult.lastOutcome == BattleOutcome.Mercy)
        {
            bool shielded = Random.value < battleResult.shieldChance;
            if (shielded)
            {
                finalDamage = 0;
                battleUI.ShowShieldPopup();   // Visual feedback
                Debug.Log("Shield triggered from mercy carry-over!");
            }
        }

        if (finalDamage > 0) player.TakeDamage(finalDamage);
        battleUI.UpdateHPBars(player, enemy);
        state = BattleState.START;   // Clear ENEMY_TURN lock before advancing
        turnManager.AdvanceTurn();
        StartNextTurn();
    }

    // ── End ─────────────────────────────────────────────────

    void EndBattle(BattleState result, bool wasMercy)
    {
        state = result;
        qteManager.OnQTEComplete -= OnQTEComplete;
        battleUI.HideMainMenu();

        if (result == BattleState.WIN)
        {
            if (wasMercy)
            {
                battleResult.RecordMercy();
                battleUI.ShowResult("You showed mercy.\nYour kindness will protect you.");
            }
            else
            {
                battleResult.RecordKill();
                battleUI.ShowResult("Enemy defeated!\nYou grow stronger.");
            }
        }
        else
        {
            battleUI.ShowResult("Defeat...");
        }

        StartCoroutine(LoadNextScene(result));
    }

    IEnumerator LoadNextScene(BattleState result)
    {
        yield return new WaitForSeconds(2.5f); 

        if (result == BattleState.WIN)
            SceneManager.LoadScene("BattleScene1");
        else
            SceneManager.LoadScene("PlayerMovement");
    }
}