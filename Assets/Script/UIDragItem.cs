using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIDragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum ItemType { Medicine, Tool }
    
    public string targetTag = "Patient"; 
    public TextMeshProUGUI feedbackText;
    public Camera mainCamera;
    
    [Header("Item Settings")]
    public ItemType itemType = ItemType.Medicine;
    public string counterID = "Item1"; // Unique ID - must match PatientListManager
    public int totalRequiredDrops = 3; // For medicine only
    
    [Header("Counter UI (For Medicine)")]
    public TextMeshProUGUI myCounterText;

    private Vector3 originalPosition;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector3 offset;
    private int currentDropCount = 0;
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

        if (myCounterText == null && itemType == ItemType.Medicine)
        {
            myCounterText = GetComponentInChildren<TextMeshProUGUI>();
        }

        // Find the PatientListManager in the scene
        listManager = FindFirstObjectByType<PatientListManager>();
        
        if (itemType == ItemType.Medicine)
            UpdateCounterDisplay();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        offset = transform.position - (Vector3)eventData.position;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false; 
        transform.SetAsLastSibling(); 
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = (Vector3)eventData.position + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

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
        if (itemType == ItemType.Medicine)
        {
            currentDropCount++;
            UpdateCounterDisplay();
            
            // Notify list manager
            if (listManager != null)
                listManager.OnMedicineDropped(counterID);

            if (currentDropCount >= totalRequiredDrops)
            {
                Debug.Log(counterID + " completed all drops!");
                Destroy(gameObject);
            }
            else
            {
                rectTransform.anchoredPosition = originalPosition;
            }
        }
        else if (itemType == ItemType.Tool)
        {
            // Notify list manager
            if (listManager != null)
                listManager.OnToolDropped(counterID);
            
            // Tool is used once and destroyed
            Debug.Log(counterID + " tool used!");
            Destroy(gameObject);
        }
    }

    void UpdateCounterDisplay()
    {
        if (myCounterText != null && itemType == ItemType.Medicine)
        {
            myCounterText.text = currentDropCount + "/" + totalRequiredDrops;
        }
    }
}