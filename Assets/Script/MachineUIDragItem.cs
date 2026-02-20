using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MachineUIDragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum ItemType { RawMaterial, API }
    public ItemType itemType;
    public string itemID; // e.g. "Type1" or "Paracetamol"
    public bool isLocked = false; // true = currently placed in a slot

    private Vector3 startPosition;
    private Transform startParent;
    private CanvasGroup canvasGroup;
    private MachineUIInputSlot currentSlot; // slot this item is occupying

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void Start()
    {
        // Cache home position once, at spawn
        startPosition = transform.position;
        startParent = transform.parent;
        RefreshVisibility();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Can't drag if already placed in a slot
        if (isLocked)
        {
            eventData.pointerDrag = null;
            return;
        }

        // Can't drag if no stock
        if (ProductManager.Instance != null && ProductManager.Instance.GetStock(itemID) <= 0)
        {
            Debug.LogWarning($"[UI] No stock left for {itemID}!");
            eventData.pointerDrag = null;
            return;
        }

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isLocked) return;
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isLocked) return;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1.0f;

        // Check if dropped over a slot
        GameObject over = eventData.pointerEnter;
        MachineUIInputSlot slot = over != null ? over.GetComponent<MachineUIInputSlot>() : null;

        if (slot != null && slot.CanAccept(this))
        {
            // Successful drop — consume stock, mark as in-use
            slot.AcceptItem(this);
            currentSlot = slot;
            isLocked = true;

            if (ProductManager.Instance != null)
                ProductManager.Instance.UseStock(itemID);

            // Return image to home position (stays visible but dimmed = "in use")
            transform.position = startPosition;
            canvasGroup.alpha = 0.4f; // dimmed to show it's placed
        }
        else
        {
            // Failed drop — snap back
            transform.position = startPosition;
        }

        RefreshVisibility();
    }

    // Called by MachineUIInputSlot.ClearSlot() when the machine resets
    public void ReturnToSpawn()
    {
        isLocked = false;
        currentSlot = null;
        transform.position = startPosition;
        transform.SetParent(startParent);
        RefreshVisibility();
    }

    // Show/hide based on remaining stock
    public void RefreshVisibility()
    {
        int stock = ProductManager.Instance != null ? ProductManager.Instance.GetStock(itemID) : 0;

        if (stock <= 0)
        {
            // No stock at all — hide completely
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }
        else if (isLocked)
        {
            // In a slot — dimmed
            canvasGroup.alpha = 0.4f;
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            // Available to drag
            canvasGroup.alpha = 1.0f;
            canvasGroup.blocksRaycasts = true;
        }
    }
}
