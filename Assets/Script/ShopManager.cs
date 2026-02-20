using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("Settings")]
    public int defaultBuyAmount = 3;

    [Header("Panel References")]
    [Tooltip("Assign 'ShopPannel' here — this is the GameObject that will be shown/hidden")]
    public GameObject shopPanel;

    private GameObject returnPanel;
    private MachineUIPanel returnUIPanel;

    private void Awake()
    {
        Instance = this;
    }

    public void SetReturnPanel(GameObject panel)
    {
        returnPanel = panel;
    }

    public void SetReturnUIPanel(MachineUIPanel panel)
    {
        returnUIPanel = panel;
    }

    public void BuyItem(string itemID, int amount)
    {
        if (ProductManager.Instance == null) return;
        Debug.Log($"[Shop] Buying {amount}x {itemID}...");
        ProductManager.Instance.AddStock(itemID, amount);
    }

    /// <summary>
    /// Called by the single Buy button in the shop.
    /// Reads each ShopItem's selected quantity and purchases all of them, then closes.
    /// </summary>
    public void BuyAll()
    {
        if (shopPanel == null)
        {
            Debug.LogError("[Shop] shopPanel not assigned!");
            return;
        }

        ShopItem[] items = shopPanel.GetComponentsInChildren<ShopItem>(true);
        if (items.Length == 0)
        {
            Debug.LogWarning("[Shop] No ShopItem components found in shopPanel!");
            return;
        }

        foreach (ShopItem item in items)
        {
            int qty = item.GetCurrentQuantity();
            if (qty > 0)
            {
                Debug.Log($"[Shop] BuyAll: {qty}x {item.itemID}");
                BuyItem(item.itemID, qty);
                item.ResetQuantity(); // reset selector back to 1 for next time
            }
        }

        CloseShop();
    }

    // Helper methods for specific buttons in Unity Inspector (Legacy / Quick Buy)
    public void BuyBinder() => BuyItem(ManufacturingConstants.BINDER, defaultBuyAmount);
    public void BuyLubricant() => BuyItem(ManufacturingConstants.LUBRICANT, defaultBuyAmount);
    public void BuyExcipient() => BuyItem(ManufacturingConstants.EXCIPIENT, defaultBuyAmount);
    
    public void BuyParacetamol() => BuyItem("Paracetamol", defaultBuyAmount);

    public void CloseShop()
    {
        Debug.Log("[Shop] Closing shop panel.");

        // Hide the shop panel itself
        if (shopPanel != null)
            shopPanel.SetActive(false);
        else
            gameObject.SetActive(false);
        
        // Restore all machine UI panels via MachineUIPanel.ShowFromShop()
        if (returnUIPanel != null)
        {
            returnUIPanel.ShowFromShop();
        }
        else if (returnPanel != null)
        {
            returnPanel.SetActive(true); // fallback
        }
    }

    // Keep legacy version for compatibility if needed, but redirect to new one
    public void CloseShop(GameObject machineUIPanelContainer)
    {
        if (machineUIPanelContainer != null) returnPanel = machineUIPanelContainer;
        CloseShop();
    }
}
