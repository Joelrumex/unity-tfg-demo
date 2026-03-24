using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [Header("HP Bars")]
    public Slider playerHPBar;
    public Slider enemyHPBar;

    [Header("Panels & Text")]
    public GameObject actionMenu;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI turnLabel;

    public void UpdateHPBars(Unit player, Unit enemy)
    {
        playerHPBar.value = (float)player.currentHP / player.maxHP;
        enemyHPBar.value  = (float)enemy.currentHP  / enemy.maxHP;
    }

    public void ShowActionMenu()
    {
        actionMenu.SetActive(true);
        if (turnLabel != null) turnLabel.text = "Your turn!";
    }

    public void HideActionMenu()
    {
        actionMenu.SetActive(false);
    }

    public void ShowResult(string message)
    {
        resultText.gameObject.SetActive(true);
        resultText.text = message;
    }
}