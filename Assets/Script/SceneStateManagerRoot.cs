using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneStateManagerRoot : MonoBehaviour
{
    private static SceneStateManagerRoot _instance;
    private static bool _isQuitting = false;

    public static SceneStateManagerRoot Instance 
    { 
        get 
        {
            if (_isQuitting) return null;

            if (_instance == null)
            {
                _instance = Object.FindAnyObjectByType<SceneStateManagerRoot>();
                if (_instance == null)
                {
                    Debug.Log("[SceneStateManager] Manager missing! Creating a new one automatically.");
                    GameObject go = new GameObject("SceneStateManagerRoot");
                    _instance = go.AddComponent<SceneStateManagerRoot>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
        private set { _instance = value; }
    }

    private void OnApplicationQuit()
    {
        _isQuitting = true;
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Load persistent states immediately
            LoadAllMaterialStatesFromPrefs();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    // --- Medicine State Persistence ---

    private Dictionary<string, int> medicineStates = new Dictionary<string, int>();

    public void SaveMedicineState(string id, int count)
    {
        if (medicineStates.ContainsKey(id))
            medicineStates[id] = count;
        else
            medicineStates.Add(id, count);
    }

    public int GetMedicineState(string id, int defaultCount)
    {
        if (medicineStates.TryGetValue(id, out int count))
            return count;
        return defaultCount;
    }

    // --- Patient List Persistence ---
    private Dictionary<string, int> droppedMedicineCounts = new Dictionary<string, int>();
    private HashSet<string> completedTools = new HashSet<string>();

    public void SaveDroppedMedicineCount(string id, int count)
    {
        if (droppedMedicineCounts.ContainsKey(id))
            droppedMedicineCounts[id] = count;
        else
            droppedMedicineCounts.Add(id, count);
    }

    public int GetDroppedMedicineCount(string id)
    {
        if (droppedMedicineCounts.TryGetValue(id, out int count))
            return count;
        return 0;
    }

    public void SaveCompletedTool(string id, bool isCompleted)
    {
        if (isCompleted)
        {
            if (!completedTools.Contains(id))
                completedTools.Add(id);
        }
        else
        {
            if (completedTools.Contains(id))
                completedTools.Remove(id);
        }
    }

    public bool IsToolCompleted(string id)
    {
        return completedTools.Contains(id);
    }

    // --- GameSave Children State Persistence (Scene2) ---
    private Dictionary<string, bool> gameSaveChildStates = new Dictionary<string, bool>();

    public void SaveGameSaveChildState(string childName, bool isActive)
    {
        if (gameSaveChildStates.ContainsKey(childName))
            gameSaveChildStates[childName] = isActive;
        else
            gameSaveChildStates.Add(childName, isActive);
        
        SaveAllMaterialStatesToPrefs();
    }

    public bool GetGameSaveChildState(string childName, out bool isActive)
    {
        return gameSaveChildStates.TryGetValue(childName, out isActive);
    }

    private void SaveAllMaterialStatesToPrefs()
    {
        List<string> data = new List<string>();
        foreach (var entry in gameSaveChildStates)
        {
            data.Add($"{entry.Key}:{(entry.Value ? "1" : "0")}");
        }
        PlayerPrefs.SetString("GameSaveChildren", string.Join(";", data));
        PlayerPrefs.Save();
        Debug.Log("SceneStateManagerRoot: All material states saved to PlayerPrefs.");
    }

    private void LoadAllMaterialStatesFromPrefs()
    {
        string rawData = PlayerPrefs.GetString("GameSaveChildren", "");
        if (string.IsNullOrEmpty(rawData)) return;

        string[] pairs = rawData.Split(';');
        foreach (string pair in pairs)
        {
            string[] kv = pair.Split(':');
            if (kv.Length == 2)
            {
                gameSaveChildStates[kv[0]] = kv[1] == "1";
            }
        }
        Debug.Log($"SceneStateManagerRoot: Loaded {gameSaveChildStates.Count} material states from PlayerPrefs.");
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}