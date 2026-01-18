using UnityEngine;

/// <summary>
/// Handles the carrying position and animation for carried objects (e.g., medicine).
/// Attach this to the same GameObject as PlayerMedicineInteraction.
/// </summary>
public class CarryingPosition : MonoBehaviour
{
    [Header("Carry Settings")]
    public Transform handPosition; // Where medicine appears when carried
    public float handDistance = 1.5f; // Distance in front of camera
    public Vector3 handOffset = new Vector3(0.3f, -0.3f, 0); // Offset from center
    public float carryScale = 0.5f; // Scale medicine when carrying (0.5 = half size)

    private Transform cam;

    void Start()
    {
        if (Camera.main != null)
        {
            cam = Camera.main.transform;
        }
        // Create hand position if not assigned
        if (handPosition == null && cam != null)
        {
            GameObject handObj = new GameObject("HandPosition");
            handPosition = handObj.transform;
            handPosition.SetParent(cam);
            handPosition.localPosition = new Vector3(handOffset.x, handOffset.y, handDistance);
        }
    }

    /// <summary>
    /// Sets the carried object's transform to the hand position and applies scale.
    /// </summary>
    public void AttachToHand(Transform obj, Vector3 originalScale)
    {
        if (handPosition == null) return;
        obj.SetParent(handPosition);
        obj.localPosition = Vector3.zero;
        obj.localRotation = Quaternion.identity;
        obj.localScale = originalScale * carryScale;
    }

    /// <summary>
    /// Optionally, animate the carried object (e.g., bobbing effect).
    /// </summary>
    public void AnimateCarriedObject(Transform obj)
    {
        float bobAmount = Mathf.Sin(Time.time * 2f) * 0.02f;
        obj.localPosition = new Vector3(0, bobAmount, 0);
    }
}
