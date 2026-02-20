using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class MachineUIPanel : MonoBehaviour
{
    [Header("Slots")]
    public List<MachineUIInputSlot> materialSlots; // Should be 3
    public MachineUIInputSlot apiSlot;

    [Header("Panels")]
    public GameObject mainContainer; // The parent of all 3 sub-panels
    public Button produceButton;
    public Button closeButton; // Optional close button

    [Header("Shop System")]
    public Button shopButton;
    public GameObject shopPanel;

    [Header("External Panels")]
    public List<GameObject> externalPanelsToToggle;

    private ManufacturingMachine currentMachine;

    void Start()
    {
        // Ensure Shop Panel is closed by default
        if (shopPanel != null) shopPanel.SetActive(false);

        Hide();
        if (produceButton != null)
            produceButton.onClick.AddListener(OnProduceClicked);
        else
            Debug.LogError("[UI] Produce Button not assigned in MachineUIPanel!");

        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);
        
        if (shopButton != null)
        {
            shopButton.onClick.AddListener(OnShopButtonClicked);
            shopButton.gameObject.SetActive(false);
        }
        
        if (materialSlots != null)
        {
            foreach(var slot in materialSlots) 
            {
                if (slot != null) slot.onContentChanged += RefreshProduceButton;
            }
        }
        
        if (apiSlot != null) 
            apiSlot.onContentChanged += RefreshProduceButton;
        else
            Debug.LogError("[UI] API Slot not assigned in MachineUIPanel!");
    }

    public void Show(ManufacturingMachine machine)
    {
        currentMachine = machine;
        ShowAllPanels();
    }

    // Called by ShopManager when returning from shop
    public void ShowFromShop()
    {
        ShowAllPanels();
    }

    private void ShowAllPanels()
    {
        mainContainer.SetActive(true);
        
        if (externalPanelsToToggle != null)
        {
            foreach(var p in externalPanelsToToggle) 
                if (p != null) p.SetActive(true);
        }

        RefreshProduceButton();
        if (ProductManager.Instance != null) ProductManager.Instance.RefreshAllStockDisplays();
        if (shopButton != null) shopButton.gameObject.SetActive(true);
    }


    public void Hide()
    {
        mainContainer.SetActive(false);
        
        if (produceButton != null)
            produceButton.gameObject.SetActive(false);

        if (externalPanelsToToggle != null)
        {
            foreach(var p in externalPanelsToToggle) 
                if (p != null) p.SetActive(false);
        }

        if (shopButton != null)
            shopButton.gameObject.SetActive(false);

        ClearAllSlots();
    }

    private void RefreshProduceButton()
    {
        // Check if all material slots are filled (assuming 3 for Paracetamol)
        bool allMaterialSlotsFilled = materialSlots.Count == 3 && materialSlots.All(s => s.currentItem != null);
        bool apiFilled = apiSlot.currentItem != null;
        
        // Recipe check for Paracetamol: Needs Binder, Lubricant, Excipient
        // This assumes the items are placed in any order in the material slots
        bool hasBinder = materialSlots.Any(s => s.currentItem != null && s.currentItem.itemID == ManufacturingConstants.BINDER);
        bool hasLubricant = materialSlots.Any(s => s.currentItem != null && s.currentItem.itemID == ManufacturingConstants.LUBRICANT);
        bool hasExcipient = materialSlots.Any(s => s.currentItem != null && s.currentItem.itemID == ManufacturingConstants.EXCIPIENT);

        // For Paracetamol, we need all three specific materials AND the API
        bool recipeComplete = hasBinder && hasLubricant && hasExcipient && apiFilled;

        Debug.Log($"[UI] Refresh Button: MaterialsReady={allMaterialSlotsFilled}, APIFilled={apiFilled}, RecipeComplete={recipeComplete}");

        bool isReady = recipeComplete;
        
        if (produceButton != null)
        {
            produceButton.interactable = isReady;
            produceButton.gameObject.SetActive(true); 
        }
    }

    private void OnProduceClicked()
    {
        Debug.Log("[UI] Produce Button Clicked!");
        if (currentMachine == null) 
        {
            Debug.LogError("[UI] No current machine assigned to panel!");
            return;
        }

        if (apiSlot.currentItem == null)
        {
            Debug.LogError("[UI] API slot is empty!");
            return;
        }

        string selectedAPI = apiSlot.currentItem.itemID;
        Debug.Log($"[UI] Starting production for: {selectedAPI}");
        
        // Start production in machine
        currentMachine.StartProductionFromUI(selectedAPI);
        
        ClearAllSlots();
        Hide(); // FIX: Hide panels after starting production
    }

    private void OnShopButtonClicked()
    {
        Debug.Log("[UI] Shop Button Clicked!");
        
        // Hide manufacturing panels
        mainContainer.SetActive(false);
        if (produceButton != null) produceButton.gameObject.SetActive(false);
        if (externalPanelsToToggle != null)
        {
            foreach(var p in externalPanelsToToggle) 
                if (p != null) p.SetActive(false);
        }

        // Show shop panel
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
            if (ShopManager.Instance != null)
            {
                ShopManager.Instance.SetReturnUIPanel(this); // pass ourselves so all panels restore
            }
        }
        else
        {
            Debug.LogError("[UI] Shop Panel not assigned in MachineUIPanel!");
        }

        if (shopButton != null) shopButton.gameObject.SetActive(false);
    }

    private void ClearAllSlots()
    {
        // Logic to return items to their source panels if needed
        foreach(var s in materialSlots) s.ClearSlot();
        apiSlot.ClearSlot();
    }
}
