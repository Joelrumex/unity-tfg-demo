using UnityEngine;

[CreateAssetMenu(fileName = "NewObserveOption", menuName = "RPG/ObserveOption")]
public class ObserveOption : ScriptableObject
{
    [TextArea] public string[] phase1Hints;   // Shown when mercyBar < 33
    [TextArea] public string[] phase2Hints;   // Shown when mercyBar 33-66
    [TextArea] public string[] phase3Hints;   // Shown when mercyBar > 66
}