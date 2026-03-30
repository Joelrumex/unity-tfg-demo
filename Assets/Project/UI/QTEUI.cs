using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QTEUI : MonoBehaviour
{
    [Header("QTE Panel")]
    public GameObject qtePanel;                  // The whole QTE overlay
    public Transform keyPromptsParent;           // Horizontal layout group holding key slots
    public GameObject keyPromptPrefab;           // Prefab for one key slot (see Step 3)
    public TextMeshProUGUI resultLabel;          // Shows "Perfect!" / "Good!" / "Failed!"

    private List<QTEKeySlot> _slots = new List<QTEKeySlot>();

    public void ShowQTE(List<KeyCode> sequence)
    {
        qtePanel.SetActive(true);
        resultLabel.gameObject.SetActive(false);

        // Clear old slots
        foreach (Transform child in keyPromptsParent)
            Destroy(child.gameObject);
        _slots.Clear();

        // Spawn one slot per key
        foreach (var key in sequence)
        {
            var go = Instantiate(keyPromptPrefab, keyPromptsParent);
            var slot = go.GetComponent<QTEKeySlot>();
            slot.SetKey(key);
            _slots.Add(slot);
        }
    }

    public void HideQTE()
    {
        qtePanel.SetActive(false);
    }

    public void HighlightKey(int index)
    {
        for (int i = 0; i < _slots.Count; i++)
            _slots[i].SetActive(i == index);
    }

    public void UpdateTimerBar(int index, float normalizedValue)
    {
        _slots[index].SetTimer(normalizedValue);
    }

    public void KeySuccess(int index) => _slots[index].ShowSuccess();
    public void KeyFail(int index)    => _slots[index].ShowFail();

    public void ShowQTEResult(QTEResult result)
    {
        resultLabel.gameObject.SetActive(true);
        switch (result)
        {
            case QTEResult.Perfect: resultLabel.text = "Perfect!"; resultLabel.color = Color.green;  break;
            case QTEResult.Good:    resultLabel.text = "Good!";    resultLabel.color = Color.yellow; break;
            case QTEResult.Failed:  resultLabel.text = "Failed!";  resultLabel.color = Color.red;    break;
        }
    }
}