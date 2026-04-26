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
    public TextMeshProUGUI promptText;

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

        foreach (var b in _actButtons) Destroy(b);
        _actButtons.Clear();

        // ── Observe button — always first ──────────────────────
        var observeGO = Instantiate(actButtonPrefab, actButtonsParent);
        var observeBtn = observeGO.GetComponent<Button>();
        var observeLabel = observeGO.GetComponentInChildren<TextMeshProUGUI>();
        observeLabel.text = "👁 Observe";
        observeLabel.color = new Color(0.95f, 0.85f, 0.3f);   // Gold color
        observeBtn.onClick.AddListener(() =>
        {
            actMenuPanel.SetActive(false);
            _bm.PlayerObserve();
        });
        _actButtons.Add(observeGO);

        // ── Phase-specific actions ──────────────────────────────
        foreach (var act in _bm.enemy.GetCurrentPhaseActions())
        {
            var go = Instantiate(actButtonPrefab, actButtonsParent);
            var btn = go.GetComponent<Button>();
            var label = go.GetComponentInChildren<TextMeshProUGUI>();
            label.text = act.actionName;

            var capturedAct = act;
            btn.onClick.AddListener(() =>
            {
                actMenuPanel.SetActive(false);
                _bm.PlayerUseAct(capturedAct);
            });
            _actButtons.Add(go);
        }

        // ── Back button — always last ───────────────────────────
        var backGO = Instantiate(actButtonPrefab, actButtonsParent);
        var backBtn = backGO.GetComponent<Button>();
        backGO.GetComponentInChildren<TextMeshProUGUI>().text = "← Back";
        backBtn.onClick.AddListener(() =>
        {
            actMenuPanel.SetActive(false);
            ShowMainMenu(_bm.enemy.mercyAvailable);
        });
        _actButtons.Add(backGO);
    }

    public void ShowPhaseChange(MercyPhase phase)
    {
        string message = phase switch
        {
            MercyPhase.Phase2 => "Something is shifting...",
            MercyPhase.Phase3 => "You can feel the tension lifting...",
            _ => ""
        };
        if (message != "") StartCoroutine(FlashPhaseMessage(message));
    }

    IEnumerator FlashPhaseMessage(string message)
    {
        // Reuse the bonusAnnouncerText or result text briefly
        bonusAnnouncerText.text = message;
        bonusAnnouncerText.color = new Color(0.6f, 0.85f, 1f, 1f);
        bonusAnnouncerText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        bonusAnnouncerText.gameObject.SetActive(false);
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
        _isShowingDialogue = true;
        dialoguePanel.SetActive(true);

        foreach (var line in lines)
        {
            // Typewrite the line character by character
            dialogueText.text = "";
            bool finishedTyping = false;

            StartCoroutine(TypewriteLine(line, () => finishedTyping = true));

            // If player presses E before typing finishes — skip to full line instantly
            while (!finishedTyping)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    StopCoroutine(nameof(TypewriteLine));
                    dialogueText.text = line;
                    finishedTyping = true;
                }
                yield return null;
            }

            // Show the "press E" indicator then wait for input
            promptText.gameObject.SetActive(true);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
            promptText.gameObject.SetActive(false);

            yield return null;   // Wait one frame so GetKeyDown doesn't fire twice
        }

        _isShowingDialogue = false;
    }

    IEnumerator TypewriteLine(string line, System.Action onComplete)
    {
        dialogueText.text = "";
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.04f);
        }
        onComplete?.Invoke();
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