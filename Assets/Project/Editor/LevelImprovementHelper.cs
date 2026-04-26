using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Helper script to add improved level elements to ForestPlatformerScene_Improved
/// Run via: Window > Level Improvements > Add Enemies and Obstacles
/// </summary>
public class LevelImprovementHelper
{
    private const string SKELETON_PREFAB_PATH = "Assets/BluBlu Games/2D Animated Skeletons/Prefabs/skeleton.prefab";
    private const string SPIKE_PREFAB_PATH = "Assets/Pixel Adventure 1/Assets/Traps/Spikes";
    
    [MenuItem("Window/Level Improvements/Add Enemies and Obstacles")]
    public static void AddLevelElements()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (!scene.name.Contains("ForestPlatformerScene_Improved"))
        {
            EditorUtility.DisplayDialog("Warning", 
                "Please open ForestPlatformerScene_Improved.unity first", "OK");
            return;
        }

        // New skeleton enemy positions
        Vector3[] skeletonPositions = new Vector3[]
        {
            new Vector3(100, 15, 0),  // Mid-level challenge
            new Vector3(150, 25, 0),  // High platform challenge
            new Vector3(200, 8, 0)    // Low platform challenge
        };

        // New spike positions
        Vector3[] spikePositions = new Vector3[]
        {
            new Vector3(80, 14, 0),   // Between platforms
            new Vector3(110, 22, 0),  // High platform valley
            new Vector3(130, 18, 0),  // Difficult jump area
            new Vector3(170, 26, 0),  // Upper level hazard
            new Vector3(220, 15, 0),  // Mid-level obstacle
            new Vector3(250, 10, 0)   // Lower platform hazard
        };

        // Load skeleton prefab
        GameObject skeletonPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(SKELETON_PREFAB_PATH);
        if (skeletonPrefab == null)
        {
            EditorUtility.DisplayDialog("Error", "Could not find skeleton prefab at: " + SKELETON_PREFAB_PATH, "OK");
            return;
        }

        // Add skeleton enemies
        Debug.Log("Adding skeleton enemies...");
        for (int i = 0; i < skeletonPositions.Length; i++)
        {
            GameObject skeleton = PrefabUtility.InstantiatePrefab(skeletonPrefab) as GameObject;
            skeleton.name = $"skeleton_improved_{i + 4}";  // Start numbering from 4 (after existing 3)
            skeleton.transform.position = skeletonPositions[i];
            skeleton.transform.localScale = new Vector3(0.52675635f, 0.52675635f, 0.52675635f);
            
            Debug.Log($"Added skeleton at position {skeletonPositions[i]}");
        }

        // Try to find spike triangle prefab
        GameObject trianglePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Pixel Adventure 1/Assets/Traps/Spikes/spike_01_up.prefab");
        if (trianglePrefab == null)
        {
            EditorUtility.DisplayDialog("Info", 
                "Spike prefabs not auto-found. You can manually add Triangle instances from the scene.\n\n" +
                "Positions to place spikes:\n" +
                "(80, 14) - Between platforms\n" +
                "(110, 22) - High platform valley\n" +
                "(130, 18) - Difficult jump area\n" +
                "(170, 26) - Upper level hazard\n" +
                "(220, 15) - Mid-level obstacle\n" +
                "(250, 10) - Lower platform hazard", 
                "OK");
        }
        else
        {
            // Add spike obstacles
            Debug.Log("Adding spike obstacles...");
            for (int i = 0; i < spikePositions.Length; i++)
            {
                GameObject spike = PrefabUtility.InstantiatePrefab(trianglePrefab) as GameObject;
                spike.name = $"spike_improved_{i + 1}";
                spike.transform.position = spikePositions[i];
                
                Debug.Log($"Added spike at position {spikePositions[i]}");
            }
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorUtility.DisplayDialog("Success", 
            $"Added {skeletonPositions.Length} skeleton enemies and {spikePositions.Length} spike obstacles!\n\n" +
            "The level now has 6 total skeletons and 22 total spikes for increased difficulty.", 
            "OK");
    }

    [MenuItem("Window/Level Improvements/Show Improvement Details")]
    public static void ShowImprovementDetails()
    {
        string details = @"=== FORESTPLATFORMERSCENE IMPROVEMENTS ===

CREATED: ForestPlatformerScene_Improved.unity

NEW ENEMIES (3 added):
1. Skeleton at (100, 15) - Mid-level challenge
2. Skeleton at (150, 25) - High platform challenge
3. Skeleton at (200, 8) - Low platform challenge

NEW OBSTACLES (6 added):
1. Spike at (80, 14) - Between platforms
2. Spike at (110, 22) - High platform valley
3. Spike at (130, 18) - Difficult jump area
4. Spike at (170, 26) - Upper level hazard
5. Spike at (220, 15) - Mid-level obstacle
6. Spike at (250, 10) - Lower platform hazard

TOTAL: 6 enemies (was 3), 22 spikes (was 16)

See LEVEL_IMPROVEMENTS.md for full documentation.";

        EditorUtility.DisplayDialog("Level Improvement Details", details, "OK");
    }
}
// Trigger recompilation
