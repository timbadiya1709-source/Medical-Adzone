using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Attach this to a persistent GameObject (bootstrap). Assign RootParent to the empty parent
/// that contains all game objects you want saved as children.
/// </summary>
public class SceneStateManagerRoot : MonoBehaviour
{
    public static SceneStateManagerRoot Instance { get; private set; }

    [Tooltip("The root GameObject whose children will be saved/restored.")]
    public GameObject RootParent;

    // In-memory snapshots keyed by scene name
    Dictionary<string, SceneSnapshot> snapshots = new Dictionary<string, SceneSnapshot>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Try to restore snapshot for the loaded scene
        RestoreScene(scene.name);
    }

    /// <summary>
    /// Save current active scene snapshot for RootParent and then load target scene.
    /// </summary>
    public void SaveAndLoad(string targetSceneName)
    {
        var current = SceneManager.GetActiveScene().name;
        SaveCurrentScene(current);
        SceneManager.LoadScene(targetSceneName);
    }

    /// <summary>
    /// Save snapshot of RootParent children for the given scene name.
    /// </summary>
    public void SaveCurrentScene(string sceneName)
    {
        if (RootParent == null)
        {
            Debug.LogWarning("[SceneStateManagerRoot] RootParent is not assigned. Nothing saved.");
            return;
        }

        var snapshot = new SceneSnapshot();
        foreach (Transform child in RootParent.transform)
        {
            // Save recursively all descendants
            SaveTransformRecursive(child, snapshot);
        }

        snapshots[sceneName] = snapshot;
        Debug.Log($"[SceneStateManagerRoot] Saved snapshot for scene '{sceneName}' with {snapshot.objects.Count} entries.");
    }

    void SaveTransformRecursive(Transform t, SceneSnapshot snapshot)
    {
        var gs = new GameObjectSnapshot
        {
            path = GetPathRelativeToRoot(t),
            active = t.gameObject.activeSelf,
            localPosition = t.localPosition,
            localRotation = t.localRotation.eulerAngles,
            localScale = t.localScale
        };
        snapshot.objects.Add(gs);

        // children
        for (int i = 0; i < t.childCount; i++)
            SaveTransformRecursive(t.GetChild(i), snapshot);
    }

    /// <summary>
    /// Restore snapshot for the given scene name (if exists).
    /// Matches objects by path relative to RootParent.
    /// </summary>
    public void RestoreScene(string sceneName)
    {
        if (RootParent == null)
        {
            Debug.LogWarning("[SceneStateManagerRoot] RootParent is not assigned. Nothing to restore.");
            return;
        }

        if (!snapshots.TryGetValue(sceneName, out var snapshot))
        {
            Debug.Log($"[SceneStateManagerRoot] No snapshot found for scene '{sceneName}'.");
            return;
        }

        int restored = 0;
        foreach (var gs in snapshot.objects)
        {
            var target = FindByPath(gs.path);
            if (target == null)
            {
                Debug.LogWarning($"[SceneStateManagerRoot] Could not find object at path '{gs.path}' to restore.");
                continue;
            }

            var go = target.gameObject;
            go.SetActive(gs.active);
            target.localPosition = gs.localPosition;
            target.localRotation = Quaternion.Euler(gs.localRotation);
            target.localScale = gs.localScale;
            restored++;
        }

        Debug.Log($"[SceneStateManagerRoot] Restored {restored}/{snapshot.objects.Count} objects for scene '{sceneName}'.");
    }

    /// <summary>
    /// Build a path for a transform relative to RootParent, e.g. "ChildA/GrandChildB".
    /// </summary>
    string GetPathRelativeToRoot(Transform t)
    {
        if (RootParent == null) return t.name;
        var root = RootParent.transform;
        var parts = new List<string>();
        var cur = t;
        while (cur != null && cur != root)
        {
            parts.Add(cur.name);
            cur = cur.parent;
        }
        parts.Reverse();
        return string.Join("/", parts);
    }

    /// <summary>
    /// Find a transform by path relative to RootParent.
    /// </summary>
    Transform FindByPath(string path)
    {
        if (RootParent == null) return null;
        if (string.IsNullOrEmpty(path)) return null;
        var parts = path.Split('/');
        Transform cur = RootParent.transform;
        foreach (var p in parts)
        {
            var child = cur.Find(p);
            if (child == null) return null;
            cur = child;
        }
        return cur;
    }

    /// <summary>
    /// Optional: clear saved snapshot for a scene.
    /// </summary>
    public void ClearSnapshot(string sceneName)
    {
        snapshots.Remove(sceneName);
    }

    /// <summary>
    /// Optional: clear all snapshots.
    /// </summary>
    public void ClearAllSnapshots()
    {
        snapshots.Clear();
    }
}


/// <summary>
/// Snapshot of a whole scene's root children.
/// </summary>
[Serializable]
public class SceneSnapshot
{
    public List<GameObjectSnapshot> objects = new List<GameObjectSnapshot>();
}

/// <summary>
/// Snapshot for a single GameObject (transform + active).
/// Path is relative to the RootParent (no leading slash).
/// </summary>
[Serializable]
public class GameObjectSnapshot
{
    public string path;
    public bool active;
    public Vector3 localPosition;
    public Vector3 localRotation; // Euler angles
    public Vector3 localScale;
}
