using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIMedicineDragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string targetTag = "Patient"; 
    public TextMeshProUGUI feedbackText;
    public Camera mainCamera;
    
    [Header("Medicine Settings")]
    public string counterID = "Medicine1"; // Unique ID - must match PatientListManager
    public int totalRequiredDrops = 3; // This is now your medicine stock/inventory
    
    [Header("Counter UI")]
    public TextMeshProUGUI myCounterText;

    private Vector3 originalPosition;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector3 offset;
    private PatientListManager listManager;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        if (mainCamera == null) mainCamera = Camera.main;
    }

    void Start()
    {
        originalPosition = rectTransform.anchoredPosition;
        
        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        if (myCounterText == null)
        {
            myCounterText = GetComponentInChildren<TextMeshProUGUI>();
        }

        // Find the PatientListManager in the scene
        listManager = FindFirstObjectByType<PatientListManager>();
        
        UpdateCounterDisplay();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Check if we have medicine left
        if (totalRequiredDrops <= 0)
        {
            Debug.Log("No medicine left to drag!");
            return;
        }
        
        offset = transform.position - (Vector3)eventData.position;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false; 
        transform.SetAsLastSibling(); 
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (totalRequiredDrops <= 0) return;
        
        transform.position = (Vector3)eventData.position + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (totalRequiredDrops <= 0)
        {
            rectTransform.anchoredPosition = originalPosition;
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(eventData.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag(targetTag))
            {
                if (feedbackText != null)
                    feedbackText.gameObject.SetActive(true);
                
                Debug.Log("Dropped " + counterID + " on: " + hit.collider.name);

                HandleSuccessfulDrop();
            }
            else
            {
                rectTransform.anchoredPosition = originalPosition;
            }
        }
        else
        {
            rectTransform.anchoredPosition = originalPosition;
        }
    }

    void HandleSuccessfulDrop()
    {
        // Decrease medicine count (using one medicine)
        totalRequiredDrops--;
        UpdateCounterDisplay();
        
        // Notify list manager
        if (listManager != null)
            listManager.OnMedicineDropped(counterID);

        // Return to original position
        rectTransform.anchoredPosition = originalPosition;
        
        // Optional: Destroy if no medicine left
        // if (totalRequiredDrops <= 0)
        // {
        //     Debug.Log(counterID + " ran out of medicine!");
        //     Destroy(gameObject);
        // }
    }

    void UpdateCounterDisplay()
    {
        if (myCounterText != null)
        {
            myCounterText.text = totalRequiredDrops.ToString();
            // Or if you want to show it differently:
            // myCounterText.text = "Stock: " + totalRequiredDrops;
        }
    }

    // Call this when a product is collected from manufacturing
    public void AddMedicine(int amount = 1)
    {
        totalRequiredDrops += amount;
        UpdateCounterDisplay();
        Debug.Log(counterID + " medicine stock increased to: " + totalRequiredDrops);
    }
}