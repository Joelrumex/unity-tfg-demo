using System.Collections;
using UnityEngine;

public class MercySystem : MonoBehaviour
{
    public System.Action<string[]> OnActUsed;
    public System.Action<string[]> OnActCompleted;   // ← new event for completion dialogue
    public System.Action<float> OnMercyBarUpdated;
    public System.Action OnMercyUnlocked;
    public System.Action OnMercyGranted;

    public void UseActOption(ActOption act, Unit enemy)
    {
        act.useCount++;

        bool justHitThreshold = act.useCount == act.requiredUses;

        // Show regular dialogue first
        OnActUsed?.Invoke(act.dialogueLines);

        // If threshold just hit, also fire completion dialogue
        if (justHitThreshold && act.completionDialogueLines != null
                              && act.completionDialogueLines.Length > 0)
            OnActCompleted?.Invoke(act.completionDialogueLines);

        // Apply mercy gain only on threshold hit
        if (justHitThreshold)
        {
            enemy.mercyBar = Mathf.Min(100f, enemy.mercyBar + act.mercyGain);
            OnMercyBarUpdated?.Invoke(enemy.mercyBar);

            if (enemy.mercyBar >= 100f && !enemy.mercyAvailable)
            {
                enemy.mercyAvailable = true;
                OnMercyUnlocked?.Invoke();
            }
        }
    }

    public void GrantMercy() => OnMercyGranted?.Invoke();
}