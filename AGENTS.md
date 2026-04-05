# AGENTS.md

## Project Overview

This is a Unity 2D RPG battle system project (similar to UNDERTALE's combat). The project contains:
- Turn-based battle mechanics with Attack/Act/Mercy options
- QTE (Quick Time Event) system for damage mitigation
- Mercy system for pacifist endings
- Unit/Ability/ActOption data structures as ScriptableObjects

**Unity Version**: 2022.3+ (Universal Render Pipeline)
**Language**: C#

## Build & Run Commands

### Opening the Project
- Open the project in **Unity Editor** (double-click the `.unity` file or open via Unity Hub)
- The project uses the solution file `tfg-demo.slnx`

### Building
- **Build Player**: Unity Editor > File > Build Settings > Build (select platform)
- **Development Build**: Enable "Development Build" in Build Settings for debug symbols

### Running in Editor
- Press **Play** button in Unity Editor to run the game
- Use **Pause** to debug frame-by-frame

### Recompiling Scripts
- Scripts automatically recompile when saved in Unity
- Manual refresh: Unity Editor > Assets > Refresh (or Ctrl+R)

## Code Style Guidelines

### Naming Conventions
- **Classes/Enums/Methods/Properties**: `PascalCase` (e.g., `BattleManager`, `CalculateDamage`)
- **Private fields**: `_camelCase` with underscore prefix (e.g., `_player`, `_combatSystem`)
- **Public fields**: `PascalCase` for Unity serialized fields (e.g., `public Unit player`)
- **Constants**: `PascalCase` (e.g., `MaxHealth`)
- **ScriptableObjects**: Use `PascalCase` with descriptive names (e.g., `FireballAbility`)

### File Organization
- One class per file, filename matches class name
- Group related classes in folders: `Assets/Project/Gameplay/Battle/`, `Assets/Project/UI/`
- Use `[Header("Section Name")]` attributes to organize Inspector fields

### Imports
```csharp
using System.Collections;          // For coroutines
using System.Collections.Generic;  // For List<T>, Dictionary<T>
using UnityEngine;                 // Always required
using TMPro;                       // For TextMeshPro UI
using UnityEngine.UI;              // For Unity UI (Slider, Button)
```

### Types & Variables
- Use explicit types rather than `var` for clarity (except in LINQ or obvious constructors)
- Use `int` for HP, damage, stats (not `float`)
- Use `float` for timers, normalized values (0-1 range), mercy bar
- Use `bool` for flags (e.g., `isPlayerUnit`, `mercyAvailable`)
- Use `List<T>` for dynamic collections (abilities, actOptions)
- Use `System.Action` for events (no custom delegates needed)

### Unity-Specific Patterns
```csharp
// MonoBehaviour template
public class MyClass : MonoBehaviour
{
    [Header("References")]
    public Unit player;
    public CombatSystem combatSystem;

    [Header("Settings")]
    public int keyCount = 4;
    public float timePerKey = 1.2f;

    private MyClass _other;  // Private reference

    void Awake() { _other = GetComponent<MyClass>(); }
    void Start() { /* initialization */ }
}

// ScriptableObject for data assets
[CreateAssetMenu(fileName = "NewAbility", menuName = "RPG/Ability")]
public class Ability : ScriptableObject
{
    public string abilityName = "Attack";
    public int baseDamage = 0;
    [TextArea] public string description;
}
```

### Formatting Rules
- **Curly braces**: K&R style (opening brace on same line)
- **Indentation**: 4 spaces (Unity default)
- **Line length**: Under 120 characters when practical
- **Regions**: Use `// ── Section ─────────────────────────────` comments to divide logical sections
- **Spacing**: One space after comma, one space around operators
- **Null checks**: Use `?.` and `??` for safe navigation

### Error Handling
- Use `Debug.Log()` for runtime information (not `Console.WriteLine`)
- Use `Debug.LogWarning()` for non-critical issues
- Use `Debug.LogError()` for critical issues
- Throw `ArgumentException` with descriptive message for invalid parameters
- Always check `state != BattleState.PLAYER_TURN` before allowing actions

### Event Patterns
```csharp
// Event declaration
public System.Action<QTEResult> OnQTEComplete;

// Event invocation (with null check)
OnQTEComplete?.Invoke(result);

// Event subscription
qteManager.OnQTEComplete += OnQTEComplete;

// Event unsubscription
qteManager.OnQTEComplete -= OnQTEComplete;
```

### Coroutines for Timing
```csharp
IEnumerator MyRoutine()
{
    yield return new WaitForSeconds(1f);
    // Continue...
}

// Start coroutine
StartCoroutine(MyRoutine());

// Or with reference to stop later
StartCoroutine(RunSequence());
```

### Common Patterns Used in This Project
- **Battle state machine**: Use `enum BattleState { START, PLAYER_TURN, ENEMY_TURN, WIN, LOSE }`
- **Turn-based flow**: Setup → Player Turn → Enemy Turn → Repeat → End
- **Damage calculation**: `Mathf.Max(1, rawDamage - defense)`
- **QTE sequence**: Random key selection from pool (W/A/S/D), timed input, result calculation
- **Mercy system**: Track `useCount`, increment on use, unlock when `mercyBar >= 100`

## Important Notes

- **No unit tests** exist in this project
- **No linting/formatting tools** configured - follow the conventions above manually
- **ScriptableObjects** live in `Assets/` and are created via right-click menu
- **Scene files** (`.unity`) contain the game scene - edit via Unity Editor
- **Prefabs** (`.prefab`) are reusable GameObject templates
- **Meta files** (`.meta`) are auto-generated by Unity - do not edit manually

## Working with Unity-MCP (AI Tools)

This project has Unity-MCP configured for AI-assisted development. Available tools:
- Scene management (create, open, save scenes)
- GameObject manipulation (create, modify, find, destroy)
- Asset management (create materials, prefabs, scripts)
- Animation/Animator control
- Package management
- Unity Editor state control (play, pause, stop)

Use `skill` tool to load specialized workflows when needed.