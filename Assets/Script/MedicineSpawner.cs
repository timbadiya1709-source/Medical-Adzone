using UnityEngine;
using UnityEngine.EventSystems;

public class MedicineSpawner : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject medicinePrefab; // The medicine prefab with DragObjects script
    
    private GameObject currentMedicine;
    private Camera mainCamera;
    private bool isDragging = false;
    
    void Start()
    {
        mainCamera = Camera.main;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Spawn medicine when drag starts
        Vector3 spawnPos = GetWorldPosition(eventData.position);
        currentMedicine = Instantiate(medicinePrefab, spawnPos, Quaternion.identity);
        
        // Disable the DragObjects script temporarily so we control the drag
        DragObjects dragScript = currentMedicine.GetComponent<DragObjects>();
        if (dragScript != null)
        {
            dragScript.enabled = false;
        }
        
        isDragging = true;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging && currentMedicine != null)
        {
            // Move medicine with mouse
            currentMedicine.transform.position = GetWorldPosition(eventData.position);
        }
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentMedicine != null)
        {
            // Enable DragObjects script so player can continue dragging
            DragObjects dragScript = currentMedicine.GetComponent<DragObjects>();
            if (dragScript != null)
            {
                dragScript.enabled = true;
                // Set the original position for the DragObjects script
                dragScript.SendMessage("SetOriginalPosition", currentMedicine.transform.position);
            }
        }
        
        isDragging = false;
        currentMedicine = null;
    }
    
    private Vector3 GetWorldPosition(Vector2 screenPosition)
    {
        Vector3 mousePoint = screenPosition;
        mousePoint.z = 10f; // Distance from camera (adjust as needed)
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }
}