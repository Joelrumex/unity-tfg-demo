using System.Collections;
using UnityEngine;

public class MercySystem : MonoBehaviour
{
    // Events BattleManager listens to
    public System.Action<string[]> OnActUsed;       // Passes dialogue lines to UI
    public System.Action<float> OnMercyBarUpdated;  // Passes new mercy value (0-100)
    public System.Action OnMercyUnlocked;           // Called when bar hits 100
    public System.Action OnMercyGranted;            // Called when player selects MERCY

    public void UseActOption(ActOption act, Unit enemy)
    {
        bool justUnlocked = enemy.ApplyMercyGain(act);

        // Fire dialogue event
        OnActUsed?.Invoke(act.dialogueLines);

        // Update mercy bar UI
        OnMercyBarUpdated?.Invoke(enemy.mercyBar);

        if (justUnlocked)
            OnMercyUnlocked?.Invoke();
    }

    public void GrantMercy()
    {
        OnMercyGranted?.Invoke();
    }
}