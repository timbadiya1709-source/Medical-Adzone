using UnityEngine;

public class MedicinePanelManager : MonoBehaviour
{
    public GameObject medicinePanel; // Assign your panel here
    public GameObject openButton; // The button to open panel
    
    void Start()
    {
        // Hide panel at start
        medicinePanel.SetActive(false);
    }
    
    void Update()
    {
        // Close panel when clicking outside of it
        if (medicinePanel.activeSelf && Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOverPanel())
            {
                ClosePanel();
            }
        }
    }
    
    public void OpenPanel()
    {
        medicinePanel.SetActive(true);
    }
    
    public void ClosePanel()
    {
        medicinePanel.SetActive(false);
    }
    
    private bool IsPointerOverPanel()
    {
        // Check if mouse is over the panel
        Vector2 localMousePosition = medicinePanel.transform.InverseTransformPoint(Input.mousePosition);
        RectTransform panelRect = medicinePanel.GetComponent<RectTransform>();
        
        return panelRect.rect.Contains(localMousePosition);
    }
}