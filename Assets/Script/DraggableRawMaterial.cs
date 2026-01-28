using UnityEngine;

public class DraggableRawMaterial : MonoBehaviour
{
    public enum MaterialType
    {
        Type1,
        Type2,
        Type3,
        Type4
    }

    public MaterialType materialType;
    public float dragHeight = 2f; // Height above ground while dragging
    
    private Vector3 originalPosition;
    private bool isDragging = false;
    private Camera mainCamera;
    private Vector3 offset;
    private Renderer objectRenderer;
    private Color originalColor;
    private Collider objectCollider;

    private void Start()
    {
        mainCamera = Camera.main;
        
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found! Make sure your camera is tagged as 'MainCamera'");
        }
        
        objectRenderer = GetComponent<Renderer>();
        objectCollider = GetComponent<Collider>();
        
        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
        }
        
        // Make sure we have a collider for mouse detection
        if (objectCollider == null)
        {
            Debug.LogWarning($"No collider found on {gameObject.name}, adding BoxCollider");
            objectCollider = gameObject.AddComponent<BoxCollider>();
        }
        
        // Check if collider is a trigger (it shouldn't be for OnMouseDown to work)
        if (objectCollider.isTrigger)
        {
            Debug.LogWarning($"Collider on {gameObject.name} is set as Trigger! OnMouseDown may not work. Uncheck 'Is Trigger' in the Inspector.");
        }
        
        Debug.Log($"{gameObject.name} initialized successfully. Material Type: {materialType}");
    }

    private void OnMouseDown()
    {
        Debug.Log($"OnMouseDown triggered on {gameObject.name}");
        isDragging = true;
        originalPosition = transform.position;
        
        // Calculate offset between mouse position and object position
        Vector3 mousePos = GetMouseWorldPosition();
        offset = transform.position - mousePos;
        
        Debug.Log($"Starting drag from position: {originalPosition}");
        
        // Visual feedback - make semi-transparent
        if (objectRenderer != null)
        {
            Color newColor = originalColor;
            newColor.a = 0.6f;
            objectRenderer.material.color = newColor;
        }
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mousePos = GetMouseWorldPosition();
            transform.position = mousePos + offset;
            
            // Keep at drag height
            transform.position = new Vector3(transform.position.x, dragHeight, transform.position.z);
        }
    }

    private void OnMouseUp()
    {
        if (isDragging)
        {
            Debug.Log($"OnMouseUp triggered on {gameObject.name} at position: {transform.position}");
            isDragging = false;
            
            // Restore original color
            if (objectRenderer != null)
            {
                objectRenderer.material.color = originalColor;
            }
            
            // Check if dropped on a machine
            bool droppedOnMachine = CheckDropOnMachine();
            
            // If not dropped on machine, return to original position
            if (!droppedOnMachine)
            {
                Debug.Log("Not dropped on machine, returning to original position");
                transform.position = originalPosition;
            }
        }
    }

    private bool CheckDropOnMachine()
    {
        Debug.Log($"Checking if dropped on machine from position: {transform.position}");
        
        // Raycast downward to check if we're over a machine
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100f))
        {
            Debug.Log($"Raycast hit: {hit.collider.gameObject.name}");
            ManufacturingMachine machine = hit.collider.GetComponent<ManufacturingMachine>();
            if (machine != null)
            {
                Debug.Log($"Found machine via raycast! Adding material {materialType}");
                machine.AddMaterial(materialType);
                Destroy(gameObject); // Destroy the material after dropping
                return true;
            }
        }
        else
        {
            Debug.Log("Raycast didn't hit anything");
        }
        
        // Also check using overlap sphere for more reliable detection
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2f);
        Debug.Log($"OverlapSphere found {colliders.Length} colliders");
        
        foreach (Collider col in colliders)
        {
            Debug.Log($"Checking collider: {col.gameObject.name}");
            ManufacturingMachine machine = col.GetComponent<ManufacturingMachine>();
            if (machine != null)
            {
                Debug.Log($"Found machine via overlap! Adding material {materialType}");
                machine.AddMaterial(materialType);
                Destroy(gameObject); // Destroy the material after dropping
                return true;
            }
        }
        
        Debug.Log("No machine found");
        return false;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mainCamera.WorldToScreenPoint(transform.position).z;
        return mainCamera.ScreenToWorldPoint(mousePos);
    }
}