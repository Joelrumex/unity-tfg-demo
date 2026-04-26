using UnityEngine;

public class MercySystem : MonoBehaviour
{
    public System.Action<string[]> OnActUsed;
    public System.Action<string[]> OnActCompleted;
    public System.Action<float>    OnMercyBarUpdated;
    public System.Action<MercyPhase> OnPhaseChanged;   // ← fires when phase advances
    public System.Action           OnMercyUnlocked;
    public System.Action           OnMercyGranted;

    private MercyPhase _lastPhase = MercyPhase.Phase1;

    public void UseActOption(ActOption act, Unit enemy)
    {
        // Wrong phase — action has no effect, give a nudge
        if (act.phase != enemy.GetCurrentPhase())
        {
            OnActUsed?.Invoke(new[] { "...This doesn't seem to be working right now." });
            return;
        }

        act.useCount++;
        bool justHitThreshold = act.useCount == act.requiredUses;

        OnActUsed?.Invoke(act.dialogueLines);

        if (justHitThreshold)
        {
            if (act.completionDialogueLines != null && act.completionDialogueLines.Length > 0)
                OnActCompleted?.Invoke(act.completionDialogueLines);

            bool mercyJustUnlocked = enemy.ApplyMercyGain(act);
            OnMercyBarUpdated?.Invoke(enemy.mercyBar);

            // Check if phase changed
            MercyPhase newPhase = enemy.GetCurrentPhase();
            if (newPhase != _lastPhase)
            {
                _lastPhase = newPhase;
                OnPhaseChanged?.Invoke(newPhase);
            }

            if (mercyJustUnlocked)
                OnMercyUnlocked?.Invoke();
        }
    }

    public void Observe(Unit enemy)
    {
        string[] hints = enemy.GetObserveHints();
        OnActUsed?.Invoke(hints);
    }

    public void GrantMercy() => OnMercyGranted?.Invoke();

    public void ResetPhase() => _lastPhase = MercyPhase.Phase1;
}