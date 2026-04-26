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
    private bool _isShowingDialogue = false;
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

    [Header("Turn Info")]
    public TextMeshProUGUI turnText;

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

    [Header("Bonus Feedback")]
    public GameObject shieldPopup;
    public TextMeshProUGUI bonusAnnouncerText;

    // ── New methods to add ────────────────────────────────────────

    public void ShowShieldPopup()
    {
        Debug.Log("SHIELD TRIGGERED!");
        StartCoroutine(FlashPopup(shieldPopup));
    }

    IEnumerator FlashPopup(GameObject popup)
    {
        if (popup == null) yield break;
        popup.SetActive(true);
        yield return new WaitForSeconds(1.2f);
        popup.SetActive(false);
    }

    public void UpdateTurnText(int turnNumber, bool isPlayerTurn)
    {
        if (turnText == null) return;
        string whose = isPlayerTurn ? "Your turn" : "Enemy turn";
        turnText.text = $"Turn {turnNumber} : {whose}";
    }

    public void ShowBattleStartBonus(BattleOutcome outcome)
    {
        if (bonusAnnouncerText == null) return;

        switch (outcome)
        {
            case BattleOutcome.Mercy:
                bonusAnnouncerText.text = "✦ Mercy shield active this battle";
                bonusAnnouncerText.color = Color.cyan;
                break;
            case BattleOutcome.Killed:
                bonusAnnouncerText.text = "✦ Stats boosted from last victory";
                bonusAnnouncerText.color = Color.yellow;
                break;
            default:
                bonusAnnouncerText.gameObject.SetActive(false);
                return;
        }

        bonusAnnouncerText.gameObject.SetActive(true);
        StartCoroutine(FadeOutAnnouncer());
    }

    IEnumerator FadeOutAnnouncer()
    {
        yield return new WaitForSeconds(2f);
        // Fade out over 0.5s
        float t = 0f;
        Color original = bonusAnnouncerText.color;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            bonusAnnouncerText.color = new Color(original.r, original.g, original.b, 1f - (t / 0.5f));
            yield return null;
        }
        bonusAnnouncerText.gameObject.SetActive(false);
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
            btn.onClick.AddListener(() =>
            {
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
        backBtn.onClick.AddListener(() =>
        {
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
        _isShowingDialogue = true;
        dialoguePanel.SetActive(true);
        StartCoroutine(TypewriteDialogue(lines));
    }

    public void HideDialogue()
    {
        _isShowingDialogue = false;
        StopCoroutine(nameof(TypewriteDialogue));
        dialoguePanel.SetActive(false);
    }

    public bool IsShowingDialogue() => _isShowingDialogue;

    IEnumerator TypewriteDialogue(string[] lines)
    {
        foreach (var line in lines)
        {
            dialogueText.text = "";
            foreach (char c in line)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(0.04f);
            }
            yield return new WaitForSeconds(0.8f);
        }
        _isShowingDialogue = false;   // ← mark as done when typewriter finishes
    }

    public void UpdateHPBars(Unit player, Unit enemy)
    {
        playerHPBar.value = (float)player.currentHP / player.maxHP;
        enemyHPBar.value = (float)enemy.currentHP / enemy.maxHP;
    }

    public void ShowResult(string message)
    {
        resultText.gameObject.SetActive(true);
        resultText.text = message;
    }
}