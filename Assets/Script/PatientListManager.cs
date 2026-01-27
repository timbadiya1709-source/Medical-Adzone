using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

// PATIENT LIST MANAGER - Attach to the Patient List Panel
public class PatientListManager : MonoBehaviour
{
    [System.Serializable]
    public class MedicineListItem
    {
        public string itemID; // Must match UIDragItem's counterID
        public string displayName = "Paracetamol";
        public int requiredCount = 3;
        public TextMeshProUGUI countText; // Text showing "0/3"
        
        [HideInInspector] public int currentCount = 0;
    }
    
    [System.Serializable]
    public class ToolListItem
    {
        public string itemID; // Must match UIDragItem's counterID
        public string displayName = "Scalpel";
        public Image backgroundImage; // Background to highlight
        public Color highlightColor = Color.yellow;
        
        [HideInInspector] public bool isCompleted = false;
        [HideInInspector] public Color originalColor;
    }
    
    [Header("List Items")]
    public List<MedicineListItem> medicines = new List<MedicineListItem>();
    public List<ToolListItem> tools = new List<ToolListItem>();
    
    [Header("Completion Notification")]
    public TextMeshProUGUI completionText;
    public string completionMessage = "All items completed!";
    public GameObject completionPanel; // Optional panel to show
    
    void Start()
    {
        // Store original colors for tools
        foreach (var tool in tools)
        {
            if (tool.backgroundImage != null)
                tool.originalColor = tool.backgroundImage.color;
        }
        
        // Initialize all displays
        UpdateAllDisplays();
        
        if (completionText != null)
            completionText.gameObject.SetActive(false);
        
        if (completionPanel != null)
            completionPanel.SetActive(false);
    }
    
    // Call this from UIDragItem when medicine is dropped
    public void OnMedicineDropped(string itemID)
    {
        MedicineListItem medicine = medicines.Find(m => m.itemID == itemID);
        
        if (medicine != null)
        {
            medicine.currentCount++;
            UpdateMedicineDisplay(medicine);
            
            Debug.Log($"{medicine.displayName}: {medicine.currentCount}/{medicine.requiredCount}");
            
            CheckCompletion();
        }
        else
        {
            Debug.LogWarning($"Medicine with ID '{itemID}' not found in list!");
        }
    }
    
    // Call this from UIDragItem when tool is dropped
    public void OnToolDropped(string itemID)
    {
        ToolListItem tool = tools.Find(t => t.itemID == itemID);
        
        if (tool != null && !tool.isCompleted)
        {
            tool.isCompleted = true;
            
            if (tool.backgroundImage != null)
            {
                tool.backgroundImage.color = tool.highlightColor;
            }
            
            Debug.Log($"{tool.displayName} highlighted!");
            
            CheckCompletion();
        }
        else if (tool == null)
        {
            Debug.LogWarning($"Tool with ID '{itemID}' not found in list!");
        }
    }
    
    void UpdateMedicineDisplay(MedicineListItem medicine)
    {
        if (medicine.countText != null)
        {
            medicine.countText.text = $"{medicine.displayName}  {medicine.currentCount}/{medicine.requiredCount}";
        }
    }
    
    void UpdateAllDisplays()
    {
        foreach (var medicine in medicines)
        {
            UpdateMedicineDisplay(medicine);
        }
    }
    
    void CheckCompletion()
    {
        // Check if all medicines are at required count
        bool allMedicinesComplete = medicines.All(m => m.currentCount >= m.requiredCount);
        
        // Check if all tools are highlighted
        bool allToolsComplete = tools.All(t => t.isCompleted);
        
        if (allMedicinesComplete && allToolsComplete)
        {
            OnAllItemsCompleted();
        }
    }
    
    void OnAllItemsCompleted()
    {
        Debug.Log("*** ALL ITEMS COMPLETED! ***");
        
        if (completionText != null)
        {
            completionText.text = completionMessage;
            completionText.gameObject.SetActive(true);
        }
        
        if (completionPanel != null)
        {
            completionPanel.SetActive(true);
        }
        
        // You can add more completion logic here:
        // - Play sound
        // - Show animation
        // - Enable next button
        // - Award points, etc.
    }
    
    // Call this when a new patient arrives
    public void ResetForNewPatient()
    {
        // Reset medicines
        foreach (var medicine in medicines)
        {
            medicine.currentCount = 0;
            UpdateMedicineDisplay(medicine);
        }
        
        // Reset tools
        foreach (var tool in tools)
        {
            tool.isCompleted = false;
            if (tool.backgroundImage != null)
            {
                tool.backgroundImage.color = tool.originalColor;
            }
        }
        
        // Hide completion message
        if (completionText != null)
            completionText.gameObject.SetActive(false);
        
        if (completionPanel != null)
            completionPanel.SetActive(false);
            
        Debug.Log("Patient list reset for new patient");
    }
    
    // Helper method to get completion status
    public bool IsAllCompleted()
    {
        bool allMedicinesComplete = medicines.All(m => m.currentCount >= m.requiredCount);
        bool allToolsComplete = tools.All(t => t.isCompleted);
        return allMedicinesComplete && allToolsComplete;
    }
    
    // Get individual item progress
    public float GetOverallProgress()
    {
        int totalItems = medicines.Count + tools.Count;
        if (totalItems == 0) return 0f;
        
        int completedMedicines = medicines.Count(m => m.currentCount >= m.requiredCount);
        int completedTools = tools.Count(t => t.isCompleted);
        
        return (float)(completedMedicines + completedTools) / totalItems;
    }
}
