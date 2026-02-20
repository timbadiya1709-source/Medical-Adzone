using UnityEngine;
using UnityEngine.UI;

public class ProductCollector : MonoBehaviour
{
    [Header("Collection Settings")]
    public string medicineID = "Medicine1";
    public int amountToAdd = 3;

    [Header("Machine Reference")]
    [HideInInspector] public ManufacturingMachine parentMachine;

    private void Start()
    {
        // CollectButton visibility is managed entirely by ProductManager.
        // ProductManager.RegisterProduct() shows the button when called.
        // ManufacturingMachine.CompleteProduction() calls RegisterProduct()
        // when the product spawns, and ManufacturingMachine.Start() calls it
        // when a waiting product is re-spawned after a scene reload.
        // This means the button only appears when a product actually exists
        // and has been properly registered — never on scene load by accident.
    }

public void CollectProduct()
{
    Debug.Log($"[Collector] '{gameObject.name}' collection started. MedicineID: {medicineID}");

    // 1. Unregister from ProductManager
    if (ProductManager.Instance != null)
    {
        ProductManager.Instance.UnregisterProduct(this);
        Debug.Log("[Collector] Unregistered from Manager.");
    }

    // 2. Write updated medicine count into persistent state
    if (SceneStateManagerRoot.Instance != null)
    {
        int current = SceneStateManagerRoot.Instance.GetMedicineState(medicineID, 0);
        SceneStateManagerRoot.Instance.SaveMedicineState(medicineID, current + amountToAdd);
        Debug.Log($"[Collector] {medicineID}: saved stock = {current + amountToAdd}");
    }
    else
    {
        Debug.LogWarning("[Collector] SceneStateManagerRoot not found! Stock NOT saved.");
    }

    // 3. Notify machine
    if (parentMachine != null)
    {
        parentMachine.OnProductCollected();
        Debug.Log("[Collector] Notified parent machine.");
    }

    // 4. Notify ProductManager
    if (ProductManager.Instance != null)
    {
        ProductManager.Instance.ProductCollected();
        Debug.Log("[Collector] Notified ProductManager (counter updated).");
    }

    // 5. Destroy product
    Debug.Log("[Collector] Destroying GameObject.");
    Destroy(gameObject);
}
}