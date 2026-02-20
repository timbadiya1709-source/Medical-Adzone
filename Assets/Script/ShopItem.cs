using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    [Header("Item Settings")]
    public string itemID; // e.g., "Binder", "Paracetamol"
    public int baseQuantity = 1;
    
    [Header("UI References")]
    public TextMeshProUGUI quantityText;
    public Button plusButton;
    // NOTE: buyButton removed — use the single Buy button in ShopManager.BuyAll()

    private int currentQuantity;

    private void Start()
    {
        currentQuantity = baseQuantity;
        UpdateUI();
        if (plusButton != null) plusButton.onClick.AddListener(OnPlusClicked);
    }

    public int GetCurrentQuantity() => currentQuantity;

    public void ResetQuantity()
    {
        currentQuantity = baseQuantity;
        UpdateUI();
    }

    private void OnPlusClicked()
    {
        currentQuantity++;
        UpdateUI();
    }

    private void OnBuyClicked()
    {
        if (ShopManager.Instance != null)
        {
            Debug.Log($"[ShopItem] buying {currentQuantity} of {itemID}");
            ShopManager.Instance.BuyItem(itemID, currentQuantity);
            
            // Optional: Reset quantity after purchase?
            // currentQuantity = baseQuantity;
            // UpdateUI();
        }
        else
        {
            Debug.LogError("[ShopItem] ShopManager Instance not found!");
        }
    }

    private void UpdateUI()
    {
        if (quantityText != null)
            quantityText.text = currentQuantity.ToString();
    }
}
