using UnityEngine;

using TMPro;

public class DragObjects : MonoBehaviour
{
    public string targetTag = "DropTarget"; // Tag for the correct drop target
    public TMPro.TextMeshProUGUI feedbackText; // Assign in inspector

    private bool isDragging = false;
    private Vector3 offset;
    private Camera mainCamera;
    private Vector3 originalPosition;
    private GameObject currentTarget;

    void Start()
    {
        mainCamera = Camera.main;
        originalPosition = transform.position;
        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);
    }

    void OnMouseDown()
    {
        isDragging = true;
        offset = transform.position - GetMouseWorldPos();
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mousePos = GetMouseWorldPos();
            transform.position = mousePos + offset;

            // Raycast to check for target under mouse
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            GameObject newTarget = null;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag(targetTag))
                {
                    newTarget = hit.collider.gameObject;
                }
            }

            // No feedback on drag, only on drop
            currentTarget = newTarget;
        }
    }

    void OnMouseUp()
    {
        if (isDragging)
        {
            isDragging = false;

            // Raycast to check if released over target
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag(targetTag))
                {
                    // Snap to target position
                    transform.position = hit.collider.transform.position;
                    if (feedbackText != null)
                        feedbackText.gameObject.SetActive(true); // Show feedback and keep it visible
                    return;
                }
            }
            // Not over target: return to original position
            transform.position = originalPosition;
        }
    }

    Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Mathf.Abs(mainCamera.WorldToScreenPoint(transform.position).z);
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }
    // Add this method to your DragObjects class
public void SetOriginalPosition(Vector3 newPosition)
{
    originalPosition = newPosition;
}
}
