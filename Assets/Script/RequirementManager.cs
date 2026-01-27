using UnityEngine;
public class RequirementManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PatientMovement patient;
    
    [Header("Required Items")]
    [SerializeField] private UIDragItem[] requiredItems; // All drag items that need completion
    
    private void Update()
    {
        // Only check when patient is waiting
        if (patient != null && patient.IsWaiting())
        {
            if (AreAllRequirementsFulfilled())
            {
                Debug.Log("All requirements fulfilled! Patient exiting...");
                patient.StartExit();
                enabled = false; // Stop checking after requirements met
            }
        }
    }

    /// <summary>
    /// Checks if all required items have been completed
    /// </summary>
    private bool AreAllRequirementsFulfilled()
    {
        if (requiredItems == null || requiredItems.Length == 0)
            return false;

        foreach (UIDragItem item in requiredItems)
        {
            // If item is destroyed, it means it completed its drops
            if (item != null)
                return false; // Still has active items
        }
        
        return true; // All items are null (destroyed), requirements met!
    }
}