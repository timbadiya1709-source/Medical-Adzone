using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct TagPanel
{
    public string tagName;   // e.g. "Paracetamol"
    public GameObject panel; // assign the UI panel GameObject for that tag
}

public class UIController : MonoBehaviour
{
    [Tooltip("Map object tags to UI panels")]
    public TagPanel[] tagPanels;

    // runtime lookup
    Dictionary<string, GameObject> map;

    void Awake()
    {
        map = new Dictionary<string, GameObject>(StringComparer.OrdinalIgnoreCase);
        foreach (var tp in tagPanels)
        {
            if (string.IsNullOrEmpty(tp.tagName) || tp.panel == null) continue;
            map[tp.tagName] = tp.panel;
            tp.panel.SetActive(false);
        }
    }

    // Show panel for a tag, hide previous
    public void ShowForTag(string tag)
    {
        HideAll();
        if (string.IsNullOrEmpty(tag)) return;
        if (map.TryGetValue(tag, out GameObject panel))
        {
            panel.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"UIController: no panel mapped for tag '{tag}'");
        }
    }

    public void HideAll()
    {
        foreach (var kv in map)
            if (kv.Value != null)
                kv.Value.SetActive(false);
    }
}
