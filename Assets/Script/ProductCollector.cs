using UnityEngine;

public class ProductCollector : MonoBehaviour
{
    [Header("Medicine to Add")]
    public string medicineCounterID = "Medicine1"; // Which medicine to add
    public int increaseAmount = 1; // How many medicines to add
    
    private void Start()
    {
        // Register this product with the ProductManager when it spawns
        ProductManager.Instance.RegisterProduct(this);
    }

    public void CollectProduct()
    {
        // Add medicine to inventory
        AddMedicineToInventory();
        
        // Notify the manager that this product is collected
        ProductManager.Instance.ProductCollected();
        
        // Destroy this product
        Destroy(gameObject);
    }

    private void AddMedicineToInventory()
    {
        // Find the medicine item by counterID
        UIMedicineDragItem[] allMedicines = FindObjectsByType<UIMedicineDragItem>(FindObjectsSortMode.None);
        
        foreach (UIMedicineDragItem medicine in allMedicines)
        {
            if (medicine.counterID == medicineCounterID)
            {
                medicine.AddMedicine(increaseAmount);
                Debug.Log("Added " + increaseAmount + " medicine(s) to " + medicineCounterID);
                break;
            }
        }
    }

    private void OnDestroy()
    {
        // Unregister when destroyed (if not collected)
        if (ProductManager.Instance != null)
        {
            ProductManager.Instance.UnregisterProduct(this);
        }
    }
}