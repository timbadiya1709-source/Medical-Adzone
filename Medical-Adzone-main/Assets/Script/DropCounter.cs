using UnityEngine;
using TMPro;

public class DropCounter : MonoBehaviour
{
    [Header("Counter Settings")]
    public int currentCount = 0;
    public int totalRequired = 3; // Change this to whatever you need
    
    [Header("UI References")]
    public TextMeshProUGUI counterText; // The text that shows "2/3"
    public GameObject counterPanel; // Optional: panel containing the counter
    
    [Header("Events (Optional)")]
    public bool hideWhenComplete = false;
    public UnityEngine.Events.UnityEvent onCounterComplete; // Trigger when goal is reached

    void Start()
    {
        UpdateCounterDisplay();
        
        // Optional: hide counter at start
        if (counterPanel != null && currentCount == 0)
        {
            // counterPanel.SetActive(false); // Uncomment if you want it hidden initially
        }
    }

    public void IncrementCount()
    {
        currentCount++;
        UpdateCounterDisplay();
        
        // Show counter panel if it was hidden
        if (counterPanel != null)
        {
            counterPanel.SetActive(true);
        }

        // Check if goal is reached
        if (currentCount >= totalRequired)
        {
            OnCounterComplete();
        }
    }

    void UpdateCounterDisplay()
    {
        if (counterText != null)
        {
            counterText.text = currentCount + "/" + totalRequired;
        }
    }

    void OnCounterComplete()
    {
        Debug.Log("Drop goal completed!");
        
        // Trigger any events you want (like showing a "Level Complete" message)
        onCounterComplete?.Invoke();
        
        // Optional: hide counter when complete
        if (hideWhenComplete && counterPanel != null)
        {
            counterPanel.SetActive(false);
        }
    }

    // Optional: Reset counter
    public void ResetCounter()
    {
        currentCount = 0;
        UpdateCounterDisplay();
    }
}