using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ProductManager : MonoBehaviour
{
    private static ProductManager _instance;

    public static ProductManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Object.FindAnyObjectByType<ProductManager>();
                if (_instance == null)
                {
                    Debug.Log("[ProductManager] No instance found — auto-creating.");
                    GameObject go = new GameObject("ProductManager");
                    _instance = go.AddComponent<ProductManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    [Header("UI References")]
    public Text productCountText; 
    [Tooltip("If left empty, Manager will try to find 'CollectButton' automatically.")]
    public GameObject collectButton;

    private List<ProductCollector> activeProducts = new List<ProductCollector>();
    private int totalCollectedCount = 0;

    // Stock management
    private Dictionary<string, int> stockCounts = new Dictionary<string, int>();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            totalCollectedCount = 0;
            InitializeStock();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void InitializeStock()
    {
        // Initialize APIs with default values (not persisted)
        foreach (var api in ManufacturingConstants.AvailableAPIs)
        {
            stockCounts[api] = 3;
        }

        // Initialize Raw Materials with default values (not persisted)
        foreach (var material in ManufacturingConstants.RawMaterialTypes)
        {
            stockCounts[material] = 3;
        }

        Debug.Log("[ProductManager] Stock Initialized: " + string.Join(", ", stockCounts.Keys));
    }

    public int GetStock(string id)
    {
        if (stockCounts.ContainsKey(id)) return stockCounts[id];
        return 0;
    }

    public void UseStock(string id)
    {
        if (stockCounts.ContainsKey(id) && stockCounts[id] > 0)
        {
            stockCounts[id]--;
            // Trigger UI refresh
            RefreshAllStockDisplays();
        }
        else
        {
            Debug.LogWarning($"[ProductManager] Attempted to use stock for {id} but count is 0 or key missing!");
        }
    }

    [ContextMenu("Reset All Stock")]
    public void ResetAllStock()
    {
        Debug.Log("[ProductManager] Resetting all stock to 3.");
        foreach (var api in ManufacturingConstants.AvailableAPIs)
            stockCounts[api] = 3;
        foreach (var material in ManufacturingConstants.RawMaterialTypes)
            stockCounts[material] = 3;
        RefreshAllStockDisplays();
    }

    public void AddStock(string id, int amount)
    {
        if (!stockCounts.ContainsKey(id))
            stockCounts[id] = 0;

        stockCounts[id] += amount;
        
        Debug.Log($"[ProductManager] Added {amount} to {id}. New stock: {stockCounts[id]}");
        RefreshAllStockDisplays();
    }

    public bool IsAllStockEmpty()
    {
        if (stockCounts == null || stockCounts.Count == 0) 
        {
            Debug.LogWarning("[ProductManager] IsAllStockEmpty called but stockCounts is empty/null!");
            return false;
        }

        foreach (var kvp in stockCounts)
        {
            if (kvp.Value > 0) 
            {
                // Debug.Log($"[ProductManager] Found stock: {kvp.Key} = {kvp.Value}");
                return false;
            }
        }
        
        Debug.Log("[ProductManager] All stock confirmed empty.");
        return true;
    }

    public void RefreshAllStockDisplays()
    {
        // Find all stock display components in the scene (even inactive ones)
        StockDisplay[] displays = Resources.FindObjectsOfTypeAll<StockDisplay>();
        foreach (var display in displays)
        {
            if (display != null && display.gameObject.scene.isLoaded) 
                display.Refresh();
        }
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshUIReferences();
    }

    private void Start()
    {
        RefreshUIReferences();
    }

    public void RefreshUIReferences()
    {
        // Find count text if present in scene
        GameObject txtObj = GameObject.Find("ProductCountText");
        if (txtObj != null)
        {
            productCountText = txtObj.GetComponent<Text>();
        }

        // Robust way to find the button even if it's inactive
        if (collectButton == null)
        {
            string buttonName = "CollectButton";
            Button foundButton = FindObjectInScene<Button>(buttonName);
            Debug.Log($"[Manager] Searching for '{buttonName}'. Found: {(foundButton != null ? "Yes" : "No")}");

            if (foundButton != null)
            {
                collectButton = foundButton.gameObject;
            }
        }

        if (collectButton != null)
        {
            Button btn = collectButton.GetComponent<Button>();
            if (btn != null)
            {
                activeProducts.RemoveAll(p => p == null);
                bool shouldShow = activeProducts.Count > 0;
                collectButton.SetActive(shouldShow);
                Debug.Log($"[Manager] Button Visibility: {shouldShow} (Active Products: {activeProducts.Count})");
                
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(CollectAnyProduct);
                Debug.Log("[Manager] Listener re-attached to CollectButton.");
            }
            else
            {
                Debug.LogError("[Manager] collectButton GameObject found, but has no Button component!");
            }
        }

        UpdateProductCountUI();
    }

    private T FindObjectInScene<T>(string name) where T : Component
    {
        T[] objects = Resources.FindObjectsOfTypeAll<T>();
        foreach (T obj in objects)
        {
            if (obj.name == name && obj.gameObject.scene.isLoaded)
            {
                return obj;
            }
        }
        return null;
    }

    public void RegisterProduct(ProductCollector product)
    {
        if (!activeProducts.Contains(product))
        {
            activeProducts.Add(product);
        }
        
        Debug.Log($"[Manager] Registered product: {product.medicineID}. Total active: {activeProducts.Count}");

        if (collectButton != null)
        {
            collectButton.SetActive(true);
            Debug.Log("[Manager] Collect Button forced to ACTIVE.");
        }
        else
        {
            Debug.LogWarning("[Manager] Cannot show button: collectButton is NULL! Attempting refresh...");
            RefreshUIReferences();
        }
    }

    public void UnregisterProduct(ProductCollector product)
    {
        activeProducts.Remove(product);
        
        if (collectButton != null)
        {
            collectButton.SetActive(activeProducts.Count > 0);
        }
    }

    private void CollectAnyProduct()
    {
        Debug.Log($"[Manager] CollectAnyProduct called. Active products: {activeProducts.Count}");
        if (activeProducts.Count > 0)
        {
            Debug.Log($"[Manager] Delegating to product: {activeProducts[0].name}");
            activeProducts[0].CollectProduct();
        }
        else
        {
            Debug.LogWarning("[Manager] CollectAnyProduct called but no active products found!");
        }
    }

    public void ProductCollected()
    {
        totalCollectedCount++;
        UpdateProductCountUI();
    }

    private void UpdateProductCountUI()
    {
        if (productCountText != null)
        {
            productCountText.text = "Products: " + totalCollectedCount;
        }
    }
}