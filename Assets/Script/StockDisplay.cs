using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StockDisplay : MonoBehaviour
{
    public string targetItemID; // e.g., "Binder", "Lubricant", "Paracetamol"
    
    private TextMeshProUGUI tmproText;
    private Text standardText;

    private void Awake()
    {
        CacheReferences();
    }

    private void Start()
    {
        if (ProductManager.Instance != null)
            Refresh();
        else
            StartCoroutine(RefreshWhenReady());
    }

    private void OnEnable()
    {
        if (ProductManager.Instance != null)
            Refresh();
        else
            StartCoroutine(RefreshWhenReady());
    }

    private IEnumerator RefreshWhenReady()
    {
        // Wait one frame, then keep waiting until ProductManager has initialized
        yield return null;
        while (ProductManager.Instance == null)
            yield return null;
        Refresh();
    }

    private void CacheReferences()
    {
        if (tmproText == null) tmproText = GetComponent<TextMeshProUGUI>();
        if (standardText == null) standardText = GetComponent<Text>();
    }

    public void Refresh()
    {
        CacheReferences();
        
        if (ProductManager.Instance == null)
        {
            // This is common during the very first scene's Awake/Start. 
            // The Start() and OnEnable() calls will eventually catch it.
            return;
        }

        int stock = ProductManager.Instance.GetStock(targetItemID);
        string displayStr = "x" + stock;

        if (tmproText != null)
        {
            tmproText.text = displayStr;
            Debug.Log($"[StockDisplay] {gameObject.name} (TMPro) updated: {targetItemID} = {displayStr}");
        }
        else if (standardText != null)
        {
            standardText.text = displayStr;
            Debug.Log($"[StockDisplay] {gameObject.name} (Standard Text) updated: {targetItemID} = {displayStr}");
        }
        else
        {
            Debug.LogError($"[StockDisplay] No suitable Text component found on {gameObject.name}! Please attach TextMeshPro or standard UI Text.");
        }
    }
}
