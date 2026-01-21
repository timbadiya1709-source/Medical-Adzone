using UnityEngine;
using UnityEngine.UI;
using TMPro;

// This script goes on the TRIGGER GameObject (not the player)
public class PatientTrigger : MonoBehaviour
{
    [Header("Patient Information")]
    [SerializeField] private string patientTitle = "Patient Name";
    [TextArea(3, 10)]
    [SerializeField] private string patientDescription = "Patient description here...";
    
    [Header("UI References")]
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button closeButton;

    [Header("Second UI Panel References")]
    [SerializeField] private GameObject uiPanel2;
    [SerializeField] private TextMeshProUGUI titleText2;
    [SerializeField] private TextMeshProUGUI descriptionText2;
    [SerializeField] private Button closeButton2;

    [Header("Second Panel Information")]
    [SerializeField] private string panel2Title = "Panel 2 Title";
    [TextArea(3, 10)]
    [SerializeField] private string panel2Description = "Panel 2 description here...";
    
    [Header("Trigger Settings")]
    [SerializeField] private bool triggerOnce = true;
    
    private bool hasTriggered = false;

    private void Start()
    {
        // Make sure the trigger object has a collider
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError("No Collider found! Please add a collider component to " + gameObject.name);
        }
        else if (!col.isTrigger)
        {
            Debug.LogError("Collider is not set as Trigger! Please check 'Is Trigger' on " + gameObject.name);
        }
        
        // Make sure panel is hidden at start
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("UI Panel not assigned in Inspector!");
        }
        
        // Setup close button
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ClosePanel);
        }

        // Hide second panel at start
        if (uiPanel2 != null)
        {
            uiPanel2.SetActive(false);
        }
        else
        {
            Debug.LogError("Second UI Panel not assigned in Inspector!");
        }

        // Setup close button for second panel
        if (closeButton2 != null)
        {
            closeButton2.onClick.AddListener(ClosePanel2);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if player entered trigger (checking for Rigidbody since you don't have Player tag)
        if (other.GetComponent<Rigidbody>() != null)
        {
            Debug.Log("Player entered trigger zone!");
            
            if (triggerOnce && hasTriggered)
                return;
                
            ShowPatientPanel();
            hasTriggered = true;
        }
    }

    private void ShowPatientPanel()
    {
        if (uiPanel == null)
        {
            Debug.LogWarning("UI Panel not assigned!");
            return;
        }

        // Update UI with patient data
        if (titleText != null)
        {
            titleText.text = patientTitle;
        }
        else
        {
            Debug.LogWarning("Title Text not assigned!");
        }
        
        if (descriptionText != null)
        {
            descriptionText.text = patientDescription;
        }
        else
        {
            Debug.LogWarning("Description Text not assigned!");
        }

        // Show panel
        uiPanel.SetActive(true);
        Debug.Log("Panel shown!");

        // Update and show second panel
        if (titleText2 != null)
        {
            titleText2.text = panel2Title;
        }
        else
        {
            Debug.LogWarning("Second Title Text not assigned!");
        }

        if (descriptionText2 != null)
        {
            descriptionText2.text = panel2Description;
        }
        else
        {
            Debug.LogWarning("Second Description Text not assigned!");
        }

        if (uiPanel2 != null)
        {
            uiPanel2.SetActive(true);
            Debug.Log("Second panel shown!");
        }
        else
        {
            Debug.LogWarning("Second UI Panel not assigned!");
        }
        
        // Optional: Pause game or freeze player movement
        // Time.timeScale = 0f; // Uncomment to pause game
        // Or disable player movement script here
    }

    public void ClosePanel()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
            Debug.Log("Panel closed!");
        }
        
        // Optional: Resume game
        // Time.timeScale = 1f; // Uncomment if you paused the game
    }

    public void ClosePanel2()
    {
        if (uiPanel2 != null)
        {
            uiPanel2.SetActive(false);
            Debug.Log("Second panel closed!");
        }
    }
}