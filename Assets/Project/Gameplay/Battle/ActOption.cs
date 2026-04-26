using UnityEngine;

public enum MercyPhase { Phase1, Phase2, Phase3 }

[CreateAssetMenu(fileName = "NewActOption", menuName = "RPG/ActOption")]
public class ActOption : ScriptableObject
{
    public string actionName = "Talk";
    public MercyPhase phase  = MercyPhase.Phase1;  // ← which phase this action belongs to

    [TextArea] public string[] dialogueLines;
    [TextArea] public string[] completionDialogueLines;

    public float mercyGain  = 34f;     // Each phase grants ~1/3 of the bar
    public int requiredUses = 1;
    [HideInInspector] public int useCount = 0;
}