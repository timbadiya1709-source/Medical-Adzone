using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIToolDragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string targetTag = "Patient"; 
    public TextMeshProUGUI feedbackText;
    public Camera mainCamera;
    
    [Header("Tool Settings")]
    public string counterID = "Tool1"; // Unique ID - must match PatientListManager

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

        // Find the PatientListManager in the scene
        listManager = FindFirstObjectByType<PatientListManager>();
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
        // Notify list manager
        if (listManager != null)
            listManager.OnToolDropped(counterID);
        
        // Tool is used once and destroyed
        Debug.Log(counterID + " tool used!");
        Destroy(gameObject);
    }
}