using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QTEKeySlot : MonoBehaviour
{
    [Header("References")]
    public Image background;        // The key box background
    public TextMeshProUGUI keyLabel; // Shows "W", "A", "S" or "D"
    public Image timerBar;          // Fills left-to-right, shrinks as time runs out

    [Header("Colors")]
    public Color idleColor    = new Color(0.2f, 0.2f, 0.2f);
    public Color activeColor  = new Color(0.9f, 0.8f, 0.1f);
    public Color successColor = Color.green;
    public Color failColor    = Color.red;

    public void SetKey(KeyCode key)
    {
        keyLabel.text = key.ToString();   // "W", "A", "S", "D"
        background.color = idleColor;
        timerBar.fillAmount = 1f;
        timerBar.gameObject.SetActive(false);
    }

    public void SetActive(bool active)
    {
        background.color = active ? activeColor : idleColor;
        timerBar.gameObject.SetActive(active);
    }

    public void SetTimer(float normalizedValue)
    {
        timerBar.fillAmount = normalizedValue;
        // Bar turns red as time runs out
        timerBar.color = Color.Lerp(Color.red, Color.green, normalizedValue);
    }

    public void ShowSuccess()
    {
        background.color = successColor;
        timerBar.gameObject.SetActive(false);
    }

    public void ShowFail()
    {
        background.color = failColor;
        timerBar.gameObject.SetActive(false);
    }
}