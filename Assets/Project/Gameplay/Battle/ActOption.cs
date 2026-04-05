using UnityEngine;

[CreateAssetMenu(fileName = "NewActOption", menuName = "RPG/ActOption")]
public class ActOption : ScriptableObject
{
    public string actionName = "Talk";
    [TextArea] public string[] dialogueLines;   // Lines shown when used
    public float mercyGain = 25f;               // How much mercy bar fills (0-100)
    public int requiredUses = 1;                // Must be used this many times to grant mercy
    [HideInInspector] public int useCount = 0; // Tracked at runtime
}