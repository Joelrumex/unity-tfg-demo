using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [Header("HP Bars")]
    public Slider playerHPBar;
    public Slider enemyHPBar;

    [Header("Mercy Bar")]
    public Slider mercyBar;               // A second slider under the enemy HP bar
    public GameObject mercyBarRoot;       // Parent object to show/hide

    [Header("Main Menu")]
    public GameObject mainMenuPanel;      // Contains the 4 main buttons
    public Button attackButton;
    public Button actButton;
    public Button mercyButton;            // Hidden until mercy bar is full

    [Header("ACT Menu")]
    public GameObject actMenuPanel;       // Shown when player clicks ACT
    public Transform actButtonsParent;    // Vertical layout group
    public GameObject actButtonPrefab;    // Simple button prefab with TextMeshPro

    [Header("Dialogue")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    [Header("Result & Labels")]
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI turnLabel;

    private BattleManager _bm;
    private List<GameObject> _actButtons = new List<GameObject>();

    public void Init(BattleManager bm)
    {
        _bm = bm;
        attackButton.onClick.AddListener(() => _bm.PlayerAttack());
        actButton.onClick.AddListener(() => ShowActMenu());
        mercyButton.onClick.AddListener(() => _bm.PlayerGrantMercy());

        mercyButton.gameObject.SetActive(false);
        actMenuPanel.SetActive(false);
        dialoguePanel.SetActive(false);
        resultText.gameObject.SetActive(false);
    }

    // ── Main menu ────────────────────────────────────────────

    public void ShowMainMenu(bool mercyUnlocked)
    {
        mainMenuPanel.SetActive(true);
        if (turnLabel != null) turnLabel.text = "Your turn!";
        mercyButton.gameObject.SetActive(mercyUnlocked);
    }

    public void HideMainMenu()
    {
        mainMenuPanel.SetActive(false);
        actMenuPanel.SetActive(false);
    }

    // ── ACT menu ─────────────────────────────────────────────

    void ShowActMenu()
    {
        actMenuPanel.SetActive(true);
        mainMenuPanel.SetActive(false);

        // Clear old buttons
        foreach (var b in _actButtons) Destroy(b);
        _actButtons.Clear();

        // Spawn one button per ACT option
        foreach (var act in _bm.enemy.actOptions)
        {
            var go = Instantiate(actButtonPrefab, actButtonsParent);
            var btn = go.GetComponent<Button>();
            var label = go.GetComponentInChildren<TextMeshProUGUI>();
            label.text = act.actionName;

            // Capture act in closure
            var capturedAct = act;
            btn.onClick.AddListener(() => {
                actMenuPanel.SetActive(false);
                _bm.PlayerUseAct(capturedAct);
            });
            _actButtons.Add(go);
        }

        // Back button
        var backGO = Instantiate(actButtonPrefab, actButtonsParent);
        var backBtn = backGO.GetComponent<Button>();
        var backLabel = backGO.GetComponentInChildren<TextMeshProUGUI>();
        backLabel.text = "← Back";
        backBtn.onClick.AddListener(() => {
            actMenuPanel.SetActive(false);
            ShowMainMenu(_bm.enemy.mercyAvailable);
        });
        _actButtons.Add(backGO);
    }

    // ── Mercy bar ─────────────────────────────────────────────

    public void UpdateMercyBar(float value)
    {
        // value is 0-100
        mercyBar.value = value / 100f;
    }

    public void ShowMercyUnlocked()
    {
        mercyButton.gameObject.SetActive(true);
        StartCoroutine(FlashMercyButton());
    }

    IEnumerator FlashMercyButton()
    {
        var img = mercyButton.GetComponent<Image>();
        for (int i = 0; i < 6; i++)
        {
            img.color = Color.yellow;
            yield return new WaitForSeconds(0.15f);
            img.color = Color.white;
            yield return new WaitForSeconds(0.15f);
        }
    }

    // ── Dialogue ──────────────────────────────────────────────

    public void ShowDialogue(string[] lines)
    {
        dialoguePanel.SetActive(true);
        StartCoroutine(TypewriteDialogue(lines));
    }

    IEnumerator TypewriteDialogue(string[] lines)
    {
        foreach (var line in lines)
        {
            dialogueText.text = "";
            foreach (char c in line)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(0.04f);   // Typewriter speed
            }
            yield return new WaitForSeconds(0.8f);        // Pause between lines
        }
    }

    public void HideDialogue()
    {
        StopCoroutine(nameof(TypewriteDialogue));
        dialoguePanel.SetActive(false);
    }

    // ── HP bars & result ──────────────────────────────────────

    public void UpdateHPBars(Unit player, Unit enemy)
    {
        if (player == null || enemy == null)
        {
            Debug.LogError("BattleUI: Player or Enemy is null!");
            return;
        }
        if (playerHPBar != null)
            playerHPBar.value = (float)player.currentHP / player.maxHP;
        if (enemyHPBar != null)
            enemyHPBar.value  = (float)enemy.currentHP  / enemy.maxHP;
    }

    public void ShowResult(string message)
    {
        resultText.gameObject.SetActive(true);
        resultText.text = message;
    }
}