using UnityEngine;
public class RequirementManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PatientMovement patient;
    
    [Header("Required Items")]
    [SerializeField] private UIMedicineDragItem[] requiredMedicineItems; // All medicine drag items that need completion
    [SerializeField] private UIToolDragItem[] requiredToolItems; // All tool drag items that need completion
    
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
        bool hasMedicine = requiredMedicineItems != null && requiredMedicineItems.Length > 0;
        bool hasTools = requiredToolItems != null && requiredToolItems.Length > 0;
        if (!hasMedicine && !hasTools)
            return false;

        if (hasMedicine)
        {
            foreach (UIMedicineDragItem item in requiredMedicineItems)
            {
                if (item != null)
                    return false;
            }
        }
        if (hasTools)
        {
            foreach (UIToolDragItem item in requiredToolItems)
            {
                if (item != null)
                    return false;
            }
        }
        return true;
    }
}